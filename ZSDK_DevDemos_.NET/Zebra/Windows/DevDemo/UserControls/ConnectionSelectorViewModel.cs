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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Zebra.Sdk.Printer.Discovery;
using Zebra.Windows.DevDemo.Enums;
using Zebra.Windows.DevDemo.Utils;

namespace Zebra.Windows.DevDemo.UserControls {

    public class ConnectionSelectorViewModel : INotifyPropertyChanged {

        private ConnectionType connectionType;
        private string ipAddress;
        private string port;
        private string macAddress;
        private string usbPrinterName;
        private string symbolicName;
        private ObservableCollection<DiscoveredPrinter> usbDevices = new ObservableCollection<DiscoveredPrinter>();

        public event PropertyChangedEventHandler PropertyChanged;

        public ConnectionType ConnectionType {
            get => connectionType;
            set {
                if (connectionType != value) {
                    connectionType = value;
                    OnPropertyChanged();
                }
            }
        }

        public string IpAddress {
            get => ipAddress;
            set {
                if (ipAddress != value) {
                    ipAddress = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Port {
            get => port;
            set {
                if (port != value) {
                    port = value;
                    OnPropertyChanged();
                }
            }
        }

        public string MacAddress {
            get => macAddress;
            set {
                if (macAddress != value) {
                    macAddress = value;
                    OnPropertyChanged();
                }
            }
        }

        public string UsbPrinterName {
            get => usbPrinterName;
            set {
                if (usbPrinterName != value) {
                    usbPrinterName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string SymbolicName {
            get => symbolicName;
            set {
                if (symbolicName != value) {
                    symbolicName = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<DiscoveredPrinter> UsbDevices {
            get => usbDevices;
        }

        public List<ConnectionType> ConnectionTypes {
            get {
                List<ConnectionType> connectionTypes = Enum.GetValues(typeof(ConnectionType)).Cast<ConnectionType>().ToList();
                if (!BluetoothHelper.IsBluetoothSupported()) {
                    connectionTypes.Remove(ConnectionType.Bluetooth);
                }
                return connectionTypes;
            }
        }

        public ConnectionSelectorViewModel() {
            Port = "9100";
        }

        private void OnPropertyChanged([CallerMemberName] string memberName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
        }
    }
}
