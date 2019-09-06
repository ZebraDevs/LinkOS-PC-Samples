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

namespace SmartCardExampleCode.Zebra.SmartCard {

    internal class ContactlessOps : WinSCardLib, IDisposable {

        #region Properties
        public bool tagConnected { get; set; } = false;
        #endregion

        #region MIFARE Methods

        /// <summary>
        /// Sends an authentication APDU
        /// </summary>
        /// <param name="block">block to authenticate</param>
        /// <param name="keyType">key to use</param>
        /// <param name="keyNumber">number of the key</param>
		/// <exception cref="Exception">WinSCard error</exception>
        public void MifareAuthenticate(int block, char keyType, byte keyNumber) {
            byte kType = (keyType.Equals('B')) ? (byte)0x61 : (byte)0x60;
            byte[] blk = BlockToBytes(block);
            byte[] apdu = { 0xFF, 0x86, 0x00, 0x00, 0x05, 0x01, blk[0], blk[1], kType, keyNumber };
            try {
                TransmitAndReceive(apdu, out byte[] respBuf);
            } catch ( Exception ex) {
                throw new Exception("Authentication error: " + ex);
            }
        }

        /// <summary>
        /// Sends a block read APDU
        /// </summary>
        /// <param name="block">block to read</param>
        /// <param name="dataOut">data read</param>
		/// <exception cref="Exception">WinSCard error</exception>
        public void MifareBlockRead(int block, out byte[] dataOut) {
            dataOut = null;
            byte[] blk = BlockToBytes(block);
            byte[] cmd = new byte[] { 0xFF, 0xB0, blk[0], blk[1], 0x00 };
            try {
                TransmitAndReceive(cmd, out dataOut);
            } catch (Exception ex) {
                throw new Exception("Block " + block.ToString() + " read error: " + ex.Message);
            }
        }

        /// <summary>
        /// Senda a block write APDU
        /// </summary>
        /// <param name="block">block to write</param>
        /// <param name="data">data to write</param>
		/// <exception cref="Exception">WinSCard error</exception>
        public void MifareBlockWrite(int block, byte[] data) {
            byte[] blk = BlockToBytes(block);
            byte[] cmd = new byte[data.Length + 5];
            int cmdLength = cmd.Length;
            cmd[0] = 0xFF;
            cmd[1] = 0xD6;
            cmd[2] = blk[0]; //sector
            cmd[3] = blk[1]; //block
            cmd[4] = (byte)data.Length;
            for (int i = 0; i < data.Length; i++) {
                cmd[i + 5] = data[i];
            }
            try {
                TransmitAndReceive(cmd, out byte[] respBuf);
            } catch ( Exception ex) {
                throw new Exception("Block " + block.ToString() + " write error: " + ex.Message);
            }
        }

        /// <summary>
        /// Load a key
        /// </summary>
        /// <param name="keyNumber">Where the key will be loaded</param>
        /// <param name="key">key data</param>
        public void MifareLoadKey(byte keyNumber, byte[] key) {
            int cmdLength = key.Length + 5;
            byte[] cmd = new byte[cmdLength];
            cmd[0] = 0xFF;
            cmd[1] = 0x82;
            cmd[2] = 0x00;
            cmd[3] = keyNumber;
            cmd[4] = (byte)key.Length;
            for (int i = 0; i < key.Length; i++) {
                cmd[i + 5] = key[i];
            }
            try {
                TransmitAndReceive(cmd, out byte[] respBuf);
            } catch ( Exception ex) {
                throw new Exception("Load Key error: " + ex.Message);
            }
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Class disposal
        ///    disconnects tag if still connected
        /// </summary>
        void IDisposable.Dispose() {
            if (this.tagConnected) {
                this.tagConnected = false;
                TagDisconnect();
            }
        }
        #endregion

        #region Private Routines

        private byte[] BlockToBytes(int block) {
            byte[] blk = { (byte)((block & 0xFF00) >> 8), (byte)(block & 0x00FF) };
            return blk;
        }

        #endregion
    }
}
