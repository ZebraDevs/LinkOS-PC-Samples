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

using System.Runtime.InteropServices;

namespace SmartCardExampleCode.Zebra.VirtualEoE {

    internal class BridgeImport {

        #region ZBRSXBridge.dll Imports

        [DllImport("ZBRSXBridge.dll", EntryPoint = "ZBRSXClose", CharSet = CharSet.Auto, 
                   CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern int ZBRSXClose(out int error);

        [DllImport("ZBRSXBridge.dll", EntryPoint = "ZBRSXDiscover", CharSet = CharSet.Auto, 
                   CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern int ZBRSXDiscover(object IPAddress, out object RetDevice, out int retError);

        [DllImport("ZBRSXBridge.dll", EntryPoint = "ZBRSXUSBEnumEx", CharSet = CharSet.Auto, 
                   CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern int ZBRSXUSBEnumEx(object ipAddress, out object usbDevices, out int retError);

        [DllImport("ZBRSXBridge.dll", EntryPoint = "ZBRSXConnect", CharSet = CharSet.Auto, 
                   CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern int ZBRSXConnect(string deviceId, bool encrypt, out int retError);

        [DllImport("ZBRSXBridge.dll", EntryPoint = "ZBRSXDisconnect", CharSet = CharSet.Auto, 
                   CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern int ZBRSXDisconnect(string deviceId, out int retError);

        [DllImport("ZBRSXBridge.dll", EntryPoint = "ZBRSXGetStatus", CharSet = CharSet.Auto,
                   CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern int ZBRSXGetStatus(string deviceId, out int dwStatus, out int error);

        [DllImport("ZBRSXBridge.dll", EntryPoint = "ZBRSXGetPCSCReaderNames", CharSet = CharSet.Auto,
                   CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern int ZBRSXGetPCSCReaderNames(string readerId, out object readerNames, out int error);

        #endregion //ZBRSXBridge.dll Imports
    }
}
