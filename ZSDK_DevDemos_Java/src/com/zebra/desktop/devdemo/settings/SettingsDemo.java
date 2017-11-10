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

package com.zebra.desktop.devdemo.settings;

import java.awt.BorderLayout;
import java.awt.Dimension;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.util.HashMap;
import java.util.Map;

import javax.swing.JButton;
import javax.swing.JFrame;
import javax.swing.JPanel;
import javax.swing.JScrollPane;
import javax.swing.JTable;
import javax.swing.SwingUtilities;
import javax.swing.event.TableModelEvent;
import javax.swing.event.TableModelListener;
import javax.swing.table.DefaultTableModel;

import com.zebra.desktop.devdemo.ConnectionCardPanel;
import com.zebra.desktop.devdemo.DemoDialog;
import com.zebra.sdk.comm.Connection;
import com.zebra.sdk.comm.ConnectionException;
import com.zebra.sdk.printer.ZebraPrinter;
import com.zebra.sdk.printer.ZebraPrinterFactory;
import com.zebra.sdk.printer.ZebraPrinterLanguageUnknownException;
import com.zebra.sdk.printer.ZebraPrinterLinkOs;
import com.zebra.sdk.settings.SettingsException;

public class SettingsDemo extends DemoDialog {

    private static final long serialVersionUID = 7112840362496182928L;

    private static final String[] COL_NAMES = new String[] { "Key", "Value", "Range" };

    private ConnectionCardPanel connectionPanel;
    private JTable settingsTable;
    private JButton saveSettingsButton;
    private JButton getSettingsListButton;
    private HashMap<String, String> modifiedSettings;

    public SettingsDemo(JFrame owner) {
        super(owner);
        connectionPanel = new ConnectionCardPanel();
        modifiedSettings = new HashMap<String, String>();
        this.add(connectionPanel, BorderLayout.NORTH);
        this.add(createSettingPanel(), BorderLayout.CENTER);

    }

    private JPanel createSettingPanel() {

        JPanel settingsListPanel = new JPanel();
        settingsListPanel.setLayout(new BorderLayout());

        getSettingsListButton = new JButton("Get Settings");
        getSettingsListButton.addActionListener(new ActionListener() {

            public void actionPerformed(ActionEvent arg0) {
                new Thread(new Runnable() {

                    public void run() {
                        setCommButtonsEnabled(false);
                        updateSettingsTable();
                        setCommButtonsEnabled(true);
                    }
                }).start();
            }
        });

        DefaultTableModel tableModel = new DefaultTableModel(null, COL_NAMES);
        settingsTable = new JTable(tableModel);

        JScrollPane settingListScroller = new JScrollPane();
        settingListScroller.getViewport().add(settingsTable);
        settingListScroller.setPreferredSize(new Dimension(200, 250));

        saveSettingsButton = new JButton("Save Settings AND Refresh");
        saveSettingsButton.addActionListener(new ActionListener() {

            public void actionPerformed(ActionEvent arg0) {
                new Thread(new Runnable() {

                    public void run() {
                        setCommButtonsEnabled(false);
                        saveModifiedSettingsToPrinter();
                        updateSettingsTable();
                        setCommButtonsEnabled(true);
                    }

                }).start();

            }

        });

        settingsListPanel.add(getSettingsListButton, BorderLayout.NORTH);
        settingsListPanel.add(settingListScroller, BorderLayout.CENTER);
        settingsListPanel.add(saveSettingsButton, BorderLayout.SOUTH);
        return settingsListPanel;
    }

    private void setCommButtonsEnabled(final boolean enabled) {
        SwingUtilities.invokeLater(new Runnable() {

            public void run() {
                getSettingsListButton.setEnabled(enabled);
                saveSettingsButton.setEnabled(enabled);
            }
        });
    }

