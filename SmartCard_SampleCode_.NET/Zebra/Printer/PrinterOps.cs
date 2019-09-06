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
using System.Threading;
using Zebra.Sdk.Card.Containers;
using Zebra.Sdk.Card.Enumerations;
using Zebra.Sdk.Card.Job;
using Zebra.Sdk.Card.Printer;
using Zebra.Sdk.Card.Printer.Discovery;
using Zebra.Sdk.Card.Zmotif.Settings;
using Zebra.Sdk.Comm;
using Zebra.Sdk.Printer.Discovery;

namespace SmartCardExampleCode.Zebra.Printer {
    
    public class PrinterOps : IDisposable {

        #region Enumerations
        private enum POLL_TYPE {
            AT_STATION,
            DONE_OK
        };
        #endregion

        #region Printer Connections Declarations
        private Connection usbConn = null;
        private TcpConnection tcpConn = null;
        #endregion

        #region Declarations
        private bool jobStarted = false;
        private int jobID = 0;
        private string ipAddress = string.Empty;
        private DiscoveredUsbPrinter usbPrinter = null;
        private List<DiscoveredUsbPrinter> usbPrinters = null;
        private SmartCardType smartCardType = SmartCardType.None;
        private ZebraPrinterZmotif zmotifPrn = null;
        #endregion

        #region Properties
        public string printerSerialNumber { get; set; } = string.Empty;
        #endregion

        #region Class Constructor

