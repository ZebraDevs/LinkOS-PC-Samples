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

using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Zebra.Sdk.Comm;
using Zebra.Sdk.Device;
using Zebra.Sdk.Printer;
using Zebra.Windows.DevDemo.Utils;

namespace Zebra.Windows.DevDemo.Demos.ListFormats {

    /// <summary>
    /// Interaction logic for ListFormatsView.xaml
    /// </summary>
    public partial class ListFormatsView : UserControl {

        private ListFormatsViewModel viewModel;

        public ListFormatsView() {
            InitializeComponent();

            viewModel = DataContext as ListFormatsViewModel;
        }

        private async void RunFormatDemo(bool isFormat) {
            await Task.Run(() => {
                SetButtonStates(false);
                PerformListFiles(isFormat);
                SetButtonStates(true);
            });
        }

        private void PerformListFiles(bool isFormat) {
            Connection printerConnection = null;
            try {
                Application.Current.Dispatcher.Invoke(() => {
                    printerConnection = connectionSelector.GetConnection();
                });

                printerConnection.Open();
                ZebraPrinter printer = ZebraPrinterFactory.GetInstance(printerConnection);

                string[] formatExtensions;
                if (printer.PrinterControlLanguage == PrinterLanguage.ZPL) {
                    formatExtensions = new string[] { "ZPL" };
                } else {
                    formatExtensions = new string[] { "FMT", "LBL" };
                }

                string[] formats = isFormat ? printer.RetrieveFileNames(formatExtensions) : printer.RetrieveFileNames();

                Application.Current.Dispatcher.Invoke(() => {
                    foreach (string format in formats) {
                        viewModel.FormatsList.Add(format);
                    }
                });
            } catch (ConnectionException e) {
                MessageBoxCreator.ShowError(e.Message, "Connection Error");
            } catch (ZebraIllegalArgumentException e) {
                MessageBoxCreator.ShowError(e.Message, "Connection Error");
            } catch (ZebraPrinterLanguageUnknownException e) {
                MessageBoxCreator.ShowError(e.Message, "Connection Error");
            } finally {
                if (printerConnection != null) {
                    printerConnection.Close();
                }
            }
        }

        private void SetButtonStates(bool enabled) {
            Application.Current.Dispatcher.Invoke(() => {
                retrieveFilesButton.IsEnabled = enabled;
                retrieveFormatsButton.IsEnabled = enabled;
            });
        }

        private void RetrieveFilesButton_Click(object sender, RoutedEventArgs e) {
            viewModel.FormatsList.Clear();
            RunFormatDemo(false);
        }

        private void RetrieveFormatsButton_Click(object sender, RoutedEventArgs e) {
            viewModel.FormatsList.Clear();
            RunFormatDemo(true);
        }
    }
}
