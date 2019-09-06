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
using ZXPSCReader;
using ZXPSCReader.ZXPUHF;
using SmartCardExampleCode.Zebra.Helpers;

namespace SmartCardExampleCode.Zebra.SmartCard {

    internal class UHFExamples {

        /// <summary>
        /// UHF Example Code
        /// </summary>
        /// <param name="serialNumber">Printer's serial number</param>
        /// <returns>Example code results</returns>
        internal string UHFExample(string serialNumber) {
            string results = string.Empty;
            try {
                UHF.IM6eReader reader = ZXPReader.ReaderFactory.GetM6eReader(serialNumber);
                try {
                    reader.Connect();
                    using (UHF.IGen2Tag tag = reader.TagFactory.GetGen2Tag()) {

                        UHF.TagData tagData = null;
                        tagData = tag.Read();
                        byte[] epcBuf = tagData.Epc;
                        byte[] epcMemDataBuf = tagData.EPCMemData;
                        string epcString = tagData.EpcString;
                        string tagProtocol = tagData.Protocol;
                        byte[] reservedMemDataBuf = tagData.RESERVEDMemData;
                        byte[] tidMemDataBuf = tagData.TIDMemData;
                        byte[] userMemDataBuf = tagData.USERMemData;

                        byte[] epcHexRead = tag.ReadEPC();
                        byte[] tidData = tag.ReadTID(4);

                        byte seed = (byte)Helper.GetRandomNumber(0, 0x10);
                        int userMemorySize = tag.GetUserMemorySize();
                        if (!userMemorySize.Equals(0)) {
                            byte[] writeUserMemory = new byte[userMemorySize];
                            for (int i = 0; i < userMemorySize; i++) {
                                writeUserMemory[i] = seed++;
                            }
                            tag.WriteUserMemory(0, writeUserMemory);
                            byte[] readUserMemory = null;
                            readUserMemory = tag.ReadUserMemory(0, userMemorySize);
                            if (readUserMemory == null || readUserMemory.Equals(0)) {
                                throw new Exception ("tag.ReadUserMemory ... no memory data returned");
                            }
                        }
                    }
                } catch (Exception ex) {
                    throw new Exception("Gen2TagTest exception: " + ex.Message);
                } finally {
                    try {
                        reader.Disconnect();
                    } catch {
                    }
                }
            } catch ( Exception ex) {
                results = ex.Message;
            } finally {
            }
            return results;
        }

        internal void Dispose() {
            GC.SuppressFinalize(this);
        }
    }
}
