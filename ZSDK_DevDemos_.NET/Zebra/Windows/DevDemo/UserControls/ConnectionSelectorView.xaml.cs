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
using System.Windows;
using System.Windows.Controls;
using Zebra.Sdk.Comm;
using Zebra.Sdk.Printer.Discovery;
using Zebra.Windows.DevDemo.Enums;

namespace Zebra.Windows.DevDemo.UserControls {

    /// <summary>
    /// Interaction logic for ConnectionSelectorView.xaml
    /// </summary>
    public partial class ConnectionSelectorView : UserControl {

        private ConnectionSelectorViewModel viewModel;

        public string IpAddress {
            get => viewModel.IpAddress;
        }

        public ConnectionType ConnectionType {
            get => viewModel.ConnectionType;
        }
        public DiscoveredPrinterDriver SelectedUsbPrinter {
            get => (DiscoveredPrinterDriver) usbPrintersDropdown.SelectedItem;
        }

        public ConnectionSelectorView() {
            InitializeComponent();

            viewModel = DataContext as ConnectionSelectorViewModel;
            connectionTypesDropdown.SelectedIndex = 0;
        }

        private void ConnectionTypesDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var selectedItem = (sender as ComboBox).SelectedItem;
            if (selectedItem != null) {
                usbPrintersLabel.Visibility = Visibility.Collapsed;
                usbPrintersDropdown.Visibility = Visibility.Collapsed;
                usbDirectLabel.Visibility = Visibility.Collapsed;
                usbDirectInput.Visibility = Visibility.Collapsed;
                ipAddressLabel.Visibility = Visibility.Collapsed;
                ipAddressInput.Visibility = Visibility.Collapsed;
                portLabel.Visibility = Visibility.Collapsed;
                portInput.Visibility =  Visibility.Collapsed;
                btAddressInput.Visibility = Visibility.Collapsed;
                btAddressLabel.Visibility = Visibility.Collapsed;

                switch (selectedItem) {
                    case ConnectionType.Network:
                        viewModel.ConnectionType = ConnectionType.Network;
                        ipAddressLabel.Visibility = Visibility.Visible;
                        ipAddressInput.Visibility = Visibility.Visible;
                        portLabel.Visibility = Visibility.Visible;
                        portInput.Visibility = Visibility.Visible;
                        break;

                    case ConnectionType.USB:
                        viewModel.ConnectionType = ConnectionType.USB;
                        usbPrintersLabel.Visibility = Visibility.Visible;
                        usbPrintersDropdown.Visibility = Visibility.Visible;
                        break;

                    case ConnectionType.UsbDirect:
                        viewModel.ConnectionType = ConnectionType.UsbDirect;
                        usbDirectLabel.Visibility = Visibility.Visible;
                        usbDirectInput.Visibility = Visibility.Visible;
                        break;

                    case ConnectionType.Bluetooth:
                        viewModel.ConnectionType = ConnectionType.Bluetooth;
                        btAddressInput.Visibility = Visibility.Visible;
                        btAddressLabel.Visibility = Visibility.Visible;
                        break;
                }

                if ((ConnectionType)selectedItem == ConnectionType.USB) {
                    GetUsbPrintersAndAddToList();
                    if (usbPrintersDropdown.Items.Count > 0) {
                        usbPrintersDropdown.SelectedIndex = 0;
                    }
                }
            }
        }
        private void GetUsbPrintersAndAddToList() {
            try {
                viewModel.UsbDevices.Clear();

                List<DiscoveredPrinterDriver> discoPrinters = UsbDiscoverer.GetZebraDriverPrinters();
                foreach (DiscoveredPrinterDriver printer in discoPrinters) {
                    viewModel.UsbDevices.Add(printer);
                }
            } catch (ConnectionException) {
                viewModel.UsbDevices.Clear();
                viewModel.UsbDevices.Add(new DiscoveredPrinterDriver("OS not supported", "", new List<string>()));
                usbPrintersDropdown.SelectedIndex = 0;
            }
        }

        public string getConnectionAddress() {
            switch(viewModel.ConnectionType) {
                case ConnectionType.Bluetooth:
                    return viewModel.MacAddress;
                case ConnectionType.UsbDirect:
                    return viewModel.SymbolicName;
                case ConnectionType.USB:
                    return viewModel.UsbPrinterName;
                default:
                    return viewModel.IpAddress;
            }
        }

        public Connection GetConnection() {
            if (viewModel.ConnectionType == ConnectionType.Network) {
                try {
                    int port = string.IsNullOrEmpty(viewModel.Port) ? 9100 : int.Parse(viewModel.Port);
                    return new TcpConnection(viewModel.IpAddress, port);
                } catch (Exception e) {
                    throw new ConnectionException(e.Message, e);
                }
            } else if (viewModel.ConnectionType == ConnectionType.UsbDirect) {
                try {
                    return new UsbConnection(viewModel.SymbolicName);
                } catch (Exception e) {
                    throw new ConnectionException(e.Message, e);
                }
            } else if(viewModel.ConnectionType == ConnectionType.USB) {
                DiscoveredPrinterDriver printer = null;
                Application.Current.Dispatcher.Invoke(() => {
                    printer = (DiscoveredPrinterDriver)usbPrintersDropdown.SelectedItem;
                });
                viewModel.UsbPrinterName = printer.PrinterName;
                return new DriverPrinterConnection(printer.PrinterName);
            } else {
                try {
                    return new BluetoothConnection(viewModel.MacAddress);
                } catch (Exception e) {
                    throw new ConnectionException(e.Message, e);
                }
            }
        }
    }
}
