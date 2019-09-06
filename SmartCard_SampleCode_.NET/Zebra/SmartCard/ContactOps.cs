/***********************************************
 * CONFIDENTIAL AND PROPRIETARY 
 * 
 * The source code and other information contained herein is the confidential and exclusive property of
 * ZIH Corp. and is subject to the terms and conditions in your end user license agreement.
 * This source code, and any other information contained herein, shall not be copied, reproduced, published, 
 * displayed or distributed, in whole or in part, in any medium, by any means, for any purpose except as
 * expressly permitted under such license agreement.
 * 
 * Copyright ZIH Corp. 2019
 * 
 * ALL RIGHTS RESERVED
 ***********************************************/

using System;

namespace SmartCardExampleCode.Zebra.SmartCard {

    internal class ContactOps : WinSCardLib, IDisposable {

        #region Properties
        public bool tagConnected { get; set; } = false;
        #endregion

        #region Dispose

        /// <summary>
        /// Class disposal
        ///    disconnects tag if still connected
        /// </summary>
        void IDisposable.Dispose() {
            if (tagConnected) {
                tagConnected = false;
                TagDisconnect();
            }
            GC.SuppressFinalize(this);
        }
        #endregion

    }
}
