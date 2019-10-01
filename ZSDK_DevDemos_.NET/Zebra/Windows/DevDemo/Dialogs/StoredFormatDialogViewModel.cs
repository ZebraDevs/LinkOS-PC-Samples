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
using Zebra.Windows.DevDemo.Demos.StoredFormat;

namespace Zebra.Windows.DevDemo.Dialogs {

    public class StoredFormatDialogViewModel : INotifyPropertyChanged {

        private ObservableCollection<FormatVariable> formatVariables = new ObservableCollection<FormatVariable>();

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<FormatVariable> FormatVariables {
            get => formatVariables;
        }

        public StoredFormatDialogViewModel(ObservableCollection<FormatVariable> formatVariables) {
            this.formatVariables = formatVariables;
        }

        private void OnPropertyChanged([CallerMemberName] string memberName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
        }
    }
}
