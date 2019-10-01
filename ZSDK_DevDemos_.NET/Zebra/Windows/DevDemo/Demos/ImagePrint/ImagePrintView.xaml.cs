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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Zebra.Sdk.Comm;
using Zebra.Sdk.Device;
using Zebra.Sdk.Graphics;
using Zebra.Sdk.Printer;
using Zebra.Windows.DevDemo.Utils;

namespace Zebra.Windows.DevDemo.Demos.ImagePrint {

    /// <summary>
    /// Interaction logic for ImagePrintView.xaml
    /// </summary>
    public partial class ImagePrintView : UserControl {

        private ImagePrintViewModel viewModel;

        public ImagePrintView() {
            InitializeComponent();

            viewModel = DataContext as ImagePrintViewModel;
        }

        private void PrintButton_Click(object sender, RoutedEventArgs evt) {
            SetPrintButtonState(false);
            Connection printerConnection = null;
            Task.Run(() => {
                try {
                    printerConnection = connectionSelector.GetConnection();
                    printerConnection.Open();
                    ZebraImageI image = ZebraImageFactory.GetImage(fileSelector.FileName);
                    if (viewModel.ShouldStoreImage) {
                        ZebraPrinterFactory.GetInstance(printerConnection).StoreImage(viewModel.StoredFileName, image, 540, 412);
                    }
                    ZebraPrinterFactory.GetInstance(printerConnection).PrintImage(image, 0, 0, 550, 412, false);
                } catch (ConnectionException e) {
                    MessageBoxCreator.ShowError(e.Message, "Connection Error");
                } catch (ZebraPrinterLanguageUnknownException e) {
                    MessageBoxCreator.ShowError(e.Message, "Connection Error");
                } catch (IOException e) {
                    MessageBoxCreator.ShowError(e.Message, "Image Error");
                }  catch (ZebraIllegalArgumentException e) {
                    MessageBoxCreator.ShowError(e.Message, "Illegal Arguments");
                } catch (ArgumentException e) {
                    MessageBoxCreator.ShowError(e.Message, "Invalid File Path");
                } finally {
                    try {
                        if (printerConnection != null) {
                            printerConnection.Close();
                        }
                    } catch (ConnectionException) {
                    } finally {
                        SetPrintButtonState(true);
                    }
                }
            });
        }

        private void SetPrintButtonState(bool state) {
            Application.Current.Dispatcher.Invoke(() => {
                printButton.IsEnabled = state;
            });
        }
    }
}
