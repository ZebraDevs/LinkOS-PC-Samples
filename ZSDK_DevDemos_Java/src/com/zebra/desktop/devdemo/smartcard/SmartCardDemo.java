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

package com.zebra.desktop.devdemo.smartcard;

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
import com.zebra.sdk.device.SmartcardReader;
import com.zebra.sdk.device.SmartcardReaderFactory;
import com.zebra.sdk.printer.ZebraPrinter;
import com.zebra.sdk.printer.ZebraPrinterFactory;
import com.zebra.sdk.printer.ZebraPrinterLanguageUnknownException;

public class SmartCardDemo extends DemoDialog {

    private static final long serialVersionUID = 5101006682127525940L;
    private ConnectionCardPanel connectionPanel;
    private JButton smartCardSendDataButton;
    private JButton smartCardSendATRButton;
    private JTextArea smartCardReadOutput;

    public SmartCardDemo(JFrame owner) {
        super(owner, 350, 250);

        connectionPanel = new ConnectionCardPanel();
        smartCardSendDataButton = new JButton("Send Data");
        smartCardSendDataButton.addActionListener(new ActionListener() {

            public void actionPerformed(ActionEvent e) {
                runSmartCardDemo(true);
            }

        });

        smartCardSendATRButton = new JButton("Send ATR");
        smartCardSendATRButton.addActionListener(new ActionListener() {

            public void actionPerformed(ActionEvent arg0) {
                runSmartCardDemo(false);
            }
        });

        JPanel centerPanel = new JPanel();

        smartCardReadOutput = new JTextArea();
        smartCardReadOutput.setEditable(false);

        centerPanel.setLayout(new BorderLayout());

        JScrollPane outputScroller = new JScrollPane();
        outputScroller.getViewport().add(smartCardReadOutput);

        JPanel buttonPanel = new JPanel();
        buttonPanel.add(smartCardSendDataButton);
        buttonPanel.add(smartCardSendATRButton);

        centerPanel.add(buttonPanel, BorderLayout.NORTH);
        centerPanel.add(outputScroller, BorderLayout.CENTER);

        this.add(connectionPanel, BorderLayout.NORTH);
        this.add(centerPanel, BorderLayout.CENTER);
    }

    private void runSmartCardDemo(final boolean sendData) {
        new Thread(new Runnable() {

            public void run() {
                enableStatusButton(false);
                readSmartCard(sendData);
                enableStatusButton(true);
            }

        }).start();
    }

    private void readSmartCard(boolean sendData) {
        Connection connection = null;
        try {
            updateSmartCardOutput("");
            connection = connectionPanel.getConnection();
            connection.open();

            ZebraPrinter printer = ZebraPrinterFactory.getInstance(connection);
            SmartcardReader smartcardReader = SmartcardReaderFactory.create(printer);
            if (smartcardReader != null) {
                final byte[] response = sendData ? smartcardReader.doCommand("8010000008") : smartcardReader.getATR();

                updateSmartCardOutput(toHexString(response));
                smartcardReader.close();
            } else {
                DemoDialog.showErrorDialog(SmartCardDemo.this, "Printer does not have a smart card reader!", "Smart Card Error!");
            }
        } catch (ConnectionException e) {
            DemoDialog.showErrorDialog(SmartCardDemo.this, e.getMessage(), "Connection Error!");
        } catch (ZebraPrinterLanguageUnknownException e) {
            DemoDialog.showErrorDialog(SmartCardDemo.this, e.getMessage(), "Connection Error!");
        } finally {
            if (connection != null)
                try {
                    connection.close();
                } catch (ConnectionException e) {
                }
        }
    }

    private void enableStatusButton(final boolean enabled) {
        SwingUtilities.invokeLater(new Runnable() {
            public void run() {
                smartCardSendATRButton.setEnabled(enabled);
                smartCardSendDataButton.setEnabled(enabled);
            }
        });
    }

    private void updateSmartCardOutput(final String newMessage) {
        SwingUtilities.invokeLater(new Runnable() {

            public void run() {
                smartCardReadOutput.setText(newMessage);
            }
        });
    }

    private String toHexString(byte[] byteArr) {
        StringBuilder sb = new StringBuilder();
        String temp;
        for (byte b : byteArr) {
            temp = String.format("%02x", b);
            sb.append(temp);
        }
        return sb.toString();
    }

}
