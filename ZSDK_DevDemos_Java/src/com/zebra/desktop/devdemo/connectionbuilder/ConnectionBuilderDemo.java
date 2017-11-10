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

package com.zebra.desktop.devdemo.connectionbuilder;

import java.awt.BorderLayout;
import java.awt.Component;
import java.awt.Dimension;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.awt.event.ItemEvent;
import java.awt.event.ItemListener;
import java.awt.event.KeyAdapter;
import java.awt.event.KeyEvent;

import javax.swing.BoxLayout;
import javax.swing.JButton;
import javax.swing.JComboBox;
import javax.swing.JFrame;
import javax.swing.JLabel;
import javax.swing.JPanel;
import javax.swing.JScrollPane;
import javax.swing.JTextArea;
import javax.swing.JTextField;
import javax.swing.SwingUtilities;

import com.zebra.desktop.devdemo.DemoDialog;
import com.zebra.sdk.comm.Connection;
import com.zebra.sdk.comm.ConnectionBuilder;
import com.zebra.sdk.comm.ConnectionException;
import com.zebra.sdk.printer.PrinterStatus;
import com.zebra.sdk.printer.ZebraPrinter;
import com.zebra.sdk.printer.ZebraPrinterFactory;
import com.zebra.sdk.printer.ZebraPrinterLanguageUnknownException;
import com.zebra.sdk.printer.ZebraPrinterLinkOs;

public class ConnectionBuilderDemo extends DemoDialog {

    private static final long serialVersionUID = 8173620075890589059L;
    private JButton testConnectionStringButton;
    private JTextArea connectionBuilderLog;

    public JTextField addressTextField;
    private JComboBox connectionBuilderPrefixCombo;
    private JLabel connectionString;

    public ConnectionBuilderDemo(JFrame owner) {
        super(owner, 500, 600);

        testConnectionStringButton = new JButton("Test Connection String");
        testConnectionStringButton.addActionListener(new ActionListener() {

            public void actionPerformed(ActionEvent e) {
                new Thread(new Runnable() {

                    public void run() {
                        connectionBuilderLog.setText("Log :\n\n");
                        enableButton(false);
                        testConnectionString();
                        enableButton(true);
                    }

                }).start();
            }
        });

        addressTextField = new JTextField();
        addressTextField.setAlignmentX(Component.LEFT_ALIGNMENT);
        addressTextField.setPreferredSize(new Dimension(110, 25));
        addressTextField.addKeyListener(new KeyAdapter() {
            public void keyReleased(KeyEvent arg0) {
                connectionString.setText(getConnectionStringForSdk());
            }
        });

        connectionBuilderPrefixCombo = new JComboBox(new String[] { "", "TCP_MULTI", "TCP", "TCP_STATUS", "REMOTE_MULTI", "REMOTE", "REMOTE_STATUS" });
        if (System.getProperty("os.name").toLowerCase().contains("windows")) {
            connectionBuilderPrefixCombo.addItem("USB");
        }
        connectionBuilderPrefixCombo.setAlignmentX(Component.LEFT_ALIGNMENT);

        connectionBuilderPrefixCombo.addItemListener(new ItemListener() {
            public void itemStateChanged(ItemEvent evt) {
                connectionString.setText(getConnectionStringForSdk());
            }
        });
        JPanel connectionInfoPanel = new JPanel();
        connectionInfoPanel.setLayout(new BoxLayout(connectionInfoPanel, BoxLayout.Y_AXIS));

        JLabel ipAddressLabel = new JLabel("USB Driver/IP Address :");
        ipAddressLabel.setAlignmentX(Component.LEFT_ALIGNMENT);

        JLabel connectionPrefixLabel = new JLabel("Connection Prefix :");
        connectionPrefixLabel.setAlignmentX(Component.LEFT_ALIGNMENT);

        JLabel connectionStringLabel = new JLabel("Connection String :");
        connectionStringLabel.setAlignmentX(Component.LEFT_ALIGNMENT);

        connectionString = new JLabel(" ");
        connectionString.setAlignmentX(Component.LEFT_ALIGNMENT);

        connectionInfoPanel.add(ipAddressLabel);
        connectionInfoPanel.add(addressTextField);
        connectionInfoPanel.add(connectionPrefixLabel);
        connectionInfoPanel.add(connectionBuilderPrefixCombo);
        connectionInfoPanel.add(connectionStringLabel);
        connectionInfoPanel.add(connectionString);
        connectionInfoPanel.add(testConnectionStringButton);

        connectionBuilderLog = new JTextArea();
        connectionBuilderLog.setEditable(false);
        connectionBuilderLog.setText("Log :\n\n");

        JScrollPane logPanel = new JScrollPane();
        logPanel.getViewport().add(connectionBuilderLog);

        JPanel containerPanel = new JPanel();
        containerPanel.setLayout(new BorderLayout());
        containerPanel.add(connectionInfoPanel, BorderLayout.PAGE_START);
        containerPanel.add(logPanel, BorderLayout.CENTER);

        this.add(containerPanel);
    }

    private void testConnectionString() {
        try {
            Connection connection = ConnectionBuilder.build(getConnectionStringForSdk());
            publishProgress("Connection string evaluated as class type " + connection.getClass().getSimpleName());
            connection.open();
            publishProgress("Connection opened successfully");

            if (isAttemptingStatusConnection()) {
                ZebraPrinterLinkOs printer = ZebraPrinterFactory.getLinkOsPrinter(connection);
                publishProgress("Created a printer, attempting to retrieve status");
                PrinterStatus status = printer.getCurrentStatus();
                publishProgress("Is printer ready to print? " + status.isReadyToPrint);

            } else {
                ZebraPrinter printer = ZebraPrinterFactory.getInstance(connection);
                publishProgress("Created a printer, attempting to print a config label");
                printer.printConfigurationLabel();
            }

            publishProgress("Closing connection");
            connection.close();
        } catch (ConnectionException e) {
            DemoDialog.showErrorDialog(ConnectionBuilderDemo.this, "Connection could not be opened", "Error!");
        } catch (ZebraPrinterLanguageUnknownException e) {
            DemoDialog.showErrorDialog(ConnectionBuilderDemo.this, "Could not create printer", "Error!");
        }

    }

    private void publishProgress(String string) {
        connectionBuilderLog.append(string + System.getProperty("line.separator"));
    }

    private void enableButton(final boolean b) {
        SwingUtilities.invokeLater(new Runnable() {
            public void run() {
                testConnectionStringButton.setEnabled(b);
            }
        });
    }

    private String getConnectionStringForSdk() {
        String selectedPrefix = "";
        if (connectionBuilderPrefixCombo.getSelectedIndex() > 0) {
            selectedPrefix = connectionBuilderPrefixCombo.getSelectedItem().toString() + ":";
        }
        String userSuppliedDescriptionString = addressTextField.getText();
        final String finalConnectionString = selectedPrefix + userSuppliedDescriptionString;
        return finalConnectionString;
    }

    private boolean isAttemptingStatusConnection() {
        return connectionBuilderPrefixCombo.getSelectedItem().toString().contains("STATUS");
    }
}
