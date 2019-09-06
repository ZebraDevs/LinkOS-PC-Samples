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
using System.Threading;

namespace SmartCardExampleCode.Zebra.VirtualEoE {

    internal class VirtualUSB : BridgeImport, IDisposable {

        #region Constants
        private const int NOT_CONNECTED = 1;
        private const int CONNECTED = 2;
        private const int ALREADY_CONNECTED = 3;
        #endregion

        #region Properties
        internal string deviceID { get; set; }
        internal string virtualError { get; set; }
        #endregion

        #region Initialization

        /// <summary>
        /// VirtualUSB class initialization
        ///    initializes properties deviceID and virtualError
        /// </summary>
        internal VirtualUSB(string ipAddr) {

            this.deviceID = string.Empty;
            this.virtualError = string.Empty;

            if (!SetDeviceID(ipAddr)) {
                throw new Exception(virtualError);
            }
            if (!Connect()) {
               throw new Exception(virtualError);
            }
        }
        #endregion

        #region Virtual USB

        /// <summary>
        /// Closes the Virtual USB connection
        /// </summary>
        /// <returns>true if successful</returns>
        internal bool Close() {
            bool isClosed = false;
            try {
                int error = 0;
                ZBRSXClose(out error);
                if (!error.Equals(0)) {
                    throw new Exception("Virtual Close Error: " + error.ToString());
                }
                isClosed = true;
            } catch (Exception ex) {
                this.virtualError = ex.Message;
            }
            return isClosed;
        }

        /// <summary>
        /// Establishes a Virtual USB connection
        /// </summary>
        /// <returns>true if successful</returns>
        internal bool Connect () {
            bool connected = true;
            try {
                int error = 0;
                if (!IsConnected()) {
                    ZBRSXConnect(this.deviceID, false, out error);
                    if (!error.Equals(0)) {
                        throw new Exception("Virtual Connection Error: " + error.ToString());
                    }
                    Thread.Sleep(500);
                    connected = IsConnected();
                }
            } catch ( Exception ex) {
                this.virtualError = ex.Message;
            }
            return connected;
        }

        /// <summary>
        /// Disconnects from a Virtual USB connection
        /// </summary>
        /// <returns>true if successful</returns>
        internal bool Disconnect() {
            bool disconnected = true;
            try {
                int error = 0;
                if (IsConnected()){
                    ZBRSXDisconnect(this.deviceID, out error);
                    if (!error.Equals(0)) {
                        disconnected = false;
                        throw new Exception("Virtual Disconnect Error: " + error.ToString());
                    }
                }
            } catch (Exception ex) {
                this.virtualError = ex.Message;
            }
            return disconnected;
        }

        /// <summary>
        /// Sets the deviceID property for a Virtual USB connection
        /// </summary>
        /// <param name="ipAddress">IP Address used to make the connection</param>
        /// <returns>true if deviceID is set</returns>
        internal bool SetDeviceID(string ipAddress) {
            bool got = false;
            try {
                int error = 0;
                object retDevice = null;
                ZBRSXDiscover(ipAddress, out retDevice, out error );
                if (!error.Equals(0)) {
                    throw new Exception("GetReader : Discover Error : " + error.ToString());
                }
                object usbDevices = null;
                ZBRSXUSBEnumEx(retDevice, out usbDevices, out error);
                if (!error.Equals(0)) {
                    throw new Exception("GetReader : EnumerateDevices Error : " + error.ToString());
                }
                string[] printers = (string[])usbDevices;
                if (printers.Length.Equals(0)) {
                    throw new Exception("GetReader : No DeviceID");
                }
                this.deviceID = printers[0];
                got = true;
            } catch ( Exception ex ) {
                this.virtualError = ex.Message;
            }
            return got;
        }

        /// <summary>
        /// Gets a list of discovered smart card readers
        /// </summary>
        /// <returns>list containing readers</returns>
        internal List<string> GetSmartCardReaders() {
            List<string> readers = new List<string>();
            try {
                object readerNames = null;
                int error = 0;
                ZBRSXGetPCSCReaderNames(this.deviceID, out readerNames, out error);
                if (readerNames != null) {
                    string[] names = (string[])readerNames;
                    foreach (string n in names) {
                        readers.Add(n);
                    }
                }
            } catch ( Exception ex) {
                this.virtualError = ex.Message;
            }
            return readers;
        }

        /// <summary>
        /// Determines if there is a Virtual USB connection
        /// </summary>
        /// <returns>true if there is a connection</returns>
        internal bool IsConnected() {
            bool isConnected = false;
            try {
                int status = 0;
                int error = 0;
                if (!string.IsNullOrEmpty(this.deviceID)) {
                    ZBRSXGetStatus(this.deviceID, out status, out error);
                    isConnected = (status == CONNECTED || status == ALREADY_CONNECTED);
                }
            } catch { }
            return isConnected;
        }

        #endregion

        #region Dispose

        internal void Dispose() {
            if (IsConnected()) {
                Disconnect();
            }
            Close();
        }

        void IDisposable.Dispose() {
            Dispose();
        }

        #endregion
    }
}
