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
using System.Collections.Generic;
using System.Linq;

namespace SmartCardExampleCode.Zebra.SmartCard
{
    public class CardATR {

        #region Supported Card ATR History Data
        private byte[] aMifare1K = new byte[] { 0x80, 0x4F, 0x0C, 0xA0, 0x00, 0x00, 0x03, 0x06, 0x03, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00 };
        private byte[] aMifare4K = new byte[] { 0x80, 0x4F, 0x0C, 0xA0, 0x00, 0x00, 0x03, 0x06, 0x03, 0x00, 0x02, 0x00, 0x00, 0x00, 0x00 };
        private byte[] aMifareUltralight = new byte[] { 0x80, 0x4F, 0x0C, 0xA0, 0x00, 0x00, 0x03, 0x06, 0x03, 0x00, 0x03, 0x00, 0x00, 0x00, 0x00 };
        private byte[] aAt88Sc0104C = new byte[] { 0x3B, 0xB2, 0x11, 0x00, 0x10, 0x80, 0x00, 0x01 };

        #endregion

        #region Declarations
        private Dictionary<string, byte[]> dictFullATR = new Dictionary<string, byte[]>();       
        private Dictionary<string, byte[]> dictHistoryData = new Dictionary<string, byte[]>();
        #endregion

        #region Class Initialization

        /// <summary>
        /// Class Initialization
        /// </summary>
        public CardATR() {
            dictHistoryData.Add("MIFARE_1K", aMifare1K);
            dictHistoryData.Add("MIFARE_4K", aMifare4K);
            dictHistoryData.Add("MIFARE_ULTRALIGHT", aMifareUltralight);
            dictFullATR.Add("AT88SC0104C", aAt88Sc0104C);
        }

        #endregion

        #region From ATR

        /// <summary>
        /// Checks to see if the ATR is a supported Card Type
        /// </summary>
        /// <param name="bAtr">ATR to compare</param>
        /// <param name="cardType">returns supported card typs; else returns ATR NOT FOUND</param>
        public string GetCardTypeFromATR(byte[] bAtr) {
            string cardType = string.Empty;
            try {
                byte[] historyData = GetATRHistoryData(bAtr);
                foreach (var item in this.dictHistoryData) {
                    if (historyData.SequenceEqual(item.Value)) {
                        cardType = item.Key;
                        break;
                    }
                }
                if (string.IsNullOrEmpty(cardType)) {
                    foreach (var item in this.dictFullATR) {
                        if (bAtr.SequenceEqual(item.Value)) {
                            cardType = item.Key;
                            break;
                        }
                    }
                }
                if (string.IsNullOrEmpty(cardType)) {
                    throw new Exception ("Card ATR not supported");
                }
            } catch {
            }
            return cardType;
        }

        /// <summary>
        /// Gets a prox card's format and cn value from an ATR
        /// </summary>
        /// <param name="bATR">ATR to parse</param>
        /// <param name="proxFormat">prox format value</param>
        /// <param name="cn">prox cn data</param>
		/// <exception cref="Exception">Get format and CN failed.</exception>
        public void GetProxFormatAndCnFromATR(byte[] bATR, out byte proxFormat, out byte[] cn) {
            proxFormat = 0xFF;
            cn = null;
            int length = bATR.Length;
            if (bATR != null && length > 2) {
                if (bATR[0].Equals(0x3B) && bATR[1].Equals(0x06)) {
                    proxFormat = bATR[2];
                    if (length > 3) {
                        cn = new byte[length - 3];
                        Array.Copy(bATR, 3, cn, 0, length-3);
                    }
                }
            }
        }

        #endregion

        #region ATR History Data

        private byte[] GetATRHistoryData(byte[] atr) {
            byte[] historyBytes = null;
            if (atr[0].Equals(0x3B)) {
                int ptr = 1;
                int offset = 0;
                bool isFD = false;
                NextField(atr[ptr], out offset, out isFD);
                ptr += offset;
                if (isFD)
                    offset++;
                if (isFD) {
                    while(true) {
                        NextField(atr[ptr], out offset, out isFD );
                        if (!isFD)
                            break;
                        ptr += offset + 1;
                    }
                }
                int historySize = atr[1] & 0x0F;
                ptr++;
                historyBytes = new byte[historySize];
                Array.Copy(atr, ptr, historyBytes, 0, historySize);
            }
            return historyBytes;
        }

        private void NextField(byte atrByte, out int offset, out bool isFD) {
            offset = 0;
            isFD = false;
            if ((atrByte & 0x10) != 0) offset++;
            if ((atrByte & 0x20) != 0) offset++;
            if ((atrByte & 0x40) != 0) offset++;
            if ((atrByte & 0x80) != 0) {
                isFD = true;
            }
        }
        #endregion
    }
}
