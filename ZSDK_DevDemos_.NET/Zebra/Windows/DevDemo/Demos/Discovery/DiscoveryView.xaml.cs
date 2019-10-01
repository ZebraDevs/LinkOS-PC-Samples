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
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Zebra.Sdk.Comm;
using Zebra.Sdk.Printer.Discovery;
using Zebra.Windows.DevDemo.Enums;
using Zebra.Windows.DevDemo.Utils;

namespace Zebra.Windows.DevDemo.Demos.Discovery {

    /// <summary>
    /// Interaction logic for Discovery.xaml
    /// </summary>
    public partial class DiscoveryView : UserControl {

        private DiscoveryViewModel viewModel;

        public DiscoveryView() {
            InitializeComponent();

            viewModel = DataContext as DiscoveryViewModel;

            discoveryMethodsDropdown.SelectedIndex = 0;
        }

        private void ResetDiscoveredPrinterData() {
            viewModel.DiscoveredPrinters.Clear();
            viewModel.DiscoveredPrinterInfo = "";
        }

        private void DiscoverPrintersButton_Click(object sender, RoutedEventArgs e) {
            try {
                ResetDiscoveredPrinterData();
                PerformDiscovery();
            } catch (Exception ex) {
                ShowErrorMessage(ex.Message);
                SetDiscoverButtonState(true);
            }
        }

        private void DiscoveryMethodsDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var selectedItem = (sender as ComboBox).SelectedItem;
            if (selectedItem != null) {
                DiscoveryMethod discoveryMethod = (DiscoveryMethod)selectedItem;
                ipAddressLabel.Visibility = discoveryMethod == DiscoveryMethod.DirectedBroadcast ? Visibility.Visible : Visibility.Collapsed;
                ipAddressInput.Visibility = discoveryMethod == DiscoveryMethod.DirectedBroadcast ? Visibility.Visible : Visibility.Collapsed;
                numberOfHopsLabel.Visibility = discoveryMethod == DiscoveryMethod.MulticastBroadcast ? Visibility.Visible : Visibility.Collapsed;
                numberOfHopsInput.Visibility = discoveryMethod == DiscoveryMethod.MulticastBroadcast ? Visibility.Visible : Visibility.Collapsed;
                subnetRangeLabel.Visibility = discoveryMethod == DiscoveryMethod.SubnetSearch ? Visibility.Visible : Visibility.Collapsed;
                subnetRangeInput.Visibility = discoveryMethod == DiscoveryMethod.SubnetSearch ? Visibility.Visible : Visibility.Collapsed;

                ResetDiscoveredPrinterData();
            }
        }

        private void DiscoveredPrintersList_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            ListBox discoveredPrintersList = sender as ListBox;
            viewModel.DiscoveredPrinterInfo = "";

            var selectedItem = discoveredPrintersList.SelectedItem;
            if (selectedItem != null) {
                DiscoveredPrinter printer = (DiscoveredPrinter)selectedItem;
                Dictionary<string, string> settings = printer.DiscoveryDataMap;

                StringBuilder sb = new StringBuilder();
                foreach (string key in settings.Keys) {
                    sb.AppendLine($"{key}: {settings[key]}");
                }

                viewModel.DiscoveredPrinterInfo = sb.ToString();
            }
        }

        private void NumberOfHopsInput_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e) {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }

        /// <exception cref="ConnectionException"></exception>
        /// <exception cref="DiscoveryException"></exception>
        private void PerformDiscovery() {
            DiscoveryHandlerImpl discoveryHandler = new DiscoveryHandlerImpl(this);
            switch (discoveryMethodsDropdown.SelectedItem) {
                case DiscoveryMethod.DirectedBroadcast:
                    NetworkDiscoverer.DirectedBroadcast(discoveryHandler, viewModel.IpAddress);
                    break;

                case DiscoveryMethod.FindPrintersNearMe:
                    NetworkDiscoverer.FindPrinters(discoveryHandler);
                    break;

                case DiscoveryMethod.LocalBroadcast:
                    NetworkDiscoverer.LocalBroadcast(discoveryHandler);
                    break;

                case DiscoveryMethod.MulticastBroadcast:
                    NetworkDiscoverer.Multicast(discoveryHandler, int.Parse(viewModel.NumberOfHops));
                    break;

                case DiscoveryMethod.SubnetSearch:
                    NetworkDiscoverer.SubnetSearch(discoveryHandler, viewModel.SubnetRange);
                    break;

                case DiscoveryMethod.ZebraUsbDrivers:
                    try {
                        discoveryHandler = null;
                        discoverPrintersButton.IsEnabled = false;
                        viewModel.DiscoveredPrinters.Clear();

                        foreach (DiscoveredPrinterDriver printer in UsbDiscoverer.GetZebraDriverPrinters()) {
                            viewModel.DiscoveredPrinters.Add(printer);
                        }
                    } finally {
                        SetDiscoverButtonState(true);
                    }
                    break;

                case DiscoveryMethod.UsbDirect:
                    try {
                        discoveryHandler = null;
                        discoverPrintersButton.IsEnabled = false;
                        viewModel.DiscoveredPrinters.Clear();

                        foreach (DiscoveredUsbPrinter printer in UsbDiscoverer.GetZebraUsbPrinters()) {
                            viewModel.DiscoveredPrinters.Add(printer);
                        }
                    } finally {
                        SetDiscoverButtonState(true);
                    }
                    break;

                case DiscoveryMethod.Bluetooth:
                    BluetoothDiscoverer.FindPrinters(discoveryHandler);
                    break;
            }
        }

        private void SetDiscoverButtonState(bool state) {
            Application.Current.Dispatcher.Invoke(() => {
                discoverPrintersButton.IsEnabled = state;
            });
        }

        private void ShowErrorMessage(string message) {
            MessageBoxCreator.ShowError(message, "Discovery Error");
        }

        private void DiscoveredPrintersList_Click(object sender, RoutedEventArgs e) {
            Clipboard.SetText(discoveredPrintersList.SelectedItem.ToString());
        }

        private class DiscoveryHandlerImpl : DiscoveryHandler {

            private DiscoveryView discoveryView;

            public DiscoveryHandlerImpl(DiscoveryView discoveryView) {
                this.discoveryView = discoveryView;
                discoveryView.SetDiscoverButtonState(false);
            }

            public void DiscoveryError(string message) {
                discoveryView.ShowErrorMessage(message);
            }

            public void DiscoveryFinished() {
                discoveryView.SetDiscoverButtonState(true);
            }

            public void FoundPrinter(DiscoveredPrinter printer) {
                Application.Current.Dispatcher.Invoke(() => {
                    discoveryView.viewModel.DiscoveredPrinters.Add(printer);
                });
            }
        }
    }
}
