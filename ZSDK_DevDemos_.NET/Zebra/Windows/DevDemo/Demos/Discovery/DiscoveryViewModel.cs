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
using System.Linq;
using Zebra.Sdk.Printer.Discovery;
using Zebra.Windows.DevDemo.Enums;
using Zebra.Windows.DevDemo.Utils;

namespace Zebra.Windows.DevDemo.Demos.Discovery {

    public class DiscoveryViewModel : IDemoViewModel {

        private ObservableCollection<DiscoveredPrinter> discoveredPrinters = new ObservableCollection<DiscoveredPrinter>();
        private string discoveredPrinterInfo;
        private string ipAddress;
        private string numberOfHops;
        private string subnetRange;

        public List<DiscoveryMethod> DiscoveryMethods {
            get {
                List<DiscoveryMethod> discoveryMethods = Enum.GetValues(typeof(DiscoveryMethod)).Cast<DiscoveryMethod>().ToList();
                if(!BluetoothHelper.IsBluetoothSupported()) {
                    discoveryMethods.Remove(DiscoveryMethod.Bluetooth);
                }
                return discoveryMethods;
            }
        }

        public ObservableCollection<DiscoveredPrinter> DiscoveredPrinters {
            get => discoveredPrinters;
        }

        public string DiscoveredPrinterInfo {
            get => discoveredPrinterInfo;
            set {
                if (discoveredPrinterInfo != value) {
                    discoveredPrinterInfo = value;
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

        public string NumberOfHops {
            get => numberOfHops;
            set {
                if (numberOfHops != value) {
                    numberOfHops = value;
                    OnPropertyChanged();
                }
            }
        }

        public string SubnetRange {
            get => subnetRange;
            set {
                if (subnetRange != value) {
                    subnetRange = value;
                    OnPropertyChanged();
                }
            }
        }

        public DiscoveryViewModel() {
            Name = "Discovery";
            NumberOfHops = "5";
        }
    }
}
