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
using System.Threading;
using SmartCardExampleCode.Zebra.Helpers;

namespace SmartCardExampleCode.Zebra.SmartCard {

    internal class ContactlessExamples {

        /// <summary>
        /// MIFARE example code ( 1K, 4K or Ultralight )
        /// </summary>
		/// <exception cref="Exception">Example code error</exception>
        internal void Mifare1Kor4KExample() {

            try {
                using ( ReaderLib reader = new ReaderLib()) {
                    reader.DiscoverSmartCardReaders();
                    reader.SetRFHF(true);
                    Mifare1Kor4KMemoryTest(reader.tagSlot);
                }
            } catch (Exception ex) {
                throw new Exception("MIFARE Example: " + ex.Message);
            }
        }

        /// <summary>
        /// Memory test for MIFARE 1K, 4K or Ultralight cards
        /// </summary>
        /// <param name="tagSlot">Elatec slot for tag APDU commands</param>
        private void Mifare1Kor4KMemoryTest(string tagSlot) {
            CardATR cardATR = new CardATR();
            try {
                using (ContactlessOps sc = new ContactlessOps()) {
                    sc.tagConnected = false;
                    byte[] atrBuf = null;
                    if(!sc.TagConnect(tagSlot, out atrBuf)) {
                        throw new Exception(sc.winSCardError);
                    }
                    sc.tagConnected = true;
                    if (atrBuf == null || atrBuf.Length.Equals(0)) {
                        throw new Exception ("No card ATR data");
                    }

                    string cardType = cardATR.GetCardTypeFromATR(atrBuf);
                    if (string.IsNullOrEmpty(cardType)) {
                        throw new Exception("Card ATR not found");
                    } else if ( !cardType.Equals("MIFARE_ULTRALIGHT") && !cardType.Equals("MIFARE_1K") && !cardType.Equals("MIFARE_4K")) {
                        throw new Exception("Not a MIFARE 1K or MIFARE 4K or MIFARE ULTRALIGHT card");
                    }

                    int block = 4;
                    int dataSize = cardType.Equals("MIFARE_ULTRALIGHT") ? 4 : 16;
                    char keyType = 'A';

                    if (!cardType.Equals("MIFARE_ULTRALIGHT")) {
                        byte keyNumber = 0x01;
                        byte[] key = { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
                        sc.MifareLoadKey(keyNumber, key);
                        Thread.Sleep(500);
                        sc.MifareAuthenticate(block, keyType, keyNumber);
                    }

                    byte[] dataIn = new byte[dataSize];
                    byte seed = (byte)Helper.GetRandomNumber(0,0x10);
                    for(int i = 0; i < dataSize; i++) {
                        dataIn[i] = seed++;
                    }
                    sc.MifareBlockWrite(block, dataIn);

                    byte[]dataOut = new byte[dataSize];
                    sc.MifareBlockRead(block, out dataOut);
                    for(int i = 0; i < dataSize; i++) {
                        if (dataIn[i] != dataOut[i]) {
                            throw new Exception("Data written does not equal data read");
                        }
                    }
                }
            } catch (Exception ex) {
                throw new Exception(ex.Message);
            } finally {
                cardATR = null;
            }
        }

        /// <summary>
        /// Prox example code
        /// </summary>
		/// <exception cref="Exception">Example code error</exception>
        internal void ProxExample() {

            try {
                using ( ReaderLib reader = new ReaderLib()) {
                    reader.DiscoverSmartCardReaders();
                    reader.SetRFLF(true);
                    ProxTest(reader.tagSlot);
                }
            } catch (Exception ex) {
                throw new Exception("Prox Example: " + ex.Message);
            }
        }

        /// <summary>
        /// Prox cards
        /// </summary>
        /// <param name="tagSlot">Elatec slot for tag APDU commands</param>
        private void ProxTest(string tagSlot) {
            CardATR cardATR = new CardATR();
            try {
                using (ContactlessOps sc = new ContactlessOps()) {
                    sc.tagConnected = false;
                    byte[] atrBuf = null;
                    if(!sc.TagConnect(tagSlot, out atrBuf)) {
                        throw new Exception(sc.winSCardError);
                    }
                    sc.tagConnected = true;
                    if (atrBuf == null || atrBuf.Length.Equals(0)) {
                        throw new Exception ("No ATR data");
                    }

                    // Parse the ATR
                    byte proxFormat = 0x00;
                    byte[] cn = null;
                    cardATR.GetProxFormatAndCnFromATR(atrBuf, out proxFormat, out cn);
                    if ( proxFormat == null || proxFormat.Equals(0) ) {
                        throw new Exception("No Prox Format data returned");
                    }

                    // Request card ID
                    byte[] apdu = new byte[] { 0xFF, 0xCA, 0x00, 0x00, 0x00 };
                    byte[] resp = null;
                    sc.TransmitAndReceive(apdu, out resp);
                    if (resp == null || resp.Length.Equals(0)) {
                        throw new Exception ("No card ID returned");
                    }

                }
            } catch (Exception ex) {
                throw new Exception(ex.Message);
            } finally {
                cardATR = null;
            }
        }
    }
}
