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

namespace SmartCardExampleCode.Zebra.Helpers {

    static class Helper {

        /// <summary>
        /// Creates a random number between min and max
        /// </summary>
        /// <param name="min">minimum value</param>
        /// <param name="max">maximum value</param>
        /// <returns>created random number</returns>
        public static int GetRandomNumber(int min, int max) {
            Random r = new Random();
            return r.Next(min, max);
        }

        /// <summary>
        /// Gets smart card type based on job type
        /// </summary>
        /// <param name="jobType">smart card example job type</param>
        /// <returns>lf, hf or contact</returns>
        public static string GetSmartCardTypeFromJobType( string jobType) {
            switch (jobType) {
                case "MIFARE":
                    return "hf";
                case "PROX":
                    return "lf";
                case "ATMEL":
                    return "contact";
                case "UHF":
                    return "uhf";
                default:
                    throw new Exception("Invalid Job Type");
            }
        }

        /// <summary>
        /// Validates job type for example jobs
        /// </summary>
        /// <param name="jobType">job type to evaluate</param>
        /// <returns>true if valid</returns>
        public static bool ValidJob(string jobType) {
            return jobType.Equals("MIFARE") || jobType.Equals("PROX") || jobType.Equals("ATMEL") || jobType.Equals("UHF");
        }

    }
}
