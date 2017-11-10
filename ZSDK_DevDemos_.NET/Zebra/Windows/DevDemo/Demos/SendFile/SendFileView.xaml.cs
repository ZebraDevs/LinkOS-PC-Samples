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
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Zebra.Sdk.Comm;
using Zebra.Sdk.Printer;
using Zebra.Windows.DevDemo.Utils;

namespace Zebra.Windows.DevDemo.Demos.SendFile {

    /// <summary>
    /// Interaction logic for SendFileView.xaml
    /// </summary>
    public partial class SendFileView : UserControl {

        public SendFileView() {
            InitializeComponent();
        }

        private void SendFileToPrinter() {
            string filePath = null;
            Connection printerConnection = null;
            Task.Run(() => {
                try {
                    printerConnection = connectionSelector.GetConnection();
                    printerConnection.Open();
                    ZebraPrinter printer = ZebraPrinterFactory.GetInstance(printerConnection);

                    filePath = CreateDemoFile(printer.PrinterControlLanguage);
                    printer.SendFileContents(filePath);
                } catch (ConnectionException e) {
                    MessageBoxCreator.ShowError(e.Message, "Connection Error");
                } catch (IOException e) {
                    MessageBoxCreator.ShowError(e.Message, "IO Error");
                } catch (ZebraPrinterLanguageUnknownException e) {
                    MessageBoxCreator.ShowError(e.Message, "Connection Error");
                } catch (Exception e) {
                    MessageBoxCreator.ShowError(e.Message, "Send File Error");
                } finally {
                    try {
                        if (printerConnection != null)
                            printerConnection.Close();
                    } catch (ConnectionException) { }

                    if (filePath != null) {
                        try {
                            new FileInfo(filePath).Delete();
                        } catch { }
                    }

                    SetSendFileButtonState(true);
                }
            });
        }

        private string CreateDemoFile(PrinterLanguage pl) {
            string tempFilePath = $"{Path.GetTempPath()}TEST_ZEBRA.LBL";
            using (FileStream tmpFile = new FileStream(tempFilePath, FileMode.Create)) {
                byte[] configLabel = null;
                if (pl == PrinterLanguage.ZPL) {
                    configLabel = Encoding.UTF8.GetBytes("^XA^FO17,16^GB379,371,8^FS^FT65,255^A0N,135,134^FDTEST^FS^XZ");
                } else if (pl == PrinterLanguage.CPCL) {
                    string cpclConfigLabel = "! 0 200 200 406 1\r\n" + "ON-FEED IGNORE\r\n" + "BOX 20 20 380 380 8\r\n" + "T 0 6 137 177 TEST\r\n" + "PRINT\r\n";
                    configLabel = Encoding.UTF8.GetBytes(cpclConfigLabel);
                }

                tmpFile.Write(configLabel, 0, configLabel.Length);
                tmpFile.Flush();
            }
            return new FileInfo(tempFilePath).FullName;
        }

        private void SendFileButton_Click(object sender, RoutedEventArgs e) {
            try {
                SetSendFileButtonState(false);
                SendFileToPrinter();
            } catch (Exception ex) {
                MessageBoxCreator.ShowError(ex.Message, "Send File Error");
            }
        }

        private void SetSendFileButtonState(bool state) {
            Application.Current.Dispatcher.Invoke(() => {
                sendFileButton.IsEnabled = state;
            });
        }
    }
}
