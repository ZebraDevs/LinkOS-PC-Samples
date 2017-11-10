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
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Zebra.Sdk.Comm;
using Zebra.Sdk.Device;
using Zebra.Sdk.Printer;
using Zebra.Windows.DevDemo.Dialogs;
using Zebra.Windows.DevDemo.Utils;

namespace Zebra.Windows.DevDemo.Demos.StoredFormat {

    /// <summary>
    /// Interaction logic for StoredFormatView.xaml
    /// </summary>
    public partial class StoredFormatView : UserControl {

        private StoredFormatViewModel viewModel;

        private List<FieldDescriptionData> fieldDescDataVars;
        private ObservableCollection<FormatVariable> formatVariables;

        public StoredFormatView() {
            InitializeComponent();

            viewModel = DataContext as StoredFormatViewModel;
        }

        private void CreateFormatTableDialog() {
            SetButtonState(retrieveFormatsButton, false);
            SetFormatListState(false);

            string formatName = (string)storedFilesList.SelectedValue;
            Connection printerConnection = null;

            Task.Run(() => {
                try {
                    printerConnection = connectionSelector.GetConnection();
                    printerConnection.Open();

                    ZebraPrinter printer = ZebraPrinterFactory.GetInstance(printerConnection);
                    byte[] formatData = printer.RetrieveFormatFromPrinter(formatName);

                    fieldDescDataVars = printer.GetVariableFields(Encoding.UTF8.GetString(formatData)).ToList();
                    fieldDescDataVars = FormatFieldDescriptionDataVars(fieldDescDataVars);

                    formatVariables = new ObservableCollection<FormatVariable>();
                    for (int i = 0; i < fieldDescDataVars.Count; ++i) {
                        formatVariables.Add(new FormatVariable { FieldName = fieldDescDataVars[i].FieldName, FieldValue = "" });
                    }

                    try {
                        if (printerConnection != null) {
                            printerConnection.Close();
                        }
                    } catch (ConnectionException) { }

                    ShowStoredFormatDialog(formatName);
                } catch (Exception e) {
                    MessageBoxCreator.ShowError(e.Message, "Communication Error");
                    SetButtonState(retrieveFormatsButton, true);
                    SetFormatListState(true);
                } finally {
                    try {
                        if (printerConnection != null && printerConnection.Connected) {
                            printerConnection.Close();
                        }
                    } catch (ConnectionException) { }
                }
            });
        }

        private void PopulateListWithFormats() {
            Connection printerConnection = null;
            Task.Run(() => {
                try {
                    ClearFormatList();
                    printerConnection = connectionSelector.GetConnection();

                    printerConnection.Open();
                    ZebraPrinter printer = ZebraPrinterFactory.GetInstance(printerConnection);

                    string[] formatExtensions = printer.PrinterControlLanguage == PrinterLanguage.ZPL ? new String[] { "ZPL" } : new String[] { "FMT", "LBL" };
                    string[] formats = printer.RetrieveFileNames(formatExtensions);

                    AddFormatsToList(formats);
                } catch (ConnectionException e) {
                    MessageBoxCreator.ShowError(e.Message, "Communication Error");
                } catch (ZebraPrinterLanguageUnknownException e) {
                    MessageBoxCreator.ShowError(e.Message, "Communication Error");
                } catch (ZebraIllegalArgumentException e) {
                    MessageBoxCreator.ShowError(e.Message, "Communication Error");
                } finally {
                    try {
                        if (printerConnection != null) {
                            printerConnection.Close();
                        }
                    } catch (ConnectionException) {
                    } finally {
                        SetButtonState(retrieveFormatsButton, true);
                        SetFormatListState(true);
                    }
                }
            });
        }

        private void SetFormatListState(bool state) {
            Application.Current.Dispatcher.Invoke(() => {
                storedFilesList.IsEnabled = state;
            });
        }

        private void AddFormatsToList(string[] formats) {
            Application.Current.Dispatcher.Invoke(() => {
                foreach (string format in formats) {
                    viewModel.StoredFormatList.Add(format);
                }
            });
        }

        private void ClearFormatList() {
            SetFormatListState(false);
            Application.Current.Dispatcher.Invoke(() => {
                viewModel.StoredFormatList.Clear();
            });
        }

        private void PrintFormat(string formatName) {
            Connection printerConnection = null;
            try {
                printerConnection = connectionSelector.GetConnection();
                printerConnection.Open();

                Dictionary<int, string> formatVars = GetFormatVariables();
                ZebraPrinterFactory.GetInstance(printerConnection).PrintStoredFormat(formatName, formatVars, "UTF-8");
            } catch (ArgumentException e) {
                MessageBoxCreator.ShowError(e.Message, "Communication Error");
            } catch (ConnectionException e) {
                MessageBoxCreator.ShowError(e.Message, "Communication Error");
            } catch (ZebraPrinterLanguageUnknownException e) {
                MessageBoxCreator.ShowError(e.Message, "Communication Error");
            } finally {
                if (printerConnection != null) {
                    try {
                        printerConnection.Close();
                    } catch (ConnectionException) { }
                }
            }
        }

        private List<FieldDescriptionData> FormatFieldDescriptionDataVars(List<FieldDescriptionData> variables) {
            foreach (FieldDescriptionData data in variables) {
                data.FieldName = data.FieldName ?? "Field " + data.FieldNumber;
            }
            return variables;
        }

        private Dictionary<int, string> GetFormatVariables() {
            Dictionary<int, string> formatVars = new Dictionary<int, string>();
            for (int i = 0; i < formatVariables.Count; i++) {
                int fieldNum = fieldDescDataVars[i].FieldNumber;
                formatVars.Add(fieldNum, formatVariables[i].FieldValue);
            }
            return formatVars;
        }

        private void RetrieveFormatsButton_Click(object sender, RoutedEventArgs e) {
            try {
                SetButtonState(retrieveFormatsButton, false);
                PopulateListWithFormats();
            } catch (Exception ex) {
                MessageBoxCreator.ShowError(ex.Message, "Stored Format Error");
            }
        }

        private void SetButtonState(Button button, bool state) {
            Application.Current.Dispatcher.Invoke(() => {
                button.IsEnabled = state;
            });
        }

        private void StoredFilesList_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            if (!e.Handled) {
                e.Handled = true;
                CreateFormatTableDialog();
            }
        }

        private void ShowStoredFormatDialog(string formatName) {
            Application.Current.Dispatcher.Invoke(() => {
                StoredFormatDialog storedFormatDialog = new StoredFormatDialog(new StoredFormatDialogViewModel(formatVariables));

                Button printFormatButton = storedFormatDialog.PrintFormatButton;
                printFormatButton.Click += async (s, e) => {
                    try {
                        SetButtonState(printFormatButton, false);
                        await Task.Run(() => PrintFormat(formatName));
                    } catch (Exception ex) {
                        MessageBoxCreator.ShowError(ex.Message, "Print Format Error");
                    } finally {
                        SetButtonState(printFormatButton, true);
                    }
                };

                storedFormatDialog.ShowDialog();
                SetButtonState(retrieveFormatsButton, true);
                SetFormatListState(true);
            });
        }
    }
}