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

using Zebra.Windows.DevDemo.Demos;

namespace Zebra.Windows.DevDemo.Demos.ImagePrint {

    class ImagePrintViewModel : IDemoViewModel {

        private bool shouldStoreImage;
        private string storedFileName;

        public bool ShouldStoreImage {
            get => shouldStoreImage;
            set {
                if (shouldStoreImage != value) {
                    shouldStoreImage = value;
                    OnPropertyChanged();
                }
            }
        }

        public string StoredFileName {
            get => storedFileName;
            set {
                if (storedFileName != value) {
                    storedFileName = value;
                    OnPropertyChanged();
                }
            }
        }

        public ImagePrintViewModel() {
            Name = "Image Print";
        }
    }
}
