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

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Zebra.Windows.DevDemo.Demos;
using Zebra.Windows.DevDemo.Demos.ConnectionBuilder;
using Zebra.Windows.DevDemo.Demos.Connectivity;
using Zebra.Windows.DevDemo.Demos.Discovery;
using Zebra.Windows.DevDemo.Demos.ImagePrint;
using Zebra.Windows.DevDemo.Demos.ListFormats;
using Zebra.Windows.DevDemo.Demos.MagCard;
using Zebra.Windows.DevDemo.Demos.PrinterStatus;
using Zebra.Windows.DevDemo.Demos.Profile;
using Zebra.Windows.DevDemo.Demos.SendFile;
using Zebra.Windows.DevDemo.Demos.Settings;
using Zebra.Windows.DevDemo.Demos.SmartCard;
using Zebra.Windows.DevDemo.Demos.StoredFormat;

namespace Zebra.Windows.DevDemo {

    class MainWindowViewModel : INotifyPropertyChanged {

        private IDemoViewModel currentDemoViewModel;
        private ObservableCollection<IDemoViewModel> demoViewModels;

        public event PropertyChangedEventHandler PropertyChanged;

        public IDemoViewModel CurrentDemoViewModel {
            get => currentDemoViewModel;
            set {
                if (currentDemoViewModel != value) {
                    currentDemoViewModel = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<IDemoViewModel> DemoViewModels {
            get {
                if (demoViewModels == null) {
                    demoViewModels = new ObservableCollection<IDemoViewModel>();
                }
                return demoViewModels;
            }
        }

        public MainWindowViewModel() {
            DemoViewModels.Add(new ConnectivityViewModel());
            DemoViewModels.Add(new DiscoveryViewModel());
            DemoViewModels.Add(new ImagePrintViewModel());
            DemoViewModels.Add(new ListFormatsViewModel());
            DemoViewModels.Add(new MagCardViewModel());
            DemoViewModels.Add(new PrinterStatusViewModel());
            DemoViewModels.Add(new ProfileViewModel());
            DemoViewModels.Add(new SendFileViewModel());
            DemoViewModels.Add(new SettingsViewModel());
            DemoViewModels.Add(new SmartCardViewModel());
            DemoViewModels.Add(new StoredFormatViewModel());
            DemoViewModels.Add(new ConnectionBuilderViewModel());

            CurrentDemoViewModel = DemoViewModels[0];
        }

        private void OnPropertyChanged([CallerMemberName] string memberName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
        }
    }
}
