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
using SmartCardExampleCode.Zebra.Helpers;
using SmartCardExampleCode.Zebra.SmartCard;

namespace SmartCardExampleCode.Zebra.Printer {

    public class SmartCardJobs {

        #region Properties
        public string results { get; set; } = string.Empty;
        #endregion

        #region Jobs

        /// <summary>
        /// Example Smart Card Jobs
        /// </summary>
        /// <param name="jobType">MIFARE, PROX, ATMEL or UHF</param>
        /// <param name="printerSerialNumber">printer serial number</param>
		/// <exception cref="Exception">Reader, tag or printer errors</exception>
        public void Examples(string jobType, string printerSerialNumber) {

            try {
                if (!Helper.ValidJob(jobType)) {
                    throw new Exception("Invalid Job");
                }

                ContactlessExamples expContactless = null;
                switch (jobType) {
                    case "MIFARE":
                        expContactless = new ContactlessExamples();
                        try {
                            expContactless.Mifare1Kor4KExample();
                            this.results = "Mifare example completed successfully";
                        } finally {
                            expContactless = null;
                        }
                        break;

                    case "PROX":
                        expContactless = new ContactlessExamples();
                        try {
                            expContactless.ProxExample();
                            this.results = "Prox example completed successfully";
                        } finally {
                            expContactless = null;
                        }
                        break;

                    case "ATMEL":
                        ContactExamples expContact = new ContactExamples();
                        try {
                            expContact.AtmelExample();
                            this.results = "ATMEL example completed successfully";
                        } finally {
                            expContact = null;
                        }
                        break;

                    case "UHF":
                        UHFExamples expUHF = new UHFExamples();
                        try {
                            expUHF.UHFExample(printerSerialNumber);
                            this.results = "UHF example completed successfully";
                        } finally {
                            expUHF.Dispose();
                            expUHF = null;
                        }
                        break;
                }
            } catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }

        #endregion
    }
}
