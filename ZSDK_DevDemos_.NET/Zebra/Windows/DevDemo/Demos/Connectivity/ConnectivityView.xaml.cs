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

using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Zebra.Sdk.Comm;
using Zebra.Sdk.Printer;
using Zebra.Sdk.Printer.Discovery;
using Zebra.Windows.DevDemo.Enums;

namespace Zebra.Windows.DevDemo.Demos.Connectivity {

    /// <summary>
    /// Interaction logic for Connectivity.xaml
    /// </summary>
    public partial class ConnectivityView : UserControl {

        public ConnectivityView() {
            InitializeComponent();
        }

        private async void TestButton_Click(object sender, RoutedEventArgs e) {
            SetTestButtonState(false);
            Connection printerConnection = null;
            if (connectionSelector.ConnectionType == ConnectionType.Network) {
                try {
                    printerConnection = connectionSelector.GetConnection();
                } catch (ConnectionException) {
                    UpdateStatusBarOnGui("Invalid Address and/or Port", ConnectionState.Error);
                    await Task.Delay(1000);
                    UpdateStatusBarOnGui("Not Connected", ConnectionState.Error);
                    return;
                }
            } else if (connectionSelector.ConnectionType == ConnectionType.Bluetooth) {
                try {
                    printerConnection = connectionSelector.GetConnection();
                } catch (ConnectionException) {
                    UpdateStatusBarOnGui("Invalid Mac Address", ConnectionState.Error);
                    await Task.Delay(1000);
                    UpdateStatusBarOnGui("Not Connected", ConnectionState.Error);
                }
            } else if (connectionSelector.SelectedUsbPrinter is DiscoveredPrinterDriver printer) {
                try {
                    printerConnection = new DriverPrinterConnection(printer.PrinterName);
                } catch (ConnectionException) {
                    return;
                }
            } else if (connectionSelector.ConnectionType == ConnectionType.UsbDirect) {
                try {
                    printerConnection = connectionSelector.GetConnection();
                } catch (ConnectionException) {
                    UpdateStatusBarOnGui("Invalid Address", ConnectionState.Error);
                    await Task.Delay(1000);
                    UpdateStatusBarOnGui("Not Connected", ConnectionState.Error);
                    return;
                }
            } else {
                return;
            }
            await Task.Run(async () => {
                try {
                    UpdateStatusBarOnGui("Connecting...", ConnectionState.Progress);
                    await Task.Delay(1500);

                    printerConnection.Open();

                    UpdateStatusBarOnGui("Connected", ConnectionState.Success);
                    await Task.Delay(1500);

                    UpdateStatusBarOnGui("Determining Printer Language...", ConnectionState.Progress);
                    await Task.Delay(1500);

                    PrinterLanguage printerLanguage = ZebraPrinterFactory.GetInstance(printerConnection).PrinterControlLanguage;
                    UpdateStatusBarOnGui("Printer Language " + printerLanguage.ToString(), ConnectionState.Info);
                    await Task.Delay(1500);

                    UpdateStatusBarOnGui("Sending Data...", ConnectionState.Progress);

                    printerConnection.Write(GetConfigLabel(printerLanguage));
                } catch (ConnectionException) {
                    UpdateStatusBarOnGui("Communications Error", ConnectionState.Error);
                } catch (ZebraPrinterLanguageUnknownException) {
                    UpdateStatusBarOnGui("Invalid Printer Language", ConnectionState.Error);
                } finally {
                    try {
                        await Task.Delay(1000);
                        UpdateStatusBarOnGui("Disconnecting...", ConnectionState.Progress);
                        if (printerConnection != null) {
                            printerConnection.Close();
                        }

                        await Task.Delay(1000);
                        UpdateStatusBarOnGui("Not Connected", ConnectionState.Error);
                    } catch (ConnectionException) {
                    } finally {
                        SetTestButtonState(true);
                    }
                }
            });
        }

        private void SetTestButtonState(bool state) {
            Application.Current.Dispatcher.Invoke(() => {
                testButton.IsEnabled = state;
            });
        }

        private Brush GetBrushFromConnectionState(ConnectionState connectionState) {
            switch (connectionState) {
                case ConnectionState.Success:
                    return Brushes.Green;

                case ConnectionState.Progress:
                    return Brushes.Goldenrod;

                case ConnectionState.Info:
                    return Brushes.Blue;

                case ConnectionState.Error:
                default:
                    return Brushes.Red;
            }
        }

        private void UpdateStatusBarOnGui(string statusMessage, ConnectionState connectionState) {
            Application.Current.Dispatcher.Invoke(() => {
                connectionStatus.Foreground = GetBrushFromConnectionState(connectionState);
                connectionStatus.Text = statusMessage;
            });
        }

        /*
		 * Returns the command for a test label depending on the printer control language
		 * The test label is a box with the word "TEST" inside of it
		 * 
		 * _________________________
		 * |                       |
		 * |                       |
		 * |        TEST           |
		 * |                       |
		 * |                       |
		 * |_______________________|
		 * 
		 */
        private byte[] GetConfigLabel(PrinterLanguage printerLanguage) {
            byte[] configLabel = null;
            if (printerLanguage == PrinterLanguage.ZPL) {
                configLabel = Encoding.UTF8.GetBytes("^XA^FO17,16^GB379,371,8^FS^FT65,255^A0N,135,134^FDTEST^FS^XZ");
            } else if (printerLanguage == PrinterLanguage.CPCL) {
                string cpclConfigLabel = "! 0 200 200 406 1\r\n" + "ON-FEED IGNORE\r\n" + "BOX 20 20 380 380 8\r\n" + "T 0 6 137 177 TEST\r\n" + "PRINT\r\n";
                configLabel = Encoding.UTF8.GetBytes(cpclConfigLabel);
            }
            return configLabel;
        }
    }
}
