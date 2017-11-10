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

package com.zebra.desktop.devdemo.discovery;

import java.awt.Dimension;
import java.awt.FlowLayout;
import java.util.Map;

import javax.swing.DefaultListModel;
import javax.swing.JList;
import javax.swing.JPanel;
import javax.swing.JScrollPane;
import javax.swing.JTextArea;
import javax.swing.event.ListSelectionEvent;
import javax.swing.event.ListSelectionListener;

import com.zebra.sdk.printer.discovery.DiscoveredPrinter;

public class DiscoveryListPanel extends JPanel {

    private JTextArea discoveryPacketData;
    private JList printerList;
    private DefaultListModel printerListModel;
    private static final long serialVersionUID = -3623486453245304445L;

    public DiscoveryListPanel() {
        this.setLayout(new FlowLayout());

        printerList = new JList();
        discoveryPacketData = new JTextArea();
        discoveryPacketData.setEditable(false);

        printerListModel = new DefaultListModel();
        printerList.addListSelectionListener(new ListSelectionListener() {

            public void valueChanged(ListSelectionEvent arg0) {
                int selectedIndex = printerList.getSelectedIndex();
                if (selectedIndex != -1) {
                    Object potentialDiscoveredPrinter = printerListModel.get(selectedIndex);
                    if (potentialDiscoveredPrinter instanceof DiscoveredPrinter) {
                        StringBuilder sb = new StringBuilder();
                        Map<String, String> availSettings = ((DiscoveredPrinter) potentialDiscoveredPrinter).getDiscoveryDataMap();
                        for (String key : availSettings.keySet()) {
                            sb.append(key + ": " + availSettings.get(key) + "\r\n");
                        }
                        discoveryPacketData.setText(sb.toString());
                        discoveryPacketData.setCaretPosition(0);
                    }
                }
            }
        });

        JScrollPane printerListScroller = new JScrollPane();
        printerListScroller.getViewport().add(printerList);
        printerListScroller.setPreferredSize(new Dimension(200, 250));

        JScrollPane discoveryPacketDataScroller = new JScrollPane();
        discoveryPacketDataScroller.getViewport().add(discoveryPacketData);
        discoveryPacketDataScroller.setPreferredSize(new Dimension(300, 250));

        this.add(printerListScroller);
        this.add(discoveryPacketDataScroller);
    }

    public void addPrinter(DiscoveredPrinter printer) {
        printerListModel.addElement(printer);
        updateListModel();
    }

    public void clearPrinterList() {
        printerListModel.clear();
        discoveryPacketData.setText("");
        updateListModel();

    }

    private void updateListModel() {
        printerList.setModel(printerListModel);
        printerList.invalidate();
        printerList.validate();
    }

}
