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

package com.zebra.desktop.devdemo.imageprint;

import java.awt.BorderLayout;
import java.awt.Dimension;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.io.File;
import java.io.IOException;

import javax.swing.JButton;
import javax.swing.JCheckBox;
import javax.swing.JFileChooser;
import javax.swing.JFrame;
import javax.swing.JPanel;
import javax.swing.JTextField;
import javax.swing.SwingUtilities;
import javax.swing.filechooser.FileFilter;

import com.zebra.desktop.devdemo.ConnectionCardPanel;
import com.zebra.desktop.devdemo.DemoDialog;
import com.zebra.sdk.comm.Connection;
import com.zebra.sdk.comm.ConnectionException;
import com.zebra.sdk.device.ZebraIllegalArgumentException;
import com.zebra.sdk.graphics.ZebraImageFactory;
import com.zebra.sdk.graphics.ZebraImageI;
import com.zebra.sdk.printer.ZebraPrinterFactory;
import com.zebra.sdk.printer.ZebraPrinterLanguageUnknownException;

public class ImagePrintDemo extends DemoDialog {

    private static final long serialVersionUID = 3576476911451727513L;

    private ConnectionCardPanel connectionPanel;
    private JTextField filePathTextField;

    public ImagePrintDemo(JFrame owner) {
        super(owner, 450, 220);

        connectionPanel = new ConnectionCardPanel();

        this.add(connectionPanel, BorderLayout.NORTH);
        this.add(createFileAndPrintPanel(), BorderLayout.CENTER);
    }

    private JPanel createFileAndPrintPanel() {
        JPanel browseFileAndPrintPanel = new JPanel(new BorderLayout());

        filePathTextField = new JTextField();
        filePathTextField.setPreferredSize(new Dimension(250, 30));

        final JTextField pathOnPrinter = new JTextField();
        pathOnPrinter.setPreferredSize(new Dimension(140, 30));
        pathOnPrinter.setEnabled(false);
        final JCheckBox storeOnPrinterCheckBox = new JCheckBox("Store on printer?");
        JPanel storeOnPrinterPanel = new JPanel();
        storeOnPrinterPanel.add(storeOnPrinterCheckBox);
        storeOnPrinterPanel.add(pathOnPrinter);

        JButton browseButton = new JButton("Browse...");
        final JButton printButton = new JButton("Print");

        final JFileChooser fileChooser = new JFileChooser();
        fileChooser.setFileFilter(new FileFilter() {

            @Override
            public String getDescription() {
                return "Image Files (*.bmp, *.jpg, *.png)";
            }

            @Override
            public boolean accept(File f) {
                String fileExt = f.getName().toLowerCase();
                return fileExt.endsWith(".bmp") || fileExt.endsWith(".png") || fileExt.endsWith(".jpg") || f.isDirectory();
            }
        });

        browseButton.addActionListener(new ActionListener() {

            public void actionPerformed(ActionEvent arg0) {
                if (fileChooser.showOpenDialog(ImagePrintDemo.this) == JFileChooser.APPROVE_OPTION) {
                    filePathTextField.setText(fileChooser.getSelectedFile().getAbsolutePath());
                }
            }
        });

        printButton.addActionListener(new ActionListener() {

            public void actionPerformed(ActionEvent e) {
                new Thread(new Runnable() {

                    public void run() {
                        SwingUtilities.invokeLater(new Runnable() {

                            public void run() {
                                printButton.setEnabled(false);
                            }
                        });
                        sendImageToPrint(storeOnPrinterCheckBox.isSelected(), pathOnPrinter.getText());

                        SwingUtilities.invokeLater(new Runnable() {

                            public void run() {
                                printButton.setEnabled(true);
                            }
                        });
                    }
                }).start();
            }
        });

        storeOnPrinterCheckBox.addActionListener(new ActionListener() {

            public void actionPerformed(ActionEvent e) {
                pathOnPrinter.setEnabled(storeOnPrinterCheckBox.isSelected());
            }
        });

        JPanel browserPanel = new JPanel();
        browserPanel.add(filePathTextField);
        browserPanel.add(browseButton);
        browseFileAndPrintPanel.add(browserPanel, BorderLayout.NORTH);

        browseFileAndPrintPanel.add(storeOnPrinterPanel, BorderLayout.CENTER);

        JPanel buttonPanel = new JPanel();
        buttonPanel.add(printButton);
        browseFileAndPrintPanel.add(buttonPanel, BorderLayout.SOUTH);

        return browseFileAndPrintPanel;
    }

    private void sendImageToPrint(boolean shouldStoreImage, String storePath) {
        File imageFile = new File(filePathTextField.getText());
        if (imageFile.exists() && imageFile.isFile()) {
            Connection printerConnection = null;
            try {
                printerConnection = connectionPanel.getConnection();
                printerConnection.open();
                ZebraImageI image = ZebraImageFactory.getImage(imageFile.getAbsolutePath());
                if (shouldStoreImage) {
                    ZebraPrinterFactory.getInstance(printerConnection).storeImage(storePath, image, 540, 412);
                }
                ZebraPrinterFactory.getInstance(printerConnection).printImage(image, 0, 0, 550, 412, false);
            } catch (ConnectionException e) {
                DemoDialog.showErrorDialog(ImagePrintDemo.this, e.getMessage(), "Connection Error!");
            } catch (ZebraPrinterLanguageUnknownException e) {
                DemoDialog.showErrorDialog(ImagePrintDemo.this, e.getMessage(), "Connection Error!");
            } catch (IOException e) {
                DemoDialog.showErrorDialog(ImagePrintDemo.this, e.getMessage(), "Image Error!");
            } catch (ZebraIllegalArgumentException e) {
                DemoDialog.showErrorDialog(ImagePrintDemo.this, e.getMessage(), "Illegal Arguments!");
            } finally {
                try {
                    if (printerConnection != null) {
                        printerConnection.close();
                    }
                } catch (ConnectionException e) {
                }
            }
        } else {
            DemoDialog.showErrorDialog(ImagePrintDemo.this, "\"" + imageFile.getAbsolutePath() + "\"" + " is not a valid path.", "Invalid File Path!");
        }
    }
}
