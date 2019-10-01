/***********************************************
 * CONFIDENTIAL AND PROPRIETARY 
 * 
 * The source code and other information contained herein is the confidential and exclusive property of
 * ZIH Corp. and is subject to the terms and conditions in your end user license agreement.
 * This source code, and any other information contained herein, shall not be copied, reproduced, published, 
 * displayed or distributed, in whole or in part, in any medium, by any means, for any purpose except as
 * expressly permitted under such license agreement.
 * 
 * Copyright ZIH Corp. 2017
 * 
 * ALL RIGHTS RESERVED
 ***********************************************/

using System.Collections.ObjectModel;
using Zebra.Windows.DevDemo.Utils;

namespace Zebra.Windows.DevDemo.Demos.ConnectionBuilder {

    public class ConnectionBuilderViewModel : IDemoViewModel {

        private string logData;

        private ObservableCollection<string> connectionPrefixes = new ObservableCollection<string>() {
                ConnectionPrefix.None,
                ConnectionPrefix.TcpMulti,
                ConnectionPrefix.Tcp,
                ConnectionPrefix.TcpStatus,
                ConnectionPrefix.Usb,
                ConnectionPrefix.UsbDirect
            };

        public string LogData {
            get => logData;
            set {
                if (logData != value) {
                    logData = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<string> ConnectionPrefixes {
            get => connectionPrefixes;
        }

        public ConnectionBuilderViewModel() {
            Name = "Connection Builder";
            logData = "Log:\n\n";
            if(BluetoothHelper.IsBluetoothSupported()) {
                connectionPrefixes.Add(ConnectionPrefix.Bluetooth);
                connectionPrefixes.Add(ConnectionPrefix.BluetoothMulti);
            }
        }
    }
}
