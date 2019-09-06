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
using System.IO;
using System.Windows.Forms;
using SmartCardExampleCode.Zebra.Printer;
using SmartCardExampleCode.Zebra.SmartCard;
using SmartCardExampleCode.Zebra.VirtualEoE;
using Zebra.Sdk.Card.Enumerations;
using SmartCardExampleCode.Zebra.Helpers;

namespace SmartCardExampleCode {

    public partial class frmMain : Form {

        #region Form Constructor

        /// <summary>
        /// Form initialization
        /// </summary>
        public frmMain() {
            InitializeComponent();
            LoadDlls();
        }

        /// <summary>
        /// On from closing
        ///     properly closes down application enviornment
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmMain_FormClosing(object sender, FormClosingEventArgs e) {
            Environment.Exit(0);
        }

        #endregion

        #region Buttons

        /// <summary>
        /// Exits the application
        /// </summary>
        private void btnExit_Click(object sender, EventArgs e) {
            Application.Exit();
        }

        /// <summary>
        /// Start USB or Encoding over Ethernet ( EoE ) example code
        /// </summary>
        private void btnRun_Click(object sender, EventArgs e) {
            VirtualUSB vUSB = null;
            if (!string.IsNullOrEmpty(this.tbIPAddress.Text) && this.cboJobType.Text.Equals("UHF") ) {
                SetMsg("UHF does not support EoE", Color.DarkRed);
            } else {
                string imagePath = this.cbImage.Checked ? Application.StartupPath + "\\Zebra.bmp" : string.Empty;
                try {
                    string smartCardType = Helper.GetSmartCardTypeFromJobType(this.cboJobType.Text);
                    using(PrinterOps prnOps = new PrinterOps(smartCardType, this.tbIPAddress.Text, this.cbImage.Checked)) {
                        SmartCardJobs job = new SmartCardJobs();
                        try {
                            if (!string.IsNullOrEmpty(this.tbIPAddress.Text)) {
                                vUSB = new VirtualUSB(this.tbIPAddress.Text);
                            }
                            SetMsg("Started " + this.cboJobType.Text + " example", Color.Navy);
                            job.Examples(this.cboJobType.Text, prnOps.printerSerialNumber);

                            // Resumes the suspended smart card job
                            //   and moves the card to the destination setting
                            prnOps.ResumeJob();

                            // Sets the card source and destination for the print job
                            if (!string.IsNullOrEmpty(imagePath)) {

                                SetMsg("Printing", Color.Navy);

                                prnOps.SetJobSource("Internal");
                                prnOps.SetJobDestination("Eject");

                                // Builds a graphics list then prints based on list data
                                List<Net_Graphics.GRAPHIC_CONFIG> gcList = new List<Net_Graphics.GRAPHIC_CONFIG>();
                                Net_Graphics.GRAPHIC_CONFIG gc = new Net_Graphics.GRAPHIC_CONFIG();
                                gc.filename = imagePath;
                                gc.fillColor = -1;
                                gc.printType = PrintType.MonoK;
                                gc.side = CardSide.Front;
                                gc.x = gc.y = 0;
                                gcList.Add(gc);
                                prnOps.Print(gcList);
                            }
                            SetMsg(job.results, Color.DarkGreen);
                        } finally {
                            if (vUSB != null) {
                                vUSB.Dispose();
                                vUSB = null;
                            }
                            job = null;
                        }
                    }
                } catch ( Exception ex) {
                    SetMsg(ex.Message, Color.DarkRed);
                }
            }
        }

        #endregion

        #region Combo Boxes

        private void cboJobType_SelectedIndexChanged(object sender, EventArgs e) {
            if (this.cboJobType.Text.Equals("UHF")) {
                this.tbIPAddress.Text = string.Empty;
            }
        }

        #endregion

        #region Labels

        /// <summary>
        /// Displays message information
        /// </summary>
        /// <param name="text">Message to display</param>
        /// <param name="color">Text color</param>
        private void SetMsg(string text, Color color) {
            this.lblMsg.Text = text;
            this.lblMsg.ForeColor = color;
            this.lblMsg.Visible = true;
            this.lblMsg.Refresh();
        }

        #endregion

        /// <summary>
        /// Copies Silex and Silex support files to the application’s working folder
        ///    file locations will be dependent path choices at SDK installation time
        /// </summary>
        private void LoadDlls() {
            string errMsg = string.Empty;
            try {
                string path = "C:/Program Files/Zebra Technologies/link_os_sdk/PC-.Net-SmartCard/v2.16.0/lib/";
                string dir = System.Environment.CurrentDirectory;
                if (!File.Exists("ZBRSXBridge.dll")) {
                    File.Copy(path + "ZBRSXBridge.dll", "ZBRSXBridge.dll");
                }
                if (!File.Exists("Zbscfgsrv.dll")) {
                    File.Copy(path + "Zbscfgsrv.dll", "Zbscfgsrv.dll");
                }
                if (!File.Exists("Sxuptp.dll")) {
                    File.Copy(path + "Sxuptp.dll", "Sxuptp.dll");
                }
            } catch {
                MessageBox.Show("Unable to load DLL files");
            }
        }
    }
}
