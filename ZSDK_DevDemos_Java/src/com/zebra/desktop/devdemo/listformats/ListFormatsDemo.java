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

package com.zebra.desktop.devdemo.listformats;

import java.awt.BorderLayout;
import java.awt.Dimension;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;

import javax.swing.DefaultListModel;
import javax.swing.JButton;
import javax.swing.JFrame;
import javax.swing.JList;
import javax.swing.JOptionPane;
import javax.swing.JPanel;
import javax.swing.JScrollPane;
import javax.swing.SwingUtilities;

import com.zebra.desktop.devdemo.ConnectionCardPanel;
import com.zebra.desktop.devdemo.DemoDialog;
import com.zebra.sdk.comm.Connection;
import com.zebra.sdk.comm.ConnectionException;
import com.zebra.sdk.device.ZebraIllegalArgumentException;
import com.zebra.sdk.printer.PrinterLanguage;
import com.zebra.sdk.printer.ZebraPrinter;
import com.zebra.sdk.printer.ZebraPrinterFactory;
import com.zebra.sdk.printer.ZebraPrinterLanguageUnknownException;

public class ListFormatsDemo extends DemoDialog {

    private static final long serialVersionUID = 1052520021299713509L;

    private ConnectionCardPanel connectionPanel;
    private JList retrieveOutputArea;
    private JPanel outputDialog;
    private JPanel buttonPanel;
    private JButton formatButton;
    private JButton filesButton;

    public ListFormatsDemo(JFrame owner) {
        super(owner, 450, 400);
        this.setLayout(new BorderLayout());

        buttonPanel = makeListFilesButtonPanel();

        connectionPanel = new ConnectionCardPanel();

        outputDialog = new JPanel();

        outputDialog.add(makeOutputDialogPane());
        outputDialog.setSize(new Dimension(300, 250));
        this.add(connectionPanel, BorderLayout.NORTH);
        this.add(buttonPanel, BorderLayout.CENTER);
        this.add(outputDialog, BorderLayout.SOUTH);
    }

    private JScrollPane makeOutputDialogPane() {
        retrieveOutputArea = new JList();
        retrieveOutputArea.setModel(new DefaultListModel());

        JScrollPane scrollPane = new JScrollPane();
        scrollPane.getViewport().add(retrieveOutputArea);
        scrollPane.setPreferredSize(new Dimension(300, 250));
        return scrollPane;
    }

    private void runFormatDemo(final boolean isFormat) {
        new Thread(new Runnable() {

            public void run() {
                filesButton.setEnabled(false);
                formatButton.setEnabled(false);
                performListFiles(isFormat);
                filesButton.setEnabled(true);
                formatButton.setEnabled(true);
            }

        }).start();
    }

    private JPanel makeListFilesButtonPanel() {
        JPanel tmpPanel = new JPanel();

        formatButton = new JButton("Retrieve Formats");
        formatButton.addActionListener(new ActionListener() {
            public void actionPerformed(ActionEvent arg0) {
                runFormatDemo(true);
            }

        });

        filesButton = new JButton("Retrieve Files");
        filesButton.addActionListener(new ActionListener() {
            public void actionPerformed(ActionEvent e) {
                runFormatDemo(false);
            }

        });

        tmpPanel.add(formatButton);
        tmpPanel.add(filesButton);
        return tmpPanel;
    }

    private void performListFiles(boolean isFormat) {
        try {
            Connection printerConnection = connectionPanel.getConnection();
            printerConnection.open();
            ZebraPrinter printer = ZebraPrinterFactory.getInstance(printerConnection);
            String[] formatExtensions;
            if (printer.getPrinterControlLanguage() == PrinterLanguage.ZPL) {
                formatExtensions = new String[] { "ZPL" };
            } else {
                formatExtensions = new String[] { "FMT", "LBL" };
            }

            String[] formats = isFormat ? printer.retrieveFileNames(formatExtensions) : printer.retrieveFileNames();

            final DefaultListModel formatList = new DefaultListModel();
            for (String format : formats) {
                formatList.addElement(format);
            }

            SwingUtilities.invokeLater(new Runnable() {
                public void run() {
                    retrieveOutputArea.setModel(formatList);
                    retrieveOutputArea.invalidate();
                }
            });
            printerConnection.close();
        } catch (ConnectionException e) {
            JOptionPane.showMessageDialog(this, e.getMessage(), "Connection Error", JOptionPane.ERROR_MESSAGE);
        } catch (ZebraIllegalArgumentException e) {
            JOptionPane.showMessageDialog(this, e.getMessage(), "Connection Error", JOptionPane.ERROR_MESSAGE);
        } catch (ZebraPrinterLanguageUnknownException e) {
            JOptionPane.showMessageDialog(this, e.getMessage(), "Connection Error", JOptionPane.ERROR_MESSAGE);
        } catch (ClassCastException e) {
            JOptionPane.showMessageDialog(this, e.getMessage(), "USB Error", JOptionPane.ERROR_MESSAGE);
        }
    }
}
