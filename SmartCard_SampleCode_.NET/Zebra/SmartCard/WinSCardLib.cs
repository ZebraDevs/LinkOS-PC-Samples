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
using System.Threading;
using System.Runtime.InteropServices;

namespace SmartCardExampleCode.Zebra.SmartCard {

    internal class WinSCardLib {

        #region Declarations
        private IntPtr context = IntPtr.Zero;
        private IntPtr conn = IntPtr.Zero;
        private uint protocol = 0;
        #endregion

        #region Structures
        [StructLayout(LayoutKind.Sequential)]
        public struct SCARD_IO_REQUEST {
            public uint dwProtocol;
            public int cbPciLength;
        }
        #endregion

        #region Constants

        public const int SCARD_S_SUCCESS = 0;
        public const int SCARD_SCOPE_USER = 0;
        public const int SCARD_SHARE_SHARED = 2;
        public const int SCARD_UNPOWER_CARD = 2;

        public const uint SCARD_F_INTERNAL_ERROR = 0x80100001;
        public const uint SCARD_E_CANCELLED = 0x80100002;
        public const uint SCARD_E_INVALID_HANDLE = 0x80100003;
        public const uint SCARD_E_INVALID_PARAMETER = 0x80100004;
        public const uint SCARD_E_INVALID_TARGET = 0x80100005;
        public const uint SCARD_E_NO_MEMORY = 0x80100006;
        public const uint SCARD_F_WAITED_TOO_LONG = 0x80100007;
        public const uint SCARD_E_INSUFFICIENT_BUFFER = 0x80100008;
        public const uint SCARD_E_UNKNOWN_READER = 0x80100009;

        public const uint SCARD_E_TIMEOUT = 0x8010000A;
        public const uint SCARD_E_SHARING_VIOLATION = 0x8010000B;
        public const uint SCARD_E_NO_SMARTCARD = 0x8010000C;
        public const uint SCARD_E_UNKNOWN_CARD = 0x8010000D;
        public const uint SCARD_E_CANT_DISPOSE = 0x8010000E;
        public const uint SCARD_E_PROTO_MISMATCH = 0x8010000F;

        public const uint SCARD_E_NOT_READY = 0x80100010;
        public const uint SCARD_E_INVALID_VALUE = 0x80100011;
        public const uint SCARD_E_SYSTEM_CANCELLED = 0x80100012;
        public const uint SCARD_F_COMM_ERROR = 0x80100013;
        public const uint SCARD_F_UNKNOWN_ERROR = 0x80100014;
        public const uint SCARD_E_INVALID_ATR = 0x80100015;
        public const uint SCARD_E_NOT_TRANSACTED = 0x80100016;
        public const uint SCARD_E_READER_UNAVAILABLE = 0x80100017;
        public const uint SCARD_P_SHUTDOWN = 0x80100018;
        public const uint SCARD_E_PCI_TOO_SMALL = 0x80100019;

        public const uint SCARD_E_READER_UNSUPPORTED = 0x8010001A;
        public const uint SCARD_E_DUPLICATE_READER = 0x8010001B;
        public const uint SCARD_E_CARD_UNSUPPORTED = 0x8010001C;
        public const uint SCARD_E_NO_SERVICE = 0x8010001D;
        public const uint SCARD_E_SERVICE_STOPPED = 0x8010001E;
        public const uint SCARD_E_UNEXPECTED = 0x8010001F;

        public const uint SCARD_W_UNSUPPORTED_CARD = 0x80100065;
        public const uint SCARD_W_UNRESPONSIVE_CARD = 0x80100066;
        public const uint SCARD_W_UNPOWERED_CARD = 0x80100067;
        public const uint SCARD_W_RESET_CARD = 0x80100068;
        public const uint SCARD_W_REMOVED_CARD = 0x80100069;

        public const int SCARD_PROTOCOL_T0 = 0x01;
        public const int SCARD_PROTOCOL_T1 = 0x02;
        public const int SCARD_ABSENT = 1;
        public const int SCARD_SWALLOWED = 3;
        public const int SCARD_POWERED = 4;
        public const int SCARD_NEGOTIABLE = 5;

        #endregion

        #region Properties
        public string winSCardError { get; set; } = string.Empty;
        #endregion

        #region "kernel32 imports"

        [DllImport("kernel32.dll")]
        private extern static IntPtr LoadLibrary(string fileName);

        [DllImport("kernel32.dll")]
        private extern static void FreeLibrary(IntPtr handle);

        [DllImport("kernel32.dll")]
        private extern static IntPtr GetProcAddress(IntPtr handle, string functionName);

        #endregion

        #region WinSCard DllImports

        [DllImport("WinSCard.dll")]
        public static extern int SCardEstablishContext(uint dwScope, IntPtr pvReserved1, IntPtr pvReserved2, ref IntPtr phContext);

