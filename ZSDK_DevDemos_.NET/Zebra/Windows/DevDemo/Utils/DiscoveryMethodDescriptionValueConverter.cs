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
using System.ComponentModel;
using System.Globalization;
using System.Windows.Data;
using Zebra.Windows.DevDemo.Enums;

namespace Zebra.Windows.DevDemo.Utils {

    public class DiscoveryMethodDescriptionValueConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var type = typeof(DiscoveryMethod);
            var name = Enum.GetName(type, value);
            return ((DescriptionAttribute) Attribute.GetCustomAttribute(type.GetField(name), typeof(DescriptionAttribute))).Description;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
