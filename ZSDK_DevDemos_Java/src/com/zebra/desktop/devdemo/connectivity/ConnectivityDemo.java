/***********************************************
 * CONFIDENTIAL AND PROPRIETARY 
 * 
 * The source code and other information contained herein is the confidential and the exclusive property of
 * ZIH Corp. and is subject to the terms and conditions in your end user license agreement.
 * This source code, and any other information contained herein, shall not be copied, reproduced, published, 
 * displayed or distributed, in whole or in part, in any medium, by any means, for any purpose except as
 * expressly permitted under such license agreement.
 * 
 * Copyright ZIH Corp. 2012
 * 
 * ALL RIGHTS RESERVED
 ***********************************************/

package com.zebra.desktop.devdemo.connectivity;

import java.awt.BorderLayout;
import java.awt.Color;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;

import javax.swing.JButton;
import javax.swing.JFrame;
import javax.swing.JLabel;
import javax.swing.JPanel;
import javax.swing.SwingUtilities;

import com.zebra.desktop.devdemo.ConnectionCardPanel;
import com.zebra.desktop.devdemo.DemoDialog;
import com.zebra.desktop.devdemo.util.DemoSleeper;
import com.zebra.sdk.comm.Connection;
import com.zebra.sdk.comm.ConnectionException;
import com.zebra.sdk.comm.DriverPrinterConnection;
import com.zebra.sdk.printer.PrinterLanguage;
import com.zebra.sdk.printer.ZebraPrinterFactory;
import com.zebra.sdk.printer.ZebraPrinterLanguageUnknownException;
import com.zebra.sdk.printer.discovery.DiscoveredPrinterDriver;

public class ConnectivityDemo extends DemoDialog {

    private static final long serialVersionUID = -6985821697340215927L;

    private JPanel statusBarPanel;
    private JLabel statusLabel;
    private JButton testButton;
    private ConnectionCardPanel connectionPanel;

    public ConnectivityDemo(JFrame owner) {
        super(owner, 450, 200);

        this.setLayout(new BorderLayout());

        statusBarPanel = new JPanel();
        statusBarPanel.setBackground(Color.RED);
        statusLabel = new JLabel("Not Connected");
        statusLabel.setForeground(Color.GRAY);
        statusBarPanel.add(statusLabel);

        connectionPanel = new ConnectionCardPanel();
        JPanel testButtonPanel = new JPanel();

        testButton = new JButton("Test");
        testButton.addActionListener(new ActionListener() {

            public void actionPerformed(ActionEvent arg0) {
                new Thread(new Runnable() {

                    public void run() {
                        testButton.setEnabled(false);
                        performConnectionTest();
                        testButton.setEnabled(true);

                    }
                }).start();
            }

        });

        testButtonPanel.add(testButton);
        this.add(statusBarPanel, BorderLayout.NORTH);
        this.add(connectionPanel, BorderLayout.CENTER);
        this.add(testButtonPanel, BorderLayout.SOUTH);

    }

    private void performConnectionTest() {
        Connection printerConnection;
        if (connectionPanel.comboBox.getSelectedIndex() == 0) {
            try {
                printerConnection = connectionPanel.getConnection();
            } catch (ConnectionException e) {
                updateStatusBarOnGui("Invalid Address and/or Port...", Color.RED);
                DemoSleeper.sleep(1000);
                updateStatusBarOnGui("Not Connected", Color.RED);
                return;
            }
        } else {
            if (connectionPanel.usbPrinterList.getSelectedItem() instanceof DiscoveredPrinterDriver) {
                DiscoveredPrinterDriver printer = (DiscoveredPrinterDriver) connectionPanel.usbPrinterList.getSelectedItem();
                try {
                    printerConnection = new DriverPrinterConnection(printer.printerName);
                } catch (ConnectionException e) {
                    return;
                }
            } else {
                return;
            }
        }
        try {
            updateStatusBarOnGui("Connecting...", Color.YELLOW);
            DemoSleeper.sleep(1500);

            printerConnection.open();
            updateStatusBarOnGui("Connected", Color.GREEN);
            DemoSleeper.sleep(1500);

            updateStatusBarOnGui("Determining Printer Language...", Color.YELLOW);
            DemoSleeper.sleep(1500);

            PrinterLanguage printerLanguage = ZebraPrinterFactory.getInstance(printerConnection).getPrinterControlLanguage();

            updateStatusBarOnGui("Printer Language " + printerLanguage.toString(), Color.BLUE);

            DemoSleeper.sleep(1500);
            updateStatusBarOnGui("Sending Data...", Color.BLUE);

            printerConnection.write(getConfigLabel(printerLanguage));

        } catch (ConnectionException e) {
            updateStatusBarOnGui("Communications Error", Color.RED);
        } catch (ZebraPrinterLanguageUnknownException e) {
            updateStatusBarOnGui("Invalid Printer Language", Color.RED);
        } finally {
            try {
                DemoSleeper.sleep(1000);
                updateStatusBarOnGui("Disconnecting...", Color.RED);
                if (printerConnection != null)
                    printerConnection.close();
                DemoSleeper.sleep(1000);
                updateStatusBarOnGui("Not Connected", Color.RED);
            } catch (ConnectionException e) {
            }
        }
    }

    private void updateStatusBarOnGui(final String statusMessage, final Color color) {
        SwingUtilities.invokeLater(new Runnable() {
            public void run() {
                statusBarPanel.setBackground(color);
                statusLabel.setText(statusMessage);

            }
        });
    }

    /*
     * Returns the command for a test label depending on the printer control language
     * The test label is a box with the word "TEST" inside of it
     * 
     * _________________________
     * |                       |
     * |                       |
     * |        TEST           |
     * |                       |
     * |                       |
     * |_______________________|
     * 
     * 
     */
    private byte[] getConfigLabel(PrinterLanguage printerLanguage) {

        byte[] configLabel = null;
        if (printerLanguage == PrinterLanguage.ZPL) {
            configLabel = "^XA^FO17,16^GB379,371,8^FS^FT65,255^A0N,135,134^FDTEST^FS^XZ".getBytes();
        } else if (printerLanguage == PrinterLanguage.CPCL) {
            String cpclConfigLabel = "! 0 200 200 406 1\r\n" + "ON-FEED IGNORE\r\n" + "BOX 20 20 380 380 8\r\n" + "T 0 6 137 177 TEST\r\n" + "PRINT\r\n";
            configLabel = cpclConfigLabel.getBytes();
        }
        return configLabel;
    }

}
