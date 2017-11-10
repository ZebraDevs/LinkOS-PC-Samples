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

package com.zebra.desktop.devdemo.printerstatus;

import java.awt.BorderLayout;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;

import javax.swing.JButton;
import javax.swing.JFrame;
import javax.swing.JPanel;
import javax.swing.JScrollPane;
import javax.swing.JTextArea;
import javax.swing.SwingUtilities;

import com.zebra.desktop.devdemo.ConnectionCardPanel;
import com.zebra.desktop.devdemo.DemoDialog;
import com.zebra.sdk.comm.Connection;
import com.zebra.sdk.comm.ConnectionException;
import com.zebra.sdk.printer.PrinterStatus;
import com.zebra.sdk.printer.PrinterStatusMessages;
import com.zebra.sdk.printer.ZebraPrinter;
import com.zebra.sdk.printer.ZebraPrinterFactory;
import com.zebra.sdk.printer.ZebraPrinterLanguageUnknownException;
import com.zebra.sdk.printer.ZebraPrinterLinkOs;

public class PrinterStatusDemo extends DemoDialog {

    private static final long serialVersionUID = 4803731686290955630L;
    private ConnectionCardPanel connectionPanel;
    private JButton statusButton;
    private JTextArea statusOutput;

    public PrinterStatusDemo(JFrame owner) {
        super(owner, 350, 250);

        connectionPanel = new ConnectionCardPanel();
        statusButton = new JButton("Get Printer Status");
        statusButton.addActionListener(new ActionListener() {

            public void actionPerformed(ActionEvent e) {
                new Thread(new Runnable() {

                    public void run() {
                        enableStatusButton(false);
                        getPrinterStatus();
                        enableStatusButton(true);
                    }

                }).start();
            }

        });

        JPanel centerPanel = new JPanel();

        statusOutput = new JTextArea();
        statusOutput.setEditable(false);

        centerPanel.setLayout(new BorderLayout());

        JScrollPane outputScroller = new JScrollPane();
        outputScroller.getViewport().add(statusOutput);

        centerPanel.add(statusButton, BorderLayout.NORTH);
        centerPanel.add(outputScroller, BorderLayout.CENTER);

        this.add(connectionPanel, BorderLayout.NORTH);
        this.add(centerPanel, BorderLayout.CENTER);

    }

    private void getPrinterStatus() {
        Connection printerConnection = null;
        try {
            printerConnection = connectionPanel.getConnection();
            printerConnection.open();

            ZebraPrinter printer = ZebraPrinterFactory.getInstance(printerConnection);
            ZebraPrinterLinkOs linkOsPrinter = ZebraPrinterFactory.createLinkOsPrinter(printer);

            PrinterStatus printerStatus = (linkOsPrinter != null) ? linkOsPrinter.getCurrentStatus() : printer.getCurrentStatus();

            String[] printerStatusString = new PrinterStatusMessages(printerStatus).getStatusMessage();
            String[] printerStatusPrefix = getPrinterStatusPrefix(printerStatus);

            final StringBuilder sb = new StringBuilder();

            for (String s : printerStatusPrefix) {
                sb.append(s + "\r\n");
            }
            for (String s : printerStatusString) {
                sb.append(s + "\r\n");
            }

            SwingUtilities.invokeLater(new Runnable() {
                public void run() {
                    statusOutput.setText(sb.toString());
                    statusOutput.invalidate();
                }
            });

        } catch (ConnectionException e) {
            DemoDialog.showErrorDialog(this, e.getMessage(), "Connection error!");
        } catch (ZebraPrinterLanguageUnknownException e) {
            DemoDialog.showErrorDialog(this, e.getMessage(), "Connection error!");
        } finally {
            if (printerConnection != null)
                try {
                    printerConnection.close();
                } catch (ConnectionException e) {
                }
        }
    }

    private void enableStatusButton(final boolean b) {
        SwingUtilities.invokeLater(new Runnable() {
            public void run() {
                statusButton.setEnabled(b);
            }
        });
    }

    private String[] getPrinterStatusPrefix(PrinterStatus printerStatus) {
        boolean ready = printerStatus != null ? printerStatus.isReadyToPrint : false;
        String readyString = "Printer " + (ready ? "ready" : "not ready");
        String labelsInBatch = "Labels in batch: " + String.valueOf(printerStatus.labelsRemainingInBatch);
        String labelsInRecvBuffer = "Labels in buffer: " + String.valueOf(printerStatus.numberOfFormatsInReceiveBuffer);
        return new String[] { readyString, labelsInBatch, labelsInRecvBuffer };
    }

}
