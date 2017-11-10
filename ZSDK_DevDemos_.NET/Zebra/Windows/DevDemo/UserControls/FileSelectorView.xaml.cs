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
using System.Windows;
using System.Windows.Controls;

namespace Zebra.Windows.DevDemo.UserControls {

    /// <summary>
    /// Interaction logic for FileSelectorView.xaml
    /// </summary>
    public partial class FileSelectorView : UserControl {

        private FileSelectorViewModel viewModel;

        public string FileName {
            get => viewModel.FileName;
        }

        public FileSelectorView() {
            InitializeComponent();

            viewModel = DataContext as FileSelectorViewModel;
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true) {
                viewModel.FileName = openFileDialog.FileName;
            }
        }
    }
}
