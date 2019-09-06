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
using System.Drawing;
using Zebra.Sdk.Card.Containers;
using Zebra.Sdk.Card.Enumerations;
using Zebra.Sdk.Card.Graphics;

namespace SmartCardExampleCode.Zebra.Printer {

    public class Net_Graphics {

        #region Structures
        public struct GRAPHIC_CONFIG {
            public CardSide side;
            public PrintType printType;
            public int x;
            public int y;
            public int fillColor;
            public string filename;
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Builds a list of graphic information for a print operation
        /// </summary>
        /// <param name="graphicConfig">graphic configuration list</param>
        ///    contains: side, print type, location, fill color, filename
        /// <returns>graphic information list</returns>
		/// <exception cref="Exception">Create image error</exception>
        public List<GraphicsInfo> BuildGraphicsInfoList(List<GRAPHIC_CONFIG> graphicConfig) {
            List<GraphicsInfo> graphicInfoList = new List<GraphicsInfo>();
            try {
                foreach(GRAPHIC_CONFIG gc in graphicConfig) {
                    byte[] img = ConvertImage(CreateImageFromFile(gc.filename));
                    graphicInfoList.Add(AddBasicImage(gc, img));
                }
            } catch ( Exception ex ) {
                throw new Exception (ex.Message);
            }
            return graphicInfoList;
        }

        /// <summary>
        /// Builds a graphic information structure record
        /// </summary>
        /// <param name="graphicConfig">side,printType,x,y,fillColor</param>
        /// <param name="imageData">binary image</param>
        /// <returns>graphic structure</returns>
		/// <exception cref="Exception">Create image error</exception>
        private GraphicsInfo AddBasicImage(GRAPHIC_CONFIG graphicConfig, byte[] imageData) {
            return new GraphicsInfo {
                Side = graphicConfig.side,
                PrintType = graphicConfig.printType,
                GraphicType = imageData != null ? GraphicType.BMP : GraphicType.NA,
                XOffset = graphicConfig.x,
                YOffset = graphicConfig.y,
                FillColor = graphicConfig.fillColor,
                Opacity = 0,
                Overprint = false,
                GraphicData = imageData != null ? new ZebraCardImage(imageData) : null
            };
        }

        /// <summary>
        /// Creates an image from a file
        /// </summary>
        /// <param name="filename">file containing the image</param>
        /// <returns>image data</returns>
		/// <exception cref="Exception">Create image error</exception>
        private static Image CreateImageFromFile(string filename) {
            try {
                return Image.FromFile(filename);
            } catch (Exception exception) {
                throw new ArgumentException($"Could not create image object from file {filename}", exception);
            }
        }

        /// <summary>
        /// Converts image data to a byte array
        /// </summary>
        /// <param name="image">image data to convert</param>
        /// <returns>byte array contain image data</returns>
        private static byte[] ConvertImage(Image image) {
            return (byte[])new ImageConverter().ConvertTo(image, typeof(byte[]));
        }

        #endregion
    }
}
