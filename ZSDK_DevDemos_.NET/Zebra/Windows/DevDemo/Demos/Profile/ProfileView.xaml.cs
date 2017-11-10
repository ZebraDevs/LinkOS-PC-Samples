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

using Microsoft.Win32;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Zebra.Sdk.Comm;
using Zebra.Sdk.Device;
using Zebra.Sdk.Printer;
using Zebra.Windows.DevDemo.Utils;

namespace Zebra.Windows.DevDemo.Demos.Profile {

    /// <summary>
    /// Interaction logic for ProfileView.xaml
    /// </summary>
    public partial class ProfileView : UserControl {

        public ProfileView() {
            InitializeComponent();
        }

        private void CreateProfile(string profilePath, Connection connection) {
            if (connection != null) {
                try {
                    connection.Open();
                    ZebraPrinter genericPrinter = ZebraPrinterFactory.GetInstance(connection);
                    ZebraPrinterLinkOs printer = ZebraPrinterFactory.CreateLinkOsPrinter(genericPrinter);

                    if (printer != null) {
                        if (!profilePath.EndsWith(".zprofile")) {
                            profilePath += ".zprofile";
                        }
                        printer.CreateProfile(profilePath);
                        MessageBoxCreator.ShowInformation($"Profile created successfully at location '{profilePath}'", "Profile Created Successfully");
                    } else {
                        MessageBoxCreator.ShowError("Profile creation is only available on Link-OS(TM) printers", "Error");
                    }
                } catch (ConnectionException e) {
                    MessageBoxCreator.ShowError(e.Message, "Connection Error");
                } catch (ZebraPrinterLanguageUnknownException e) {
                    MessageBoxCreator.ShowError(e.Message, "Connection Error");
                } catch (IOException e) {
                    MessageBoxCreator.ShowError(e.Message, "Connection Error");
                } catch (ZebraIllegalArgumentException e) {
                    MessageBoxCreator.ShowError(e.Message, "Connection Error");
                } catch (Exception e) {
                    MessageBoxCreator.ShowError(e.Message, "Create Profile Error");
                } finally {
                    SetButtonStates(true);
                    try {
                        connection.Close();
                    } catch (ConnectionException) { }
                }
            }
        }

        private void UploadProfile(string profilePath, Connection connection) {
            if (connection != null) {
                try {
                    connection.Open();
                    ZebraPrinter genericPrinter = ZebraPrinterFactory.GetInstance(connection);
                    ZebraPrinterLinkOs printer = ZebraPrinterFactory.CreateLinkOsPrinter(genericPrinter);

                    if (printer != null) {
                        printer.LoadProfile(profilePath, FileDeletionOption.NONE, false);
                        string printerAddress = connectionSelector.getConnectionAddress();
                        MessageBoxCreator.ShowInformation($"Profile loaded successfully to printer {printerAddress}", "Profile Uploaded Successfully");
                    } else {
                        MessageBoxCreator.ShowError("Profile loading is only available on Link-OS(TM) printers", "Error");
                    }
                } catch (ConnectionException e) {
                    MessageBoxCreator.ShowError(e.Message, "Connection Error");
                } catch (ZebraPrinterLanguageUnknownException e) {
                    MessageBoxCreator.ShowError(e.Message, "Connection Error");
                } catch (IOException e) {
                    MessageBoxCreator.ShowError(e.Message, "Connection Error");
                } catch (Exception e) {
                    MessageBoxCreator.ShowError(e.Message, "Upload Profile Error");
                } finally {
                    SetButtonStates(true);
                    try {
                        connection.Close();
                    } catch (ConnectionException) { }
                }
            }
        }

        private void SetButtonStates(bool enabled) {
            Application.Current.Dispatcher.Invoke(() => {
                createProfileButton.IsEnabled = enabled;
                uploadProfileButton.IsEnabled = enabled;
            });
        }

        private async void CreateProfileButton_Click(object sender, RoutedEventArgs e) {
            try {
                SetButtonStates(false);
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                if (saveFileDialog.ShowDialog() == true) {
                    string profilePath = saveFileDialog.FileName;
                    Connection connection = connectionSelector.GetConnection();
                    await Task.Run(() => CreateProfile(profilePath, connection));
                } else {
                    SetButtonStates(true);
                }
            } catch (Exception ex) {
                MessageBoxCreator.ShowError(ex.Message, "Create Profile Error");
                SetButtonStates(true);
            }
        }

        private async void UploadProfileButton_Click(object sender, RoutedEventArgs e) {
            try {
                SetButtonStates(false);
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == true) {
                    string profilePath = openFileDialog.FileName;
                    Connection connection = connectionSelector.GetConnection();
                    await Task.Run(() => UploadProfile(profilePath, connection));
                } else {
                    SetButtonStates(true);
                }
            } catch (Exception ex) {
                MessageBoxCreator.ShowError(ex.Message, "Upload Profile Error");
                SetButtonStates(true);
            }
        }
    }
}
