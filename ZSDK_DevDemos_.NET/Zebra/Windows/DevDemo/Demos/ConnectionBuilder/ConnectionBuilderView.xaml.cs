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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Zebra.Sdk.Comm;
using Zebra.Sdk.Printer;
using Zebra.Windows.DevDemo.Utils;
using ZebraConnectionBuilder = Zebra.Sdk.Comm.ConnectionBuilder;
using ZebraPrinterStatus = Zebra.Sdk.Printer.PrinterStatus;

namespace Zebra.Windows.DevDemo.Demos.ConnectionBuilder {

    /// <summary>
    /// Interaction logic for ConnectionBuilderView.xaml
    /// </summary>
    public partial class ConnectionBuilderView : UserControl {

        private ConnectionBuilderViewModel viewModel;
        private Connection connection = null;

        public ConnectionBuilderView() {
            InitializeComponent();

            viewModel = DataContext as ConnectionBuilderViewModel;
        }

        private void TestConnectionString() {
            Task.Run(() => {
                try {
                    ClearProgress();
                    connection = ZebraConnectionBuilder.Build(GetConnectionStringForSdk());
                    PublishProgress("Connection string evaluated as class type " + connection.GetType().Name);
                    connection.Open();

                    PublishProgress("Connection opened successfully");

                    if (IsAttemptingStatusConnection()) {
                        ZebraPrinterLinkOs printer = ZebraPrinterFactory.GetLinkOsPrinter(connection);
                        PublishProgress("Created a printer, attempting to retrieve status");

                        ZebraPrinterStatus status = printer.GetCurrentStatus();
                        PublishProgress("Is printer ready to print? " + status.isReadyToPrint);
                    } else {
                        ZebraPrinter printer = ZebraPrinterFactory.GetInstance(connection);
                        PublishProgress("Created a printer, attempting to print a config label");
                        printer.PrintConfigurationLabel();
                    }

                    PublishProgress("Closing connection");
                } catch (ConnectionException) {
                    MessageBoxCreator.ShowError("Connection could not be opened", "Error");
                } catch (ZebraPrinterLanguageUnknownException) {
                    MessageBoxCreator.ShowError("Could not create printer", "Error");
                } finally {
                    if (connection != null) {
                        try {
                            connection.Close();
                        } catch (ConnectionException) { } finally {
                            connection = null;
                            SetTestButtonState(true);
                        }
                    } else {
                        SetTestButtonState(true);
                    }
                }
            });
        }

        private void SetTestButtonState(bool state) {
            Application.Current.Dispatcher.Invoke(() => {
                testConnectionStringButton.IsEnabled = state;
            });
        }

        private void ClearProgress() {
            Application.Current.Dispatcher.Invoke(() => {
                logData.Text = "Log:\nTesting string: " + GetConnectionStringForSdk() + "\n";
            });
        }

        private string GetConnectionStringForSdk() {
            string finalConnectionString = "";
            Application.Current.Dispatcher.Invoke(() => {
                string selectedPrefix = "";
                if (connectionPrefixDropdown.SelectedIndex > 0) {
                    selectedPrefix = connectionPrefixDropdown.SelectedValue + ":";
                }

                string userSuppliedDescriptionString = usbDriverIpAddress.Text;
                finalConnectionString = selectedPrefix + userSuppliedDescriptionString;
            });
            return finalConnectionString;
        }

        private bool IsAttemptingStatusConnection() {
            return connection.GetType().Name.Contains("Status");
        }

        private void PublishProgress(string progress) {
            Application.Current.Dispatcher.Invoke(() => {
                logData.Text = logData.Text + progress + Environment.NewLine;
            });
        }

        private void ConnectionPrefixDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            connectionString.Text = GetConnectionStringForSdk();
            SetAddressTextBlock();
        }

        private void SetAddressTextBlock() {
            switch(connectionPrefixDropdown.SelectedValue) {
                case ConnectionPrefix.Tcp:
                case ConnectionPrefix.TcpMulti:
                case ConnectionPrefix.TcpStatus:
                    AddressTextBlock.Text = "IP Address:";
                    break;
                case ConnectionPrefix.Bluetooth:
                case ConnectionPrefix.BluetoothMulti:
                    AddressTextBlock.Text = "BT Address:";
                    break;
                case ConnectionPrefix.Usb:
                    AddressTextBlock.Text = "USB Driver:";
                    break;
                case ConnectionPrefix.UsbDirect:
                    AddressTextBlock.Text = "Symbolic Name:";
                    break;
                default:
                    AddressTextBlock.Text = "Address:";
                    break;
            };
        }

        private void TestConnectionStringButton_Click(object sender, RoutedEventArgs e) {
            try {
                viewModel.LogData = "Log:\n\n";
                SetTestButtonState(false);

                TestConnectionString();
            } catch (Exception ex) {
                MessageBoxCreator.ShowError(ex.Message, "Connection Builder Error");
            }
        }

        private void UsbDriverIpAddress_KeyUp(object sender, KeyEventArgs e) {
            connectionString.Text = GetConnectionStringForSdk();
        }
    }
}
