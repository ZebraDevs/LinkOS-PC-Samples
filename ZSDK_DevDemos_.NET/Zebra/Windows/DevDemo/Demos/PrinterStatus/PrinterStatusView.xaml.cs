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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Zebra.Sdk.Comm;
using Zebra.Sdk.Printer;
using Zebra.Windows.DevDemo.Utils;
using ZebraPrinterStatus = Zebra.Sdk.Printer.PrinterStatus;

namespace Zebra.Windows.DevDemo.Demos.PrinterStatus {

    /// <summary>
    /// Interaction logic for PrinterStatusView.xaml
    /// </summary>
    public partial class PrinterStatusView : UserControl {

        public PrinterStatusView() {
            InitializeComponent();
        }

        private void GetPrinterStatus() {
            Connection printerConnection = null;
            Task.Run(() => {
                try {
                    printerConnection = connectionSelector.GetConnection();
                    printerConnection.Open();

                    ZebraPrinter printer = ZebraPrinterFactory.GetInstance(printerConnection);
                    ZebraPrinterLinkOs linkOsPrinter = ZebraPrinterFactory.CreateLinkOsPrinter(printer);

                    ZebraPrinterStatus status = (linkOsPrinter != null) ? linkOsPrinter.GetCurrentStatus() : printer.GetCurrentStatus();

                    string[] printerStatusString = new PrinterStatusMessages(status).GetStatusMessage();
                    List<string> printerStatusPrefix = GetPrinterStatusPrefix(status);

                    StringBuilder sb = new StringBuilder();
                    foreach (string s in printerStatusPrefix) {
                        sb.AppendLine(s);
                    }

                    foreach (string s in printerStatusString) {
                        sb.AppendLine(s);
                    }

                    Application.Current.Dispatcher.Invoke(() => {
                        printerStatus.Text = sb.ToString();
                    });
                } catch (ConnectionException e) {
                    MessageBoxCreator.ShowError(e.Message, "Connection Error");
                } catch (ZebraPrinterLanguageUnknownException e) {
                    MessageBoxCreator.ShowError(e.Message, "Connection Error");
                } finally {
                    if (printerConnection != null) {
                        try {
                            printerConnection.Close();
                        } catch (ConnectionException) {
                        } finally {
                            SetTestButtonState(true);
                        }
                    } else {
                        SetTestButtonState(true);
                    }
                }
            });
        }

        private List<string> GetPrinterStatusPrefix(ZebraPrinterStatus printerStatus) {
            bool ready = printerStatus != null ? printerStatus.isReadyToPrint : false;
            string readyString = "Printer " + (ready ? "ready" : "not ready");
            string labelsInBatch = "Labels in batch: " + Convert.ToString(printerStatus.labelsRemainingInBatch);
            string labelsInRecvBuffer = "Labels in buffer: " + Convert.ToString(printerStatus.numberOfFormatsInReceiveBuffer);
            return new List<string> { readyString, labelsInBatch, labelsInRecvBuffer };
        }

        private void GetPrinterStatusButton_Click(object sender, RoutedEventArgs e) {
            try {
                SetTestButtonState(false);
                GetPrinterStatus();
            } catch (Exception ex) {
                MessageBoxCreator.ShowError(ex.Message, "Printer Status Error");
            }
        }

        private void SetTestButtonState(bool state) {
            Application.Current.Dispatcher.Invoke(() => {
                getPrinterStatusButton.IsEnabled = state;
            });
        }
    }
}
