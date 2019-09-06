/***********************************************
 * CONFIDENTIAL AND PROPRIETARY 
 * 
 * The source code and other information contained herein is the confidential and exclusive property of
 * ZIH Corp. and is subject to the terms and conditions in your end user license agreement.
 * This source code, and any other information contained herein, shall not be copied, reproduced, published, 
 * displayed or distributed, in whole or in part, in any medium, by any means, for any purpose except as
 * expressly permitted under such license agreement.
 * 
 * Copyright ZIH Corp. 2019
 * 
 * ALL RIGHTS RESERVED
 ***********************************************/

using System;
using System.Collections.Generic;

namespace SmartCardExampleCode.Zebra.SmartCard {

    internal class ReaderLib : WinSCardLib, IDisposable {

        #region Declarations
        private bool hfSet = false;
        private bool lfSet = false;
        #endregion

        #region Properties
        public string tagSlot { get; set; } = string.Empty;
        public string readerSlot { get; set; } = string.Empty;
        public string readerError { get; set; } = string.Empty;
        #endregion

        #region Reader

        /// <summary>
        /// Discovers only Elatec
        ///    class ReaderError is set if an error is encountered
        /// </summary>
        public void DiscoverSmartCardReaders() {
            IntPtr context = IntPtr.Zero;
            List<string> lstReaders = new List<string>();
            try {
                int r = SCardEstablishContext(SCARD_SCOPE_USER, IntPtr.Zero, IntPtr.Zero, ref context);
                if (r != 0) {
                    throw new Exception("WinSCard: GetPCSCReader: EstablishContext Error: " + r.ToString());
                }
                uint byteCnt = 0;
                r = SCardListReaders(context, null, null, ref byteCnt);
                if (r != SCARD_S_SUCCESS) {
                    throw new Exception("WinSCard: GetPCSCReader: ListReaders Error: " + r.ToString());
                }
                byte[] readersList = new byte[byteCnt];
                r = SCardListReaders(context, null, readersList, ref byteCnt);
                if (r != SCARD_S_SUCCESS) {
                    throw new Exception("WinSCard: GetPCSCReader: ListReaders Error: " + r.ToString());
                }
                int indx = 0;
                string readerName = "";
                while (readersList[indx] != 0) {
                    while (readersList[indx] != 0) {
                        readerName = readerName + (char)readersList[indx];
                        indx++;
                    }
                    if (readerName.Contains("TWN4") || readerName.Contains("Elatec") ) {
                        lstReaders.Add(readerName);
                    }
                    readerName = string.Empty;
                    indx++;
                }
                if (lstReaders == null || lstReaders.Count.Equals(0)) {
                    throw new Exception("Discovery failed");
                }
                this.tagSlot = lstReaders[0];
                this.readerSlot = lstReaders[lstReaders.Count - 1];
            } catch (Exception ex) {
                this.tagSlot = string.Empty;
                this.readerSlot = string.Empty;
                throw new Exception("DiscoverSmartCardReaders: " + ex.Message);
            }
            finally {
                SCardReleaseContext(context);
            }
        }

        /// <summary>
        /// Get reader version string
        ///    class ReaderError is set if an error is encountered
        /// </summary>
        /// <returns>reader firmware version</returns>
        public string GetVersionString() {
            string version = string.Empty;
            byte[] apdu = new byte[] { 0xFF, 0x9B, 0x00, 0x00, 0x03, 0x00, 0x04, 0xFF, 0x00 };
            byte[] respBuf = null;
            if (!VirtualSlot(this.readerSlot, apdu, ref respBuf)) {
                throw new Exception("Get Version failed");
            }
            byte[] verBuf = new byte[respBuf.Length];
            int iVer = 0;
            for(int i = 0; i < respBuf.Length; i++) {
                if (respBuf[i] >= 0x20) {
                    verBuf[iVer++] = respBuf[i];
                }
            }
            version = System.Text.Encoding.Default.GetString(verBuf);
            return version;
        }