    private void saveModifiedSettingsToPrinter() {
        Connection pConn = null;
        try {
            pConn = connectionPanel.getConnection();
            pConn.open();
            ZebraPrinter genericPrinter = ZebraPrinterFactory.getInstance(pConn);
            ZebraPrinterLinkOs printer = ZebraPrinterFactory.createLinkOsPrinter(genericPrinter);
            if (printer != null) {
                for (String key : modifiedSettings.keySet()) {
                    if (printer.isSettingReadOnly(key) == false) {
                        printer.setSetting(key, modifiedSettings.get(key));
                    }
                }
            } else {
                DemoDialog.showErrorDialog(SettingsDemo.this, "Connected printer does not support settings", "Connection Error!");
            }
        } catch (ConnectionException e) {
            DemoDialog.showErrorDialog(SettingsDemo.this, e.getMessage(), "Connection Error!");
        } catch (ZebraPrinterLanguageUnknownException e) {
            DemoDialog.showErrorDialog(SettingsDemo.this, e.getMessage(), "Connection Error!");
        } catch (SettingsException e) {
            DemoDialog.showErrorDialog(SettingsDemo.this, e.getMessage(), "Settings Error!");
        } finally {
            try {
                if (pConn != null)
                    pConn.close();
            } catch (ConnectionException e) {
            }
        }
    }

    private void updateSettingsTable() {
        Connection pConn = null;
        try {

            SwingUtilities.invokeLater(new Runnable() {

                public void run() {
                    settingsTable.removeAll();
                }
            });

            pConn = connectionPanel.getConnection();
            pConn.open();
            ZebraPrinter genericPrinter = ZebraPrinterFactory.getInstance(pConn);
            ZebraPrinterLinkOs printer = ZebraPrinterFactory.createLinkOsPrinter(genericPrinter);
            if (printer != null) {
                Map<String, String> settings = printer.getAllSettingValues();

                final String[][] data = new String[settings.keySet().size()][3];

                int counter = 0;
                for (String key : settings.keySet()) {
                    data[counter][0] = key;
                    data[counter][1] = settings.get(key);
                    data[counter][2] = printer.getSettingRange(key);
                    ++counter;
                }

                final DefaultTableModel defaultTableModel = new DefaultTableModel(data, COL_NAMES) {
                    private static final long serialVersionUID = 1523076389319379461L;

                    public boolean isCellEditable(int row, int column) {
                        return column == 1;
                    }
                };

                defaultTableModel.addTableModelListener(new TableModelListener() {

                    public void tableChanged(TableModelEvent arg0) {
                        if (settingsTable.getSelectedRow() != -1 && settingsTable.getSelectedColumn() != -1) {
                            String key = data[settingsTable.getSelectedRow()][0];
                            String newValue = (String) settingsTable.getModel().getValueAt(settingsTable.getSelectedRow(), settingsTable.getSelectedColumn());
                            modifiedSettings.put(key, newValue);
                        }
                    }
                });

                SwingUtilities.invokeLater(new Runnable() {

                    public void run() {
                        settingsTable.setModel(defaultTableModel);
                    }
                });

            } else {
                DemoDialog.showErrorDialog(SettingsDemo.this, "Connected printer does not support settings", "Connection Error!");

            }
        } catch (ConnectionException e) {
            DemoDialog.showErrorDialog(SettingsDemo.this, e.getMessage(), "Connection Error!");
        } catch (ZebraPrinterLanguageUnknownException e) {
            DemoDialog.showErrorDialog(SettingsDemo.this, e.getMessage(), "Connection Error!");
        } catch (SettingsException e) {
            DemoDialog.showErrorDialog(SettingsDemo.this, e.getMessage(), "Settings Error!");
        } finally {
            if (pConn != null) {
                try {
                    pConn.close();
                } catch (ConnectionException e) {
                    DemoDialog.showErrorDialog(SettingsDemo.this, e.getMessage(), "Close Connection Error!");
                }
            }
        }
    }

}
