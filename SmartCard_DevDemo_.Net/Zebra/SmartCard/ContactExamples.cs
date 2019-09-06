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
using System.Linq;
using SmartCardExampleCode.Zebra.Helpers;

namespace SmartCardExampleCode.Zebra.SmartCard {
    
    internal class ContactExamples {

        /// <summary>
        /// Atmel example code
        /// </summary>
		/// <exception cref="Exception">Reader or tag errors</exception>
        internal void AtmelExample() {
            try {
                using ( ReaderLib reader = new ReaderLib()) {
                    reader.DiscoverSmartCardReaders();
                    MemoryTestAT88SC0104C(reader.tagSlot);
                }
            } catch (Exception ex) {
                throw new Exception ("ATMEL Example: " + ex.Message);
            }
        }

        /// <summary>
        /// Atmel AT88SC0104C Zone Test
        /// </summary>
        /// <param name="tagSlot">Elatec slot for sending APDU to tag</param>
		/// <exception cref="Exception">Reader or tag errors</exception>
        private void  MemoryTestAT88SC0104C(string tagSlot) {
            CardATR cardATR = new CardATR();
            try {
                using (ContactOps sc = new ContactOps()) {

                    // Connect to tag and get tag's ATR
                    sc.tagConnected = false;
                    byte[] atrBuf = null;
                    if(!sc.TagConnect(tagSlot, out atrBuf)) {
                        throw new Exception(sc.winSCardError);
                    }
                    sc.tagConnected = true;
                    if (atrBuf == null || atrBuf.Length.Equals(0)) {
                        throw new Exception ( "No card ATR data");
                    }

                    string cardType = cardATR.GetCardTypeFromATR(atrBuf);
                    if (!cardType.Equals("AT88SC0104C")) {
                        throw new Exception ("Not a AT88SC0104C ATMEL card");
                    }

                    // Sending a set Zone 0 APDU; set Zone always indiates a failure
                    //   even though it indicates that setting the zone failed, it was set
                    //   ignore the failure
                    byte[] respBuf = null;
                    byte[] zoneZero = new byte[] { 0x00, 0xB4, 0x03, 0x00, 0x00 };
                    try {
                        sc.TransmitAndReceive(zoneZero, out respBuf);
                    } catch { }

                    byte baseData = Convert.ToByte(Helper.GetRandomNumber(0x00, 0x80));
                    byte[] dataWrote = new byte[32];
                    for (int i = 0; i < 32; i++) {
                        dataWrote[i] = baseData++;
                    }

                    // Write data to Zone 0
                    byte[] writeZoneData = new byte[] { 0x00, 0xB0, 0x00, 0x00, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                    for (int offset = 0; offset <= 0x18; offset += 0x08) {
                        writeZoneData[3] = (byte)offset;
                        Array.Copy(dataWrote, offset, writeZoneData, 5, 8);
                        sc.TransmitAndReceive(writeZoneData, out respBuf);
                    }

                    // Read data from Zone 0
                    byte[] dataOut = new byte[10];
                    byte[] dataRead = new byte[32];
                    byte[] readZoneData = new byte[] { 0x00, 0xB2, 0x00, 0x00, 0x08 };
                    for (int offset = 0; offset <= 0x18; offset += 0x08) {
                        readZoneData[3] = (byte)offset;
                        dataOut = null;
                        sc.TransmitAndReceive(readZoneData, out dataOut);
                        Array.Copy(dataOut, 0, dataRead, offset, 8);
                    }

                    // Compare
                    if (!dataWrote.SequenceEqual(dataRead)) {
                        throw new Exception(" Zone 0 read does not equal written");
                    }
                }
            } catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }
    }
}