        /// <summary>
        /// Determine if a SIO card is inserted
        /// </summary>
        /// <returns>true if SIO card is inserted</returns>
        public bool IsSIOCard() {
            bool isSIO = false;
            byte[] apdu = new byte[] { 0xFF, 0x9B, 0x00, 0x00, 0x03, 0x10, 0x00, 0x20, 0x00 };
            byte[] sio = new byte[] { 0x00, 0x01, 0x00, 0x00, 0x00 };
            byte[] respBuf = null;
            if (!VirtualSlot(this.readerSlot, apdu, ref respBuf)) {
                throw new Exception("IsSIOCard failed");
            }
            if (respBuf.Length.Equals(5)) {
                isSIO = ( respBuf[0].Equals(0x00) &&
                    respBuf[1].Equals(0x01) &&
                    respBuf[2].Equals(0x00) &&
                    respBuf[3].Equals(0x00) &&
                    respBuf[4].Equals(0x00) );
            }
            return isSIO;
        }

        /// <summary>
        /// Reset the smart card reader
        /// </summary>
        public void Reset() {
            byte[] apdu = new byte[] { 0xFF, 0x9B, 0x00, 0x00, 0x02, 0x00, 0x01, 0x00 };
            VirtualSlot(this.readerSlot, apdu);
        }

        /// <summary>
        /// Set reader to EMV or Standard
        /// </summary>
        /// <param name="mode">Standard or EMV</param>
        public void SetMode(string  mode) {

            byte[] apdu = null;
            if (mode.ToLower().Equals("emv")) {
                apdu = new byte[] { 0xFF, 0xF1, 0x11, 0x01, 0x00 };
            } else {
                apdu = new byte[] { 0xFF, 0xF1, 0x11, 0x00, 0x00 };
            }
            if (!VirtualSlot(this.readerSlot, apdu)) {
                throw new Exception("Set Mode " + mode + " failed");
            }
        }

        /// <summary>
        /// Turns on or off high frequency RF
        /// </summary>
        /// <param name="on">indicates if the RF is to be turned on or off</param>
		/// <exception cref="Exception">Sending RF APDU exception</exception>
        public void SetRFHF(bool on) {
            byte[] apdu = null;
            if (on) {
                this.hfSet = true;
                apdu = new byte[5] { 0xFF, 0xF1, 0x02, 0x01, 0x00 };
            } else {
                apdu = new byte[5] { 0xFF, 0xF1, 0x02, 0x00, 0x00 };
            }
            if (!VirtualSlot(this.readerSlot, apdu)) {
                this.hfSet = false;
                throw new Exception("Set RF high frequency to " + on.ToString() + " failed");
            }
        }

        /// <summary>
        /// Turns on or off low frequency RF
        /// </summary>
        /// <param name="on">indicates if the RF is to truned on or off</param>
		/// <exception cref="Exception">Sending RF APDU exception</exception>
        public void SetRFLF(bool on) {
            byte[] apdu = null;
            if (on) {
                this.lfSet = true;
                apdu = new byte[5] { 0xFF, 0xF1, 0x01, 0x01, 0x00 };
            }  else {
                apdu = new byte[5] { 0xFF, 0xF1, 0x01, 0x00, 0x00 };
            }
            if (!VirtualSlot(this.readerSlot, apdu)) {
                this.lfSet = false;
                throw new Exception("Set RF low frequency to " + on.ToString() + " failed");
            }
        }

        #endregion

        #region Dispose
        public void Dispose() {
            if (this.hfSet) {
                SetRFHF(false);
            } else if (this.lfSet) {
                SetRFLF(false);
            }
            TagDisconnect();
        }
        #endregion

        #region Private

        private bool VirtualSlot(string readerName, byte[] apdu) {
            bool configured = false;
            try {
                TagConnect(readerName);
                byte[] respBuf = new byte[256];
                TransmitAndReceive(apdu, out respBuf);
                configured = true;
            } catch (Exception ex) {
                readerError = ex.Message;
            } finally {
                TagDisconnect();
            }
            return configured;
        }

        private bool VirtualSlot(string readerName, byte[] apdu, ref byte[] respBuf) {
            bool configured = false;
            try {
                TagConnect(readerName);
                TransmitAndReceive(apdu, out respBuf);
                configured = true;
            } catch (Exception ex) {
                readerError = ex.Message;
            } finally {
                TagDisconnect();
            }
            return configured;
        }

        #endregion
    }
}
