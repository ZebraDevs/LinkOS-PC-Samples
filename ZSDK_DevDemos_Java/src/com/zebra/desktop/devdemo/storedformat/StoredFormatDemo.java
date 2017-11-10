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

package com.zebra.desktop.devdemo.storedformat;

import java.awt.BorderLayout;
import java.awt.Dimension;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.awt.event.MouseEvent;
import java.awt.event.MouseListener;
import java.io.UnsupportedEncodingException;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;

import javax.swing.DefaultListModel;
import javax.swing.JButton;
import javax.swing.JDialog;
import javax.swing.JFrame;
import javax.swing.JList;
import javax.swing.JPanel;
import javax.swing.JScrollPane;
import javax.swing.JTable;
import javax.swing.SwingUtilities;
import javax.swing.table.DefaultTableModel;

import com.zebra.desktop.devdemo.ConnectionCardPanel;
import com.zebra.desktop.devdemo.DemoDialog;
import com.zebra.sdk.comm.Connection;
import com.zebra.sdk.comm.ConnectionException;
import com.zebra.sdk.device.ZebraIllegalArgumentException;
import com.zebra.sdk.printer.FieldDescriptionData;
import com.zebra.sdk.printer.PrinterLanguage;
import com.zebra.sdk.printer.ZebraPrinter;
import com.zebra.sdk.printer.ZebraPrinterFactory;
import com.zebra.sdk.printer.ZebraPrinterLanguageUnknownException;

public class StoredFormatDemo extends DemoDialog {

    private static final long serialVersionUID = 7112840362496182928L;
    private static final String[] COLUMN_NAMES = new String[] { "Variable", "Value" };

    private ConnectionCardPanel connectionPanel;
    private JList formatList;
    private JButton printFormatButton;
    private JTable formatVarTable;
    private JDialog formatVarDialogPopup;
    private DefaultListModel formatListModel;
    private JButton retrieveFormatsButton;
    private List<FieldDescriptionData> variablesList;

    public StoredFormatDemo(JFrame owner) {
        super(owner);

        connectionPanel = new ConnectionCardPanel();

        formatList = new JList();
        formatList.addMouseListener(new ZebraMouseListener());
        JScrollPane formatListScroller = new JScrollPane();
        formatListScroller.getViewport().add(formatList);
        formatListScroller.setPreferredSize(new Dimension(200, 250));

        variablesList = new ArrayList<FieldDescriptionData>();
        formatListModel = new DefaultListModel();
        retrieveFormatsButton = new JButton("Retrieve Formats");

        JPanel formatPanel = new JPanel();
        formatPanel.setLayout(new BorderLayout());
        formatPanel.add(formatListScroller, BorderLayout.NORTH);
        formatPanel.add(retrieveFormatsButton, BorderLayout.SOUTH);

        retrieveFormatsButton.addActionListener(new ActionListener() {

            public void actionPerformed(ActionEvent arg0) {
                new Thread(new Runnable() {

                    public void run() {
                        setRetrieveFormatsButtonState(false);
                        populateListWithFormats();
                        setRetrieveFormatsButtonState(true);
                    }
                }).start();
            }

        });

        this.add(connectionPanel, BorderLayout.NORTH);
        this.add(formatPanel, BorderLayout.CENTER);

    }

    private void setRetrieveFormatsButtonState(final boolean enabled) {
        SwingUtilities.invokeLater(new Runnable() {

            public void run() {
                retrieveFormatsButton.setEnabled(enabled);
            }
        });
    }

    private void populateListWithFormats() {
        formatListModel.clear();

        Connection printerConnection = null;
        try {
            printerConnection = connectionPanel.getConnection();
            printerConnection.open();
            ZebraPrinter printer = ZebraPrinterFactory.getInstance(printerConnection);
            String[] formatExtensions = printer.getPrinterControlLanguage() == PrinterLanguage.ZPL ? new String[] { "ZPL" } : new String[] { "FMT", "LBL" };

            String[] formats = printer.retrieveFileNames(formatExtensions);

            for (String format : formats) {
                formatListModel.addElement(format);
            }

            formatList.setModel(formatListModel);

        } catch (ConnectionException e) {
            DemoDialog.showErrorDialog(StoredFormatDemo.this, e.getMessage(), "Communication Error!");
        } catch (ZebraPrinterLanguageUnknownException e) {
            DemoDialog.showErrorDialog(StoredFormatDemo.this, e.getMessage(), "Communication Error!");
        } catch (ZebraIllegalArgumentException e) {
            DemoDialog.showErrorDialog(StoredFormatDemo.this, e.getMessage(), "Communication Error!");
        } finally {
            try {
                if (printerConnection != null)
                    printerConnection.close();
            } catch (ConnectionException e) {
            }
        }
    }

