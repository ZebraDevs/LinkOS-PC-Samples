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

using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Zebra.Windows.DevDemo.Demos;
using Zebra.Windows.DevDemo.Dialogs;

namespace Zebra.Windows.DevDemo {

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        public MainWindow() {
            InitializeComponent();
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private void CloseButton_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            QuitApplication();
        }

        private void ListBoxItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            if (sender is ListBoxItem item) {
                IDemoViewModel demoViewModel = item.DataContext as IDemoViewModel;
                (DataContext as MainWindowViewModel).CurrentDemoViewModel = (DataContext as MainWindowViewModel).DemoViewModels.FirstOrDefault(vm => vm == demoViewModel);
            }
        }

        private void AboutMenuItem_Click(object sender, RoutedEventArgs e) {
            AboutDialog aboutDialog = new AboutDialog();
            aboutDialog.Show();
        }

        private void QuitMenuItem_Click(object sender, RoutedEventArgs e) {
            QuitApplication();
        }

        private void QuitApplication() {
            Application.Current.Shutdown();
        }
    }
}