        /// <summary>
        /// Printer Ops constructor
        ///     Ensures the printer has a smart card reader and sets the smart card channel
        ///     then start job based on jobType
        /// </summary>
        /// <param name="jobType">Type of smart card encoding</param>
        /// <param name="ipAddress">IP Address for Encoding over Ethernet</param>
        /// <param name="imagePrint">Identifies if printing is to be done after smart card encoding</param>
		/// <exception cref="Exception">Preparing a smart card job failed</exception>
        public PrinterOps(string jobType, string ipAddress, bool imagePrint) {
            try {
                // Opens connection to an Ethernet or USB printer
                if (string.IsNullOrEmpty(ipAddress)) {
                    OpenUSBConnection();
                } else {
                    OpenTcpConnection(ipAddress);
                }

                // Validates that the connected printer can do a smart card job
                IsAlarmHandling();
                HasSmartCardEncoder(jobType);

                // Sets the printer's internal smart card channel for direct USB or virtual USB ( EoE )
                //   setting is persistent 
                //   manufacturing default is USB
                SetSmartCardChannel();
                jobStarted = false;

                // If image is to be printed after smart card encoding
                //   Source: is where the card is expected at the beginning of a job
                //   Destination: is where the card is placed on ResumeJob
                //   Defaults: Source = Feeder and Destination = Eject
                if (imagePrint) {
                    SetJobSource("Feeder");
                    SetJobDestination("Hold");
                }

                // Starts a smart card job
                //   card is moved to the smart card station, then the job is suspended
                switch (jobType) {
                    case "hf":
                    case "lf":
                        StartContactlessSmartCardJob();
                        break;
                    case "contact":
                        StartContactSmartCardJob();
                        break;
                    case "uhf":
                        StartUHFSmartCardJob();
                        break;
                }
                jobStarted = true;
            } catch ( Exception ex) {
                throw new Exception("PrinterOps constructor: " + ex.Message);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Cancels an active job based on jobID
        /// </summary>
        public void CancelJob() {
            try {
                if (!this.jobID.Equals(0)) {
                    this.zmotifPrn.Cancel(this.jobID);
                }
            } catch { }
        }

        /// <summary>
        /// Ejects a card from a printer
        /// </summary>
        public void EjectCard() {
            try {
                this.zmotifPrn.EjectCard();
            } catch { }
        }

        /// <summary>
        /// Prints images
        /// </summary>
        /// <param name="gcList">list of images to print</param>
		/// <exception cref="Exception">Printing failed</exception>
        public void Print(List<Net_Graphics.GRAPHIC_CONFIG> gcList ) {
            Net_Graphics ng = new Net_Graphics();
            try {
                List<GraphicsInfo> lst = ng.BuildGraphicsInfoList(gcList);
                this.jobID = this.zmotifPrn.Print(1, lst);
                PollJob(POLL_TYPE.DONE_OK, 10);
            } catch (Exception ex) {
                throw new Exception("Print: " + ex.Message);
            } finally {
                ng = null;
            }
        }

        /// <summary>
        /// Sets card destination on ResumeJob
        /// </summary>
        /// <param name="destination">Hold or Eject</param>
        public void SetJobDestination (string destination) {
            this.zmotifPrn.SetJobSetting(ZebraCardJobSettingNames.CARD_DESTINATION,destination);
        }

        /// <summary>
        /// Sets anticipated card location for a job 
        /// </summary>
        /// <param name="src">Internal or Feeder</param>
        public void SetJobSource (string source) {
            this.zmotifPrn.SetJobSetting(ZebraCardJobSettingNames.CARD_SOURCE,source);
        }

        /// <summary>
        /// Resumes a suspended job
        /// </summary>
		/// <exception cref="Exception">Resume job failed</exception>
        public void ResumeJob() {
            try {
                if (this.jobID.Equals(0)) {
                    zmotifPrn.EjectCard();
                } else {
                    zmotifPrn.Resume();
                    this.jobStarted = false;
                }
                Thread.Sleep(1000);
                try {
                    PollJob(POLL_TYPE.DONE_OK, 30);
                } catch {
                    this.zmotifPrn.Cancel(this.jobID);
                }
            } catch ( Exception ex) {
                throw new Exception("Resume Job: " + ex.Message);
            }
        }

        #endregion

        #region Open and Close Connection

        /// <summary>
        /// Closes an open printer connection
        /// </summary>
        private void CloseConnection() {
            try {
                if (this.usbConn != null) {
                    this.usbConn.Close();
                } else if (this.tcpConn != null) {
                    this.tcpConn.Close();
                }
            } catch {
            } finally {
                this.usbConn = null;
                this.tcpConn = null;
                try {
                    if (this.zmotifPrn != null) {
                        this.zmotifPrn.Destroy();
                        this.zmotifPrn = null;
                    }
                } catch {}
            }
        }

        /// <summary>
        /// Discovers USB printers and connects to the 1st one discovered
        /// </summary>
		/// <exception cref="Exception">Connectiong to a USB printer failed</exception>
        private void OpenUSBConnection() {
            try {
                if (this.usbPrinter == null) {
                    this.DiscoverUSBPrinters();
                }
                this.usbConn = usbPrinter.GetConnection();
                if (this.usbConn == null) {
                    throw new Exception("Unable to connect to an USB printer");
                }
                this.usbConn.Open();
                this.zmotifPrn = ZebraCardPrinterFactory.GetZmotifPrinter(usbConn);
                if (this.zmotifPrn == null) {
                    throw new Exception("Unable to get an instance to an USB printer");
                }
                PrinterInfo pi = this.zmotifPrn.GetPrinterInformation();
                this.printerSerialNumber = pi.SerialNumber;
            } catch ( Exception ex) {
                this.usbConn = null;
                throw new Exception("OpenUSBConnection: " + ex.Message);
            }
        }

        /// <summary>
        /// Connects to an Ethernet printer
        /// </summary>
        /// <param name="ipAddress">Printer's Ethernet IP Address</param>
		/// <exception cref="Exception">Connection to an Ethernet printer failed</exception>
        private void OpenTcpConnection(string ipAddress) {
            this.ipAddress = ipAddress;
            try {
                this.tcpConn = new TcpConnection(this.ipAddress, 9100);
                if (this.tcpConn == null)
                    throw new Exception("Unable to connect to " + ipAddress + " printer");
                this.tcpConn.Open();
                this.zmotifPrn = ZebraCardPrinterFactory.GetZmotifPrinter(this.tcpConn);
                if (this.zmotifPrn == null)
                    throw new Exception("Unable to get an instance to an Ethernet printer");
                PrinterInfo pi = this.zmotifPrn.GetPrinterInformation();
                this.printerSerialNumber = pi.SerialNumber;
            } catch (Exception ex) {
                this.tcpConn = null;
                throw new Exception ( "OpenTcpConnection: " + ex.Message);
            }
        }

        /// <summary>
        /// USB printer discovery and selects the first discovered printer
        /// </summary>
		/// <exception cref="Exception">Discovery process failed or did not find any USB printers</exception>
        private void DiscoverUSBPrinters() {
            try {
                this.usbPrinters = UsbDiscoverer.GetZebraUsbPrinters(new ZebraCardPrinterFilter());
                if (this.usbPrinters == null || this.usbPrinters.Count.Equals(0)) {
                    throw new Exception("No printers found");
                }
                this.usbPrinter = this.usbPrinters[0];
            } catch (Exception ex) {
                throw new Exception("DiscoverUSBPrinter: " + ex.Message);
            }
        }

        #endregion

        #region Printer

        /// <summary>
        /// Indicates if a printer has a smart card encoder
        /// <param name="jobType">type of smart card reader</param>
        /// </summary>
		/// <exception cref="Exception">Printer does not have a smart card reader</exception>
        private void HasSmartCardEncoder(string smartCardType) {
            bool installed = false;
            Dictionary<string, string> smartCardInfo = this.zmotifPrn.GetSmartCardConfigurations();
            if (smartCardInfo != null && !smartCardInfo.Count.Equals(0)) {
                foreach (KeyValuePair<string,string> kvp in smartCardInfo) {
                    if (kvp.Key.Equals(smartCardType)) {
                        installed = true;
                        break;
                    }
                }
            }
            if (!installed) {
                throw new Exception("No " + smartCardType + " supporting reader");
            }
        }

        /// <summary>
        /// Determines if a printer is in alarm state
        /// </summary>
		/// <exception cref="Exception">Description of the alarm</exception>
        private void IsAlarmHandling() {
            try {
                PrinterStatusInfo psi = this.zmotifPrn.GetPrinterStatus();
                if (psi.Status.Equals("alarm_handling")) {
                    throw new Exception(psi.AlarmInfo.Description);
                }
            } catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }

        #endregion

        #region Job

        /// <summary>
        /// Polls until:
        ///    smart card at reader station or done OK for jobs
        /// </summary>
        /// <param name="pollType">AT_STATION or DONE_OK</param>
        /// <param name="loopCount">Number of loops before timeout</param>
		/// <exception cref="Exception">Timed out</exception>
        private void PollJob(POLL_TYPE pollType, int loopCount) {
            bool done = false;
            try {
                JobStatusInfo jobStatusInfo = new JobStatusInfo();
                long start = Math.Abs(Environment.TickCount);
                while (!done && loopCount > 0) {
                    jobStatusInfo = this.zmotifPrn.GetJobStatus(this.jobID);
                    if (pollType == POLL_TYPE.AT_STATION) {
                        if (jobStatusInfo.ContactlessSmartCardStatus.Contains("at_station") || jobStatusInfo.ContactSmartCardStatus.Contains("at_station")) {
                            done = true;
                            break;
                        }
                    } else if (jobStatusInfo.PrintStatus.Contains("done_ok")) {
                        done = true;
                        break;
                    }
                    if (jobStatusInfo.PrintStatus.Contains("error") || jobStatusInfo.PrintStatus.Contains("cancelled")) {
                        break;
                    } else if (jobStatusInfo.ErrorInfo.Value > 0) {
                        this.zmotifPrn.Cancel(this.jobID);
                    } else if (jobStatusInfo.PrintStatus.Contains("in_progress") && jobStatusInfo.CardPosition.Contains("feeding")) {
                        if (Math.Abs(Environment.TickCount) > start + 30000) {
                            this.zmotifPrn.Cancel(this.jobID);
                            break;
                        }
                    }
                    loopCount--;
                    if (!done) {
                        Thread.Sleep(1000);
                    }
                }
                if (!done) {
                    throw new Exception(jobStatusInfo.PrintStatus);
                }
            } catch ( Exception ex ) {
                throw new Exception("PollJob: " + ex.Message);
            }
        }

        #endregion

        #region Smart Card Job

        /// <summary>
        /// Starts a contactless smart card job
        ///    moves the card to the smart card reader and the job is suppeded
        ///    while suspended the host application has direct access to the smart card reader
        /// </summary>
		/// <exception cref="Exception">Contactless smart card job did not start successfully</exception>
        private void StartContactlessSmartCardJob() {
            try {
                HasSmartCardEncoder("hf");
                this.zmotifPrn.SetJobSetting(ZebraCardJobSettingNames.SMART_CARD_CONTACTLESS, "hf");
                this.jobID = zmotifPrn.SmartCardEncode(1);
                if (this.jobID.Equals(0)) {
                    throw new Exception("Job ID = 0");
                }
                Thread.Sleep(1000);
                PollJob(POLL_TYPE.AT_STATION, 10);
                this.smartCardType = SmartCardType.Contactless;
            } catch ( Exception ex) {
                throw new Exception("ContactlessSmartCardJob: " + ex.Message);
            }
        }

        /// <summary>
        /// Starts a contact smart card job
        ///    moves the card to the smart card reader and the job is suppeded
        ///    while suspended the host application has direct access to the smart card reader
        /// </summary>
		/// <exception cref="Exception">Contact smart card job did not start successfully</exception>
        private void StartContactSmartCardJob() {
            try {
                HasSmartCardEncoder("contact");
                this.zmotifPrn.SetJobSetting(ZebraCardJobSettingNames.SMART_CARD_CONTACT, "yes");
                this.jobID = zmotifPrn.SmartCardEncode(1);
                if (this.jobID.Equals(0))
                    throw new Exception("Job ID = 0");
                Thread.Sleep(1000);
                PollJob(POLL_TYPE.AT_STATION, 10);
                this.smartCardType = SmartCardType.Contact;
            } catch (Exception ex) {
                throw new Exception("ContactSmartCardJob: " + ex.Message);
            }
        }

        /// <summary>
        /// Starts an UHF contact smart card job
        ///    moves the card to the smart card reader and the job is suppeded
        ///    while suspended the host application has direct access to the smart card reader
        /// </summary>
		/// <exception cref="Exception">UHF smart card job did not start successfully</exception>
        private void StartUHFSmartCardJob() {
            try {
                Dictionary<string, string> smartCardInfo = zmotifPrn.GetSmartCardConfigurations();
                if (smartCardInfo == null || smartCardInfo.Count <= 0) {
                    throw new Exception("Smart Card Info is Null");
                }
                if (!HasUHFSmartCardReader(smartCardInfo)) {
                    throw new Exception("No Contact Smart Card Encoder");
                }
                this.zmotifPrn.SetJobSetting(ZebraCardJobSettingNames.SMART_CARD_CONTACTLESS, "uhf");
                this.jobID = zmotifPrn.SmartCardEncode(1);
                if (this.jobID.Equals(0)) {
                    throw new Exception("Job ID = 0");
                }
                Thread.Sleep(1000);
                PollJob(POLL_TYPE.AT_STATION, 10);
                jobStarted = true;
            } catch (Exception ex) {
                throw new Exception("StartUHFSmartCardJob: " + ex.Message);
            }
        }

        /// <summary>
        /// Sets a printer's internal smart card encoder channel
        /// </summary>
		/// <exception cref="Exception">Setting smart card channel failed</exception>
        private void SetSmartCardChannel() {
            try {
                if (this.usbConn == null && this.tcpConn == null) {
                    throw new Exception("No printer connection");
                }
                string channel = string.Empty;
                if (this.tcpConn == null) {
                    zmotifPrn.SetSetting(ZebraCardSettingNamesZmotif.INTERNAL_ENCODER_CHANNEL,"usb_2_0");
                    Thread.Sleep(500);
                    channel = zmotifPrn.GetSettingValue(ZebraCardSettingNamesZmotif.INTERNAL_ENCODER_CHANNEL);
                    if (!channel.Equals("usb_2_0")) {
                        throw new Exception("Local channel not");
                    }
                } else {
                    zmotifPrn.SetSetting(ZebraCardSettingNamesZmotif.INTERNAL_ENCODER_CHANNEL,"ethernet_10_100_1G");
                    Thread.Sleep(500);
                    channel = zmotifPrn.GetSettingValue(ZebraCardSettingNamesZmotif.INTERNAL_ENCODER_CHANNEL);
                    if (!channel.Equals("ethernet_10_100_1G")) {
                        throw new Exception("Virtual channel not");
                    }
                }
            } catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Determines if the printer has a UHF smart card reader
        /// </summary>
        /// <param name="smartCardInfo">printer's smart card information</param>
        /// <returns>true if the printer has a reader</returns>
        private static bool HasUHFSmartCardReader(Dictionary<string, string> smartCardInfo) {
            bool hasReader = false;
            foreach (string type in smartCardInfo.Keys) {
                if (!string.IsNullOrEmpty(type) && type.StartsWith("uhf")) {
                    hasReader = true;
                    break;
                }
            }
            return hasReader;
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Class disposal
        /// </summary>
        public void Dispose() {
            if (jobStarted) {
                jobStarted = false;
                ResumeJob();
            }
            CloseConnection();
        }
        
        void IDisposable.Dispose() {
            Dispose();
        }

        #endregion

    }
}
