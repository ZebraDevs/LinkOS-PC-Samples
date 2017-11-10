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

package com.zebra.desktop.devdemo.magcard;

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
import com.zebra.sdk.device.MagCardReader;
import com.zebra.sdk.device.MagCardReaderFactory;
import com.zebra.sdk.printer.ZebraPrinter;
import com.zebra.sdk.printer.ZebraPrinterFactory;
import com.zebra.sdk.printer.ZebraPrinterLanguageUnknownException;

public class MagCardDemo extends DemoDialog {

    private static final long serialVersionUID = -7483445340398212494L;
    private ConnectionCardPanel connectionPanel;
    private JButton readMagButton;
    private JTextArea magReadOutput;

    public MagCardDemo(JFrame owner) {
        super(owner, 350, 250);

        connectionPanel = new ConnectionCardPanel();
        readMagButton = new JButton("Read Mag Card");
        readMagButton.addActionListener(new ActionListener() {

            public void actionPerformed(ActionEvent e) {
                new Thread(new Runnable() {

                    public void run() {
                        enableStatusButton(false);
                        readMagCard();
                        setButtonText("Read Mag Card");
                        enableStatusButton(true);
                    }

                }).start();
            }

        });

        JPanel centerPanel = new JPanel();

        magReadOutput = new JTextArea();
        magReadOutput.setEditable(false);

        centerPanel.setLayout(new BorderLayout());

        JScrollPane outputScroller = new JScrollPane();
        outputScroller.getViewport().add(magReadOutput);

        centerPanel.add(readMagButton, BorderLayout.NORTH);
        centerPanel.add(outputScroller, BorderLayout.CENTER);

        this.add(connectionPanel, BorderLayout.NORTH);
        this.add(centerPanel, BorderLayout.CENTER);
    }

    private void readMagCard() {
        Connection connection = null;
        try {
            connection = connectionPanel.getConnection();
            connection.open();

            ZebraPrinter printer = ZebraPrinterFactory.getInstance(connection);
            MagCardReader magCardReader = MagCardReaderFactory.create(printer);
            if (magCardReader != null) {
                setButtonText("Swipe Card Now");
                updateMagCardOutput("");
                final String[] trackData = magCardReader.read(30000);

                if (trackData[0].equals("") && trackData[1].equals("") && trackData[2].equals("")) {
                    DemoDialog.showErrorDialog(MagCardDemo.this, "Connection timed out!", "Mag Card Error!");
                } else {
                    updateMagCardOutput(trackData[0] + "\r\n" + trackData[1] + "\r\n" + trackData[2]);
                }

            } else {
                DemoDialog.showErrorDialog(MagCardDemo.this, "Printer does not have a mag card reader!", "Mag Card Error!");
            }
        } catch (ConnectionException e) {
            DemoDialog.showErrorDialog(MagCardDemo.this, e.getMessage(), "Connection Error!");
        } catch (ZebraPrinterLanguageUnknownException e) {
            DemoDialog.showErrorDialog(MagCardDemo.this, e.getMessage(), "Connection Error!");
        } finally {
            if (connection != null)
                try {
                    connection.close();
                } catch (ConnectionException e) {
                }
        }
    }

    private void setButtonText(final String buttonText) {
        SwingUtilities.invokeLater(new Runnable() {

            public void run() {
                readMagButton.setText(buttonText);
            }
        });
    }

    private void updateMagCardOutput(final String newMessage) {
        SwingUtilities.invokeLater(new Runnable() {

            public void run() {
                magReadOutput.setText(newMessage);
            }
        });
    }

    private void enableStatusButton(final boolean b) {
        SwingUtilities.invokeLater(new Runnable() {
            public void run() {
                readMagButton.setEnabled(b);
            }
        });
    }

}
