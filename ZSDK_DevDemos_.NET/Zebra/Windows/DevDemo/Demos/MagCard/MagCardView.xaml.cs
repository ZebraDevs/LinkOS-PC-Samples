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
using Zebra.Sdk.Comm;
using Zebra.Sdk.Device;
using Zebra.Sdk.Printer;
using Zebra.Windows.DevDemo.Utils;

namespace Zebra.Windows.DevDemo.Demos.MagCard {

    /// <summary>
    /// Interaction logic for MagCardView.xaml
    /// </summary>
    public partial class MagCardView : UserControl {

        private MagCardViewModel viewModel;

        public MagCardView() {
            InitializeComponent();

            viewModel = DataContext as MagCardViewModel;
        }

        private void ReadMagCard() {
            Connection connection = null;
            Task t = Task.Run(() => {
                try {
                    connection = connectionSelector.GetConnection();
                    connection.Open();

                    ZebraPrinter printer = ZebraPrinterFactory.GetInstance(connection);
                    MagCardReader magCardReader = MagCardReaderFactory.Create(printer);

                    if (magCardReader != null) {
                        UpdateMagCardOutput("");
                        SetButtonText("Swipe Card Now");
                        string[] trackData = magCardReader.Read(30000);

                        if (trackData[0].Equals("") && trackData[1].Equals("") && trackData[2].Equals("")) {
                            MessageBoxCreator.ShowError("Connection timed out", "Mag Card Error");
                        } else {
                            UpdateMagCardOutput(trackData[0] + "\r\n" + trackData[1] + "\r\n" + trackData[2]);
                        }
                    } else {
                        MessageBoxCreator.ShowError("Printer does not have a mag card reader", "Mag Card Error");
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
                    SetButtonText("Read Mag Card");
                    SetButtonState(true);
                }
            });
        }

        private void UpdateMagCardOutput(string message) {
            viewModel.MagCardData = message;
        }

        delegate void ParametrizedMethodInvoker5(string arg);

        private void SetButtonText(string text) {
            Application.Current.Dispatcher.Invoke(() => {
                readMagCardButton.Content = text;
            });
        }

        private void SetButtonState(bool state) {
            Application.Current.Dispatcher.Invoke(() => {
                readMagCardButton.IsEnabled = state;
            });
        }

        private void ReadMagCardButton_Click(object sender, RoutedEventArgs e) {
            try {
                SetButtonState(false);
                UpdateMagCardOutput("");
                ReadMagCard();
            } catch (Exception ex) {
                MessageBoxCreator.ShowError(ex.Message, "Mag Card Data Error");
            }
        }
    }
}