    private void printFormat(final String formatName) {
        Map<Integer, String> vars = new HashMap<Integer, String>();
        for (int i = 0; i < formatVarTable.getModel().getRowCount(); ++i) {
            int fieldNum = variablesList.get(i).fieldNumber;
            vars.put(fieldNum, (String) formatVarTable.getModel().getValueAt(i, 1));
        }

        Connection printerConnection = null;
        try {
            printerConnection = connectionPanel.getConnection();
            printerConnection.open();
            ZebraPrinterFactory.getInstance(printerConnection).printStoredFormat(formatName, vars, "utf8");
        } catch (UnsupportedEncodingException e) {
            DemoDialog.showErrorDialog(StoredFormatDemo.this, e.getMessage(), "Communication Error!");
        } catch (ConnectionException e) {
            DemoDialog.showErrorDialog(StoredFormatDemo.this, e.getMessage(), "Communication Error!");
        } catch (ZebraPrinterLanguageUnknownException e) {
            DemoDialog.showErrorDialog(StoredFormatDemo.this, e.getMessage(), "Communication Error!");
        } finally {
            if (printerConnection != null)
                try {
                    printerConnection.close();
                } catch (ConnectionException e) {
                }
        }
    }

    private class ZebraMouseListener implements MouseListener {

        public void mouseClicked(MouseEvent event) {
            if (event.getClickCount() == 2 && !event.isConsumed()) {
                event.consume();
                createFormatTableDialog();
            }
        }

        public void mouseEntered(MouseEvent e) {
        }

        public void mouseExited(MouseEvent e) {
        }

        public void mousePressed(MouseEvent e) {
        }

        public void mouseReleased(MouseEvent e) {
        }

    }

    private void createFormatTableDialog() {
        final String formatName = (String) formatList.getSelectedValue();
        Connection printerConnection = null;
        try {
            formatVarTable = new JTable();
            JScrollPane formatVarTableScroller = new JScrollPane();
            formatVarTableScroller.getViewport().add(formatVarTable);

            formatVarDialogPopup = new JDialog(StoredFormatDemo.this, true);
            printFormatButton = new JButton("Print Format");

            formatVarDialogPopup.setLayout(new BorderLayout());
            formatVarDialogPopup.setSize(new Dimension(250, 325));
            formatVarDialogPopup.setLocation(StoredFormatDemo.this.getLocation());
            formatVarDialogPopup.add(formatVarTableScroller, BorderLayout.CENTER);
            formatVarDialogPopup.add(printFormatButton, BorderLayout.SOUTH);

            displayFormatDialog();

            printerConnection = connectionPanel.getConnection();
            printerConnection.open();
            ZebraPrinter printer = ZebraPrinterFactory.getInstance(printerConnection);
            byte[] formatData = printer.retrieveFormatFromPrinter(formatName);

            final DefaultTableModel tableModel = createDefaultTableModel(COLUMN_NAMES);
            FieldDescriptionData[] variables = printer.getVariableFields(new String(formatData, "utf8"));
            tableModel.setDataVector(getFormatVariableArray(variables), COLUMN_NAMES);

            for (int i = 0; i < variables.length; ++i) {
                variablesList.add(variables[i]);
            }

            printFormatButton.addActionListener(new ActionListener() {

                public void actionPerformed(ActionEvent arg0) {
                    new Thread(new Runnable() {

                        public void run() {
                            setPrintFormatButtonEnabled(false);
                            if (formatVarTable.isEditing()) {
                                formatVarTable.getCellEditor().stopCellEditing();
                            }
                            printFormat(formatName);
                            setPrintFormatButtonEnabled(true);
                        }
                    }).start();
                }

            });

            updateFormatTableModel(tableModel);

        } catch (ConnectionException e) {
            DemoDialog.showErrorDialog(StoredFormatDemo.this, e.getMessage(), "Communication Error!");
        } catch (ZebraPrinterLanguageUnknownException e) {
            DemoDialog.showErrorDialog(StoredFormatDemo.this, e.getMessage(), "Communication Error!");
        } catch (UnsupportedEncodingException e) {
            DemoDialog.showErrorDialog(StoredFormatDemo.this, e.getMessage(), "Communication Error!");
        } finally {
            try {
                if (printerConnection != null)
                    printerConnection.close();
            } catch (ConnectionException e) {
            }
        }
    }

    private void updateFormatTableModel(final DefaultTableModel tableModel) {
        SwingUtilities.invokeLater(new Runnable() {

            public void run() {
                formatVarTable.setModel(tableModel);
                formatVarTable.invalidate();
            }
        });
    }

    private void displayFormatDialog() {
        SwingUtilities.invokeLater(new Runnable() {

            public void run() {
                formatVarDialogPopup.setVisible(true);
            }
        });
    }

    private void setPrintFormatButtonEnabled(final boolean setEnabled) {
        SwingUtilities.invokeLater(new Runnable() {

            public void run() {
                printFormatButton.setEnabled(setEnabled);
            }
        });
    }

    private String[][] getFormatVariableArray(FieldDescriptionData[] variables) {
        String[][] formatVars = new String[variables.length][2];
        int counter = 0;
        for (String[] formatVar : formatVars) {
            FieldDescriptionData var = variables[counter++];
            formatVar[0] = var.fieldName == null ? "Field " + var.fieldNumber : var.fieldName;
            formatVar[1] = "";
        }
        return formatVars;
    }

    private DefaultTableModel createDefaultTableModel(final String[] columnNames) {
        final DefaultTableModel tableModel = new DefaultTableModel(null, columnNames) {
            private static final long serialVersionUID = 7324666916867972790L;

            @Override
            public boolean isCellEditable(int row, int column) {
                return column == 1;
            }
        };
        return tableModel;
    }
}