        [DllImport("WinSCard.dll")]
        public static extern int SCardReleaseContext(IntPtr phContext);

        [DllImport("WinSCard.dll")]
        public static extern int SCardConnect(IntPtr hContext, string szReaderName, uint dwShareMode, uint dwPrefProtocol, ref IntPtr phCard, ref uint ActiveProtocol);

        [DllImport("WinSCard.dll")]
        public static extern int SCardDisconnect(IntPtr hCard, int disposition);

        [DllImport("WinSCard.dll", EntryPoint = "SCardListReadersA", CharSet = CharSet.Ansi)]
        public static extern int SCardListReaders(IntPtr hContext, byte[] groups, byte[] readers, ref uint pcchReaders);

        [DllImport("WinSCard.dll")]
        private static extern int SCardStatus(IntPtr hCard, string szReaderName, ref int pcchReaderLen, ref int state, ref uint protocol, IntPtr ATR, ref int ATRLen);

        public static int SCardStatusEx(IntPtr hCard, string szReaderName, ref int pcchReaderLen, ref int State, ref uint Protocol, ref byte[] ATR, ref int ATRLen) {
            IntPtr pnt = IntPtr.Zero;
            int result = -1;
            try {
                int size = Marshal.SizeOf(ATR[0]) * ATR.Length;
                pnt = Marshal.AllocHGlobal(size);
                Marshal.Copy(ATR, 0, pnt, ATR.Length);
                result = SCardStatus(hCard, szReaderName, ref pcchReaderLen, ref State, ref Protocol, pnt, ref size);
                Marshal.Copy(pnt, ATR, 0, size);
                ATRLen = size;
            } finally {
                Marshal.FreeHGlobal(pnt);
            }
            return result;
        }

        [DllImport("WinSCard.dll")]
        public static extern int SCardTransmit(IntPtr hCard, ref SCARD_IO_REQUEST pioSendRequest, byte[] SendBuff, int SendBuffLen, IntPtr pioRecvPci, byte[] RecvBuff, ref int RecvBuffLen);

        [DllImport("WinSCard.dll")]
        public static extern int SCardControl(IntPtr hCard, uint dwControlCode, IntPtr SendBuff, int SendBuffLen, IntPtr RecvBuff, int RecvBuffLen, ref int pcbBytesReturned);

        [DllImport("WinSCard.dll")]
        private static extern int SCardGetAttrib(IntPtr hCard, uint dwAttrId, IntPtr pbAttr, ref int pcbAttrLen);

        //Get the address of Pci from "WinSCard.dll".
        private static IntPtr GetPciT0() {
            IntPtr handle = LoadLibrary("WinSCard.dll");
            IntPtr pci = GetProcAddress(handle, "g_rgSCardT0Pci");
            FreeLibrary(handle);
            return pci;
        }

        private static IntPtr GetPciT1() {
            IntPtr handle = LoadLibrary("WinSCard.dll");
            IntPtr pci = GetProcAddress(handle, "g_rgSCardT1Pci");
            FreeLibrary(handle);
            return pci;
        }

        private static IntPtr GetPciRaw() {
            IntPtr handle = LoadLibrary("WinSCard.dll");
            IntPtr pci = GetProcAddress(handle, "g_rgSCardRawPci");

            FreeLibrary(handle);
            return pci;
        }

        #endregion

        #region Tag Methods

