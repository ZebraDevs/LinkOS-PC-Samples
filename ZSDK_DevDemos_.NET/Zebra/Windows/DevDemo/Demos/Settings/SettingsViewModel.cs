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

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Zebra.Windows.DevDemo.Demos.Settings {

    class SettingsViewModel : IDemoViewModel {

        private ObservableCollection<Setting> settings = new ObservableCollection<Setting>();
        private Dictionary<string, string> modifiedSettings = new Dictionary<string, string>();

        public ObservableCollection<Setting> Settings {
            get => settings;
        }

        public Dictionary<string, string> ModifiedSettings {
            get => modifiedSettings;
        }

        public SettingsViewModel() {
            Name = "Settings";
        }
    }
}
