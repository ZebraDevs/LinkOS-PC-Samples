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
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Zebra.Sdk.Comm;
using Zebra.Sdk.Device;
using Zebra.Sdk.Printer;
using Zebra.Windows.DevDemo.Utils;

namespace Zebra.Windows.DevDemo.Demos.SmartCard {

    /// <summary>
    /// Interaction logic for SmartCardView.xaml
    /// </summary>
    public partial class SmartCardView : UserControl {
        public SmartCardView() {
            InitializeComponent();
        }

        private void RunSmartCardDemo(bool sendData) {
            try {
                SetButtonStates(false);
                ReadSmartCard(sendData);
            } catch (Exception e) {
                MessageBoxCreator.ShowError(e.Message, "Smart Card Error");
            }
        }

        private void ReadSmartCard(bool sendData) {
            Connection connection = null;
            Task.Run(() => {
                try {
                    UpdateSmartCardOutput("");

                    Application.Current.Dispatcher.Invoke(() => {
                        connection = connectionSelector.GetConnection();
                    });

                    connection.Open();

                    ZebraPrinter printer = ZebraPrinterFactory.GetInstance(connection);
                    SmartcardReader smartcardReader = SmartcardReaderFactory.Create(printer);
                    if (smartcardReader != null) {
                        byte[] response = sendData ? smartcardReader.DoCommand("8010000008") : smartcardReader.GetATR();

                        UpdateSmartCardOutput(string.Concat(response.Select(x => x.ToString("x2"))));
                        smartcardReader.Close();
                    } else {
                        MessageBoxCreator.ShowError("Printer does not have a smart card reader", "Smart Card Error");
                    }
                } catch (ConnectionException e) {
                    MessageBoxCreator.ShowError(e.Message, "Connection Error");
                } catch (ZebraPrinterLanguageUnknownException e) {
                    MessageBoxCreator.ShowError(e.Message, "Connection Error");
                } finally {
                    if (connection != null) {
                        try {
                            connection.Close();
                        } catch (ConnectionException) { }
                    }
                    SetButtonStates(true);
                }
            });
        }

        private void UpdateSmartCardOutput(string newMessage) {
            Application.Current.Dispatcher.Invoke(() => {
                responseData.Text = newMessage;
            });
        }

        private void SetButtonStates(bool enabled) {
            Application.Current.Dispatcher.Invoke(() => {
                sendDataButton.IsEnabled = enabled;
                sendAtrButton.IsEnabled = enabled;
            });
        }

        private void SendDataButton_Click(object sender, RoutedEventArgs e) {
            RunSmartCardDemo(true);
        }

        private void SendAtrButton_Click(object sender, RoutedEventArgs e) {
            RunSmartCardDemo(false);
        }
    }
}