        /// <summary>
        /// Establishes a connection with a smart card tag
        /// </summary>
        /// <param name="readerName">Elatec driver slot</param>
        /// <exception cref="Exception">WinSCard error</exception>
        public void TagConnect(string readerName) {
            try {
                int ret = 0;
                if (this.context == IntPtr.Zero) {
                    ret = SCardEstablishContext(SCARD_SCOPE_USER, IntPtr.Zero, IntPtr.Zero, ref this.context);
                    if (ret != 0)
                        throw new Exception("Connection Failed");
                }
                ret = SCardConnect(this.context, readerName, SCARD_SHARE_SHARED, SCARD_PROTOCOL_T0 | SCARD_PROTOCOL_T1, ref this.conn, ref this.protocol);
                if (ret != 0) {
                    SCardReleaseContext(this.context);
                    SCardDisconnect(this.conn, SCARD_UNPOWER_CARD);
                    throw new Exception("Connection Failed");
                }

                int readerLen = 0x100;
                int state = 0;
                int rcvLen = 0;
                byte[] rcvBuf = new byte[128];

                ret = SCardStatusEx(this.conn, readerName, ref readerLen, ref state, ref this.protocol, ref rcvBuf, ref rcvLen);
                if (!ret.Equals(0))
                    throw new Exception("Getting Card Status Failed: " + GetSCardErrMsg(ret));

                switch (state) {
                    case SCARD_ABSENT:
                        throw new Exception("No card is currently inserted.");

                    case SCARD_SWALLOWED:
                        throw new Exception("There is a card in the reader in position for use. The card is not powered.");

                    case SCARD_POWERED:
                        throw new Exception("Power is being provided to the card, but the reader driver is unaware of the mode of the card.");

                    case SCARD_NEGOTIABLE:
                        throw new Exception("The card has been reset and is awaiting PTS negotiation.");
                }
            } catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Establishes a connection with a smart card tag
        /// </summary>
        /// <param name="readerName">Elatec driver slot</param>
        /// <param name="atrBuf">Connected card's ATR</param>
        /// <returns>true if connection was successful</returns>
        public bool TagConnect(string readerName, out byte[] atrBuf) {
            bool connected = false;
            atrBuf = null;
            try {
                int ret = 0;
                if (this.context == IntPtr.Zero) {
                    ret = SCardEstablishContext(SCARD_SCOPE_USER, IntPtr.Zero, IntPtr.Zero, ref this.context);
                    if (ret != 0)
                        throw new Exception("Connection Failed");
                }

                ret = SCardConnect(this.context, readerName, SCARD_SHARE_SHARED, SCARD_PROTOCOL_T0 | SCARD_PROTOCOL_T1, ref this.conn, ref this.protocol);
                if (ret != 0) {
                    SCardReleaseContext(this.context);
                    SCardDisconnect(this.conn, SCARD_UNPOWER_CARD);
                    throw new Exception("Connection Failed");
                }

                int readerLen = 0x100;
                int state = 0;
                int rcvLen = 0;
                byte[] rcvBuf = new byte[128];
                ret = SCardStatusEx(this.conn, readerName, ref readerLen, ref state, ref this.protocol, ref rcvBuf, ref rcvLen);
                if (!ret.Equals(0))
                    throw new Exception("Getting Card Status Failed: " + GetSCardErrMsg(ret));
                if (rcvLen.Equals(0)) {
                    throw new Exception("Not ATR returned");
                }

                switch (state) {
                    case SCARD_ABSENT:
                        throw new Exception("No card is currently inserted.");

                    case SCARD_SWALLOWED:
                        throw new Exception("There is a card in the reader in position for use. The card is not powered.");

                    case SCARD_POWERED:
                        throw new Exception("Power is being provided to the card, but the reader driver is unaware of the mode of the card.");

                    case SCARD_NEGOTIABLE:
                        throw new Exception("The card has been reset and is awaiting PTS negotiation.");
                }

                atrBuf = new byte[rcvLen];
                Array.Copy(rcvBuf, atrBuf, rcvLen);
                connected = true;

            } catch (Exception ex) {
                this.winSCardError = "Tag Connect: " + ex.Message;
            }
            return connected;
        }

        /// <summary>
        /// Disconnects a connected smart card tag
        /// </summary>
        public void TagDisconnect() {
            try {
                int ret = 0;
                if (this.conn != null || !this.conn.Equals(0)) {
                    ret = ReleaseConnection(this.conn);
                    if (ret != 0)
                        throw new Exception(GetSCardErrMsg(ret));
                }
                if (this.context != null || !this.context.Equals(0)) {
                    ret = SCardReleaseContext(this.context);
                    if (ret != 0)
                        throw new Exception(GetSCardErrMsg(ret));
                }
            } catch { } finally {
                this.conn = IntPtr.Zero;
                this.context = IntPtr.Zero;
            }
        }

        /// <summary>
        /// Sends an APDU command to a tag and gets a response
        /// </summary>
        /// <param name="apdu">APDU command to send</param>
        /// <param name="dataOut">response data</param>
        /// <exception cref="Exception">WinSCard error</exception>
        public void TransmitAndReceive(byte[] apdu, out byte[] dataOut) {
            dataOut = null;
            try {
                SCARD_IO_REQUEST sIO = new SCARD_IO_REQUEST {
                    dwProtocol = this.protocol,
                    cbPciLength = 8
                };

                int rcvBufLen = 1024;
                byte[] rcvBuf = new byte[rcvBufLen];
                Array.Clear(rcvBuf, 0, rcvBufLen);

                int ret = SCardTransmit(this.conn, ref sIO, apdu, apdu.Length, IntPtr.Zero, rcvBuf, ref rcvBufLen);
                if (ret != 0)
                    throw new Exception(GetSCardErrMsg(ret));

                if (rcvBuf[rcvBufLen - 2] != 0x90)
                    throw new Exception("APDU Command Failed");

                if (rcvBufLen > 2) {
                    dataOut = new byte[rcvBufLen - 2];
                    for (int i = 0; i < rcvBufLen - 2; i++)
                        dataOut[i] = rcvBuf[i];
                }
            } catch (Exception ex) {
                throw new Exception("Transmit and Receive: " + ex.Message);
            }
        }

        private static string GetSCardErrMsg(int ReturnCode) {

            switch ((uint)ReturnCode) {
                case SCARD_E_CANCELLED:
                    return ("The action was canceled by an SCardCancel request.");
                case SCARD_E_CANT_DISPOSE:
                    return ("The system could not dispose of the media in the requested manner.");
                case SCARD_E_CARD_UNSUPPORTED:
                    return ("The smart card does not meet minimal requirements for support.");
                case SCARD_E_DUPLICATE_READER:
                    return ("The reader driver didn't produce a unique reader name.");
                case SCARD_E_INSUFFICIENT_BUFFER:
                    return ("The data buffer for returned data is too small for the returned data.");
                case SCARD_E_INVALID_ATR:
                    return ("An ATR string obtained from the registry is not a valid ATR string.");
                case SCARD_E_INVALID_HANDLE:
                    return ("The supplied handle was invalid.");
                case SCARD_E_INVALID_PARAMETER:
                    return ("One or more of the supplied parameters could not be properly interpreted.");
                case SCARD_E_INVALID_TARGET:
                    return ("Registry startup information is missing or invalid.");
                case SCARD_E_INVALID_VALUE:
                    return ("One or more of the supplied parameter values could not be properly interpreted.");
                case SCARD_E_NOT_READY:
                    return ("The reader or card is not ready to accept commands.");
                case SCARD_E_NOT_TRANSACTED:
                    return ("An attempt was made to end a non-existent transaction.");
                case SCARD_E_NO_MEMORY:
                    return ("Not enough memory available to complete this command.");
                case SCARD_E_NO_SERVICE:
                    return ("The smart card resource manager is not running.");
                case SCARD_E_NO_SMARTCARD:
                    return ("The operation requires a smart card, but no smart card is currently in the device.");
                case SCARD_E_PCI_TOO_SMALL:
                    return ("The PCI receive buffer was too small.");
                case SCARD_E_PROTO_MISMATCH:
                    return ("The requested protocols are incompatible with the protocol currently in use with the card.");
                case SCARD_E_READER_UNAVAILABLE:
                    return ("The specified reader is not currently available for use.");
                case SCARD_E_READER_UNSUPPORTED:
                    return ("The reader driver does not meet minimal requirements for support.");
                case SCARD_E_SERVICE_STOPPED:
                    return ("The smart card resource manager has shut down.");
                case SCARD_E_SHARING_VIOLATION:
                    return ("The smart card cannot be accessed because of other outstanding connections.");
                case SCARD_E_SYSTEM_CANCELLED:
                    return ("The action was canceled by the system, presumably to log off or shut down.");
                case SCARD_E_TIMEOUT:
                    return ("The user-specified timeout value has expired.");
                case SCARD_E_UNKNOWN_CARD:
                    return ("The specified smart card name is not recognized.");
                case SCARD_E_UNKNOWN_READER:
                    return ("The specified reader name is not recognized.");
                case SCARD_F_COMM_ERROR:
                    return ("An internal communications error has been detected.");
                case SCARD_F_INTERNAL_ERROR:
                    return ("An internal consistency check failed.");
                case SCARD_F_UNKNOWN_ERROR:
                    return ("An internal error has been detected, but the source is unknown.");
                case SCARD_F_WAITED_TOO_LONG:
                    return ("An internal consistency timer has expired.");
                case SCARD_S_SUCCESS:
                    return ("No error was encountered.");
                case SCARD_W_REMOVED_CARD:
                    return ("The smart card has been removed, so that further communication is not possible.");
                case SCARD_W_RESET_CARD:
                    return ("The smart card has been reset, so any shared state information is invalid.");
                case SCARD_W_UNPOWERED_CARD:
                    return ("Power has been removed from the smart card, so that further communication is not possible.");
                case SCARD_W_UNRESPONSIVE_CARD:
                    return ("The smart card is not responding to a reset.");
                case SCARD_W_UNSUPPORTED_CARD:
                    return ("The reader cannot communicate with the card, due to ATR string configuration conflicts.");
                default:
                    return ("Unknown error code returned: " + ReturnCode.ToString());
            }
        }

        private int ReleaseConnection(IntPtr conn) {
            int ret = 0;
            try {
                if (conn != IntPtr.Zero) {
                    for (int i = 0; i < 5; i++) {
                        ret = SCardDisconnect(conn, SCARD_UNPOWER_CARD);
                        Thread.Sleep(500);
                        if (ret == 0) break;
                    }
                }
            } catch { }
            return ret;
        }

        #endregion
    }
}
