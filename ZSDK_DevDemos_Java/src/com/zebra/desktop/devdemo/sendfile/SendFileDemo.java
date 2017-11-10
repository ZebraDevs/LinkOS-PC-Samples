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

package com.zebra.desktop.devdemo.sendfile;

import java.awt.BorderLayout;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;

import javax.swing.JButton;
import javax.swing.JFrame;
import javax.swing.JPanel;
import javax.swing.SwingUtilities;

import com.zebra.desktop.devdemo.ConnectionCardPanel;
import com.zebra.desktop.devdemo.DemoDialog;
import com.zebra.sdk.comm.Connection;
import com.zebra.sdk.comm.ConnectionException;
import com.zebra.sdk.printer.PrinterLanguage;
import com.zebra.sdk.printer.ZebraPrinter;
import com.zebra.sdk.printer.ZebraPrinterFactory;
import com.zebra.sdk.printer.ZebraPrinterLanguageUnknownException;

public class SendFileDemo extends DemoDialog {

    private static final long serialVersionUID = 2720301105327974543L;

    private ConnectionCardPanel connectionPanel;
    private JButton sendFileButton;

    public SendFileDemo(JFrame owner) {
        super(owner, 350, 175);

        connectionPanel = new ConnectionCardPanel();

        this.add(connectionPanel, BorderLayout.NORTH);
        this.add(sendFilePanel(), BorderLayout.SOUTH);
    }

    private JPanel sendFilePanel() {
        JPanel browseFileAndPrintPanel = new JPanel();

        sendFileButton = new JButton("Send File");

        sendFileButton.addActionListener(new ActionListener() {

            public void actionPerformed(ActionEvent arg0) {
                new Thread(new Runnable() {

                    public void run() {
                        setSendFileButtonState(false);
                        sendFileToPrinter();
                        setSendFileButtonState(true);
                    }

                }).start();
            }
        });

        browseFileAndPrintPanel.add(sendFileButton);

        return browseFileAndPrintPanel;
    }

    private void setSendFileButtonState(final boolean enabled) {
        SwingUtilities.invokeLater(new Runnable() {
            public void run() {
                sendFileButton.setEnabled(enabled);
            }
        });
    }

    private void sendFileToPrinter() {
        Connection printerConnection = null;
        try {
            printerConnection = connectionPanel.getConnection();
            printerConnection.open();
            ZebraPrinter printer = ZebraPrinterFactory.getInstance(printerConnection);
            String filePath = createDemoFile(printer.getPrinterControlLanguage());
            printer.sendFileContents(filePath);
        } catch (ConnectionException e) {
            DemoDialog.showErrorDialog(SendFileDemo.this, e.getMessage(), "Connection Error!");
        } catch (IOException e) {
            DemoDialog.showErrorDialog(SendFileDemo.this, e.getMessage(), "IO Error!");
        } catch (ZebraPrinterLanguageUnknownException e) {
            DemoDialog.showErrorDialog(SendFileDemo.this, e.getMessage(), "Connection Error!");
        } finally {
            try {
                if (printerConnection != null)
                    printerConnection.close();
            } catch (ConnectionException e) {
            }
        }
    }

    private String createDemoFile(PrinterLanguage pl) throws IOException {

        File tmpFile = File.createTempFile("TEST_ZEBRA", "LBL");
        FileOutputStream os = new FileOutputStream(tmpFile);

        byte[] configLabel = null;

        if (pl == PrinterLanguage.ZPL) {
            configLabel = "^XA^FO17,16^GB379,371,8^FS^FT65,255^A0N,135,134^FDTEST^FS^XZ".getBytes();
        } else if (pl == PrinterLanguage.CPCL) {
            String cpclConfigLabel = "! 0 200 200 406 1\r\n" + "ON-FEED IGNORE\r\n" + "BOX 20 20 380 380 8\r\n" + "T 0 6 137 177 TEST\r\n" + "PRINT\r\n";
            configLabel = cpclConfigLabel.getBytes();
        }
        os.write(configLabel);
        os.flush();
        os.close();
        return tmpFile.getAbsolutePath();
    }
}
