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

package com.zebra.desktop.devdemo.profile;

import java.awt.BorderLayout;
import java.awt.Dimension;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.io.File;
import java.io.IOException;

import javax.swing.JButton;
import javax.swing.JFileChooser;
import javax.swing.JFrame;
import javax.swing.JOptionPane;
import javax.swing.JPanel;
import javax.swing.JTextField;
import javax.swing.SwingUtilities;
import javax.swing.filechooser.FileFilter;

import com.zebra.desktop.devdemo.ConnectionCardPanel;
import com.zebra.desktop.devdemo.DemoDialog;
import com.zebra.sdk.comm.Connection;
import com.zebra.sdk.comm.ConnectionException;
import com.zebra.sdk.device.ZebraIllegalArgumentException;
import com.zebra.sdk.printer.FileDeletionOption;
import com.zebra.sdk.printer.ZebraPrinter;
import com.zebra.sdk.printer.ZebraPrinterFactory;
import com.zebra.sdk.printer.ZebraPrinterLanguageUnknownException;
import com.zebra.sdk.printer.ZebraPrinterLinkOs;

public class ProfileDemo extends DemoDialog {

    private static final long serialVersionUID = 3660495670172503627L;

    private ConnectionCardPanel connectionPanel;
    private JTextField filePathTextField;
    private JButton createButton;
    private JButton uploadButton;

    public ProfileDemo(JFrame owner) {
        super(owner, 450, 220);

        connectionPanel = new ConnectionCardPanel();

        this.add(connectionPanel, BorderLayout.NORTH);
        this.add(createTopLevelPanel(), BorderLayout.CENTER);
    }

    private JPanel createTopLevelPanel() {
        JPanel topLevelPanel = new JPanel(new BorderLayout());

        filePathTextField = new JTextField();
        filePathTextField.setPreferredSize(new Dimension(250, 30));

        JButton browseButton = new JButton("Browse...");
        createButton = new JButton("Create Profile");
        uploadButton = new JButton("Upload Profile");

        final JFileChooser fileChooser = new JFileChooser();
        fileChooser.setFileFilter(new FileFilter() {

            @Override
            public String getDescription() {
                return "Profile Files";
            }

            @Override
            public boolean accept(File f) {
                String fileExt = f.getName().toLowerCase();
                return fileExt.endsWith(".zprofile") || (false == fileExt.contains("."));
            }
        });

        browseButton.addActionListener(new ActionListener() {

            public void actionPerformed(ActionEvent arg0) {
                if (fileChooser.showOpenDialog(ProfileDemo.this) == JFileChooser.APPROVE_OPTION) {
                    filePathTextField.setText(fileChooser.getSelectedFile().getAbsolutePath());
                }
            }
        });

        uploadButton.addActionListener(new ActionListener() {
            public void actionPerformed(ActionEvent e) {
                new Thread(new Runnable() {

                    public void run() {
                        setButtonState(uploadButton, false);
                        final String profilePath = filePathTextField.getText();
                        try {
                            final Connection connection = connectionPanel.getConnection();
                            uploadProfile(profilePath, connection);
                        } catch (ConnectionException e) {
                            DemoDialog.showErrorDialog(ProfileDemo.this, e.getMessage(), "Connection Error!");
                        }
                        setButtonState(uploadButton, true);
                    }
                }).start();
            }
        });

        createButton.addActionListener(new ActionListener() {
            public void actionPerformed(ActionEvent e) {
                new Thread(new Runnable() {

                    public void run() {
                        setButtonState(createButton, false);
                        final String profilePath = filePathTextField.getText();
                        try {
                            final Connection connection = connectionPanel.getConnection();
                            createProfile(profilePath, connection);
                        } catch (ConnectionException e) {
                            DemoDialog.showErrorDialog(ProfileDemo.this, e.getMessage(), "Connection Error!");
                        }
                        setButtonState(createButton, true);
                    }
                }).start();
            }
        });

        JPanel browserPanel = new JPanel();
        browserPanel.add(filePathTextField);
        browserPanel.add(browseButton);
        topLevelPanel.add(browserPanel, BorderLayout.NORTH);

        JPanel buttonPanel = new JPanel();
        buttonPanel.add(createButton);
        buttonPanel.add(uploadButton);
        topLevelPanel.add(buttonPanel, BorderLayout.SOUTH);

        return topLevelPanel;
    }

    private void setButtonState(final JButton button, final boolean enabled) {
        SwingUtilities.invokeLater(new Runnable() {

            public void run() {
                button.setEnabled(enabled);
            }
        });
    }

    private void createProfile(final String profilePath, final Connection connection) {
        if (connection != null)
            try {
                connection.open();
                ZebraPrinter genericPrinter = ZebraPrinterFactory.getInstance(connection);
                ZebraPrinterLinkOs printer = ZebraPrinterFactory.createLinkOsPrinter(genericPrinter);
                if (printer != null) {
                    printer.createProfile(profilePath);
                    JOptionPane.showMessageDialog(ProfileDemo.this, "Profile created successfully at location '" + profilePath + "'", "Profile Created Successfully", JOptionPane.INFORMATION_MESSAGE);
                } else {
                    DemoDialog.showErrorDialog(this, "Profile creation is only available on Link-OS(TM) printers.", "Error");
                }
            } catch (ConnectionException e) {
                DemoDialog.showErrorDialog(this, e.getMessage(), "Connection error!");
            } catch (ZebraPrinterLanguageUnknownException e) {
                DemoDialog.showErrorDialog(this, e.getMessage(), "Connection error!");
            } catch (IOException e) {
                DemoDialog.showErrorDialog(this, e.getMessage(), "Connection error!");
            } catch (ZebraIllegalArgumentException e) {
                DemoDialog.showErrorDialog(this, e.getMessage(), "Connection error!");
            } finally {
                try {
                    connection.close();
                } catch (ConnectionException e) {
                }
            }
    }

    private void uploadProfile(final String profilePath, final Connection connection) {
        if (connection != null)
            try {
                connection.open();
                ZebraPrinter genericPrinter = ZebraPrinterFactory.getInstance(connection);
                ZebraPrinterLinkOs printer = ZebraPrinterFactory.createLinkOsPrinter(genericPrinter);
                if (printer != null) {
                    printer.loadProfile(profilePath, FileDeletionOption.NONE, false);
                    JOptionPane.showMessageDialog(ProfileDemo.this, "Profile loaded successfully to printer " + printer.toString(), "Profile Created Successfully", JOptionPane.INFORMATION_MESSAGE);
                } else {
                    DemoDialog.showErrorDialog(this, "Profile loading is only available on Link-OS(TM) printers.", "Error");
                }
            } catch (ConnectionException e) {
                DemoDialog.showErrorDialog(this, e.getMessage(), "Connection error!");
            } catch (ZebraPrinterLanguageUnknownException e) {
                DemoDialog.showErrorDialog(this, e.getMessage(), "Connection error!");
            } catch (IOException e) {
                DemoDialog.showErrorDialog(this, e.getMessage(), "Connection error!");
            } finally {
                try {
                    connection.close();
                } catch (ConnectionException e) {
                }
            }
    }

}
