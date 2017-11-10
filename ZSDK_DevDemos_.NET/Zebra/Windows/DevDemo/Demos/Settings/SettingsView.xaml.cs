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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Zebra.Sdk.Comm;
using Zebra.Sdk.Printer;
using Zebra.Sdk.Settings;
using Zebra.Windows.DevDemo.Utils;

namespace Zebra.Windows.DevDemo.Demos.Settings {

    /// <summary>
    /// Interaction logic for SettingsView.xaml
    /// </summary>
    public partial class SettingsView : UserControl {

        private SettingsViewModel viewModel;

        public SettingsView() {
            InitializeComponent();

            viewModel = DataContext as SettingsViewModel;
        }

        private void UpdateSettingsTable() {
            Connection connection = null;
            try {
                connection = connectionSelector.GetConnection();
                connection.Open();

                ZebraPrinter genericPrinter = ZebraPrinterFactory.GetInstance(connection);
                ZebraPrinterLinkOs printer = ZebraPrinterFactory.CreateLinkOsPrinter(genericPrinter);

                if (printer != null) {
                    Dictionary<string, Sdk.Settings.Setting> settings = printer.GetAllSettings();

                    Application.Current.Dispatcher.Invoke(() => {
                        if (settings != null) {
                            foreach (string key in settings.Keys) {
                                viewModel.Settings.Add(new Setting { Key = key, Value = settings[key].Value, Range = printer.GetSettingRange(key) });
                            }
                        } else {
                            MessageBoxCreator.ShowError("Error reading settings", "Settings Error");
                        }

                        printerSettingsGrid.UnselectAll();
                    });
                } else {
                    MessageBoxCreator.ShowError("Connected printer does not support settings", "Connection Error");
                }
            } catch (ConnectionException e) {
                MessageBoxCreator.ShowError(e.Message, "Connection Error");
            } catch (ZebraPrinterLanguageUnknownException e) {
                MessageBoxCreator.ShowError(e.Message, "Connection Error");
            } catch (SettingsException e) {
                MessageBoxCreator.ShowError(e.Message, "Settings Error");
            } catch (Exception e) {
                MessageBoxCreator.ShowError(e.Message, "Save Settings Error");
            } finally {
                SetButtonStates(true);
                try {
                    if (connection != null) {
                        connection.Close();
                    }
                } catch (ConnectionException) { }
            }
        }

        private void ClearSettings() {
            Application.Current.Dispatcher.Invoke(() => {
                viewModel.Settings.Clear();
            });
        }

        private bool SaveModifiedSettingsToPrinter() {
            bool result = false;
            Connection connection = null;
            try {
                connection = connectionSelector.GetConnection();
                connection.Open();

                ZebraPrinter genericPrinter = ZebraPrinterFactory.GetInstance(connection);
                ZebraPrinterLinkOs printer = ZebraPrinterFactory.CreateLinkOsPrinter(genericPrinter);

                if (printer != null) {
                    foreach (string key in viewModel.ModifiedSettings.Keys) {
                        if (printer.IsSettingReadOnly(key) == false) {
                            printer.SetSetting(key, viewModel.ModifiedSettings[key]);
                        }
                    }

                    viewModel.ModifiedSettings.Clear();
                    result = true;
                } else {
                    MessageBoxCreator.ShowError("Connected printer does not support settings", "Connection Error");
                }
            } catch (ConnectionException e) {
                MessageBoxCreator.ShowError(e.Message, "Connection Error");
            } catch (ZebraPrinterLanguageUnknownException e) {
                MessageBoxCreator.ShowError(e.Message, "Connection Error");
            } catch (SettingsException e) {
                MessageBoxCreator.ShowError(e.Message, "Settings Error");
            } catch (Exception e) {
                MessageBoxCreator.ShowError(e.Message, "Save Settings Error");
            } finally {
                try {
                    if (connection != null) {
                        connection.Close();
                    }
                } catch (ConnectionException) { }
            }
            return result;
        }

        private void SetButtonStates(bool enabled) {
            Application.Current.Dispatcher.Invoke(() => {
                saveSettingsButton.IsEnabled = enabled;
                getSettingsButton.IsEnabled = enabled;
            });
        }

        private async void SaveSettingsButton_Click(object sender, RoutedEventArgs e) {
            try {
                SetButtonStates(false);
                if (await Task.Run(() => SaveModifiedSettingsToPrinter())) {
                    ClearSettings();
                    await Task.Run(() => UpdateSettingsTable());
                } else {
                    SetButtonStates(true);
                }
            } catch (Exception ex) {
                MessageBoxCreator.ShowError(ex.Message, "Save Settings Error");
                SetButtonStates(true);
            }
        }

        private async void GetSettingsButton_Click(object sender, RoutedEventArgs e) {
            try {
                SetButtonStates(false);
                ClearSettings();
                await Task.Run(() => UpdateSettingsTable());
            } catch (Exception ex) {
                MessageBoxCreator.ShowError(ex.Message, "Get Settings Error");
                SetButtonStates(true);
            }
        }

        private void PrinterSettingsGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e) {
            if (viewModel.Settings.Count > 0) {
                TextBox textBox = e.EditingElement as TextBox;
                string newValue = textBox.Text.ToString();

                Setting setting = e.Row.Item as Setting;
                string key = setting.Key;
                if (key != null) {
                    if (viewModel.ModifiedSettings.ContainsKey(key)) {
                        viewModel.ModifiedSettings[key] = newValue;
                    } else {
                        viewModel.ModifiedSettings.Add(key, newValue);
                    }
                }
            }
        }
    }
}
