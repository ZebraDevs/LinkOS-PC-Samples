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

package com.zebra.desktop.devdemo;

import java.awt.BorderLayout;
import java.awt.CardLayout;
import java.awt.Dimension;
import java.awt.event.FocusEvent;
import java.awt.event.FocusListener;
import java.awt.event.ItemEvent;
import java.awt.event.ItemListener;

import javax.swing.JComboBox;
import javax.swing.JLabel;
import javax.swing.JPanel;
import javax.swing.JTextField;

import com.zebra.sdk.comm.Connection;
import com.zebra.sdk.comm.ConnectionException;
import com.zebra.sdk.comm.DriverPrinterConnection;
import com.zebra.sdk.comm.TcpConnection;
import com.zebra.sdk.printer.discovery.DiscoveredPrinterDriver;
import com.zebra.sdk.printer.discovery.UsbDiscoverer;

public class ConnectionCardPanel extends JPanel implements FocusListener {

    private static final long serialVersionUID = -1438813812046734210L;
    public JComboBox comboBox;
    public JComboBox usbPrinterList;
    public JTextField ipAddressTextField;
    public NumericTextField portNumTextField;

    private static final String networkComboBoxLabel = "Network";
    private static final String usbComboBoxLabel = "USB";

    public ConnectionCardPanel() {
        super();
        addFocusListener(this);
        setFocusable(true);

        final JPanel cardPanel = new JPanel();
        JPanel comboBoxPanel = new JPanel();

        comboBox = new JComboBox(new String[] { networkComboBoxLabel, usbComboBoxLabel });
        comboBox.addItemListener(new ItemListener() {

            public void itemStateChanged(ItemEvent evt) {
                CardLayout cl = (CardLayout) (cardPanel.getLayout());
                cl.show(cardPanel, (String) evt.getItem());
            }
        });
        comboBoxPanel.add(comboBox);

        cardPanel.setLayout(new CardLayout());
        JPanel addressPortPanel = createNetworkCard();
        JPanel usbConnectivityPanel = createUsbCard();

        cardPanel.add(addressPortPanel, networkComboBoxLabel);
        cardPanel.add(usbConnectivityPanel, usbComboBoxLabel);

        this.setLayout(new BorderLayout());
        this.add(comboBoxPanel, BorderLayout.NORTH);
        this.add(cardPanel, BorderLayout.CENTER);
    }

    private JPanel createUsbCard() {
        JPanel usbCardPanel = new JPanel();
        usbPrinterList = new JComboBox();

        getUsbPrintersAndAddToComboList();
        usbCardPanel.add(usbPrinterList);

        return usbCardPanel;
    }

    private JPanel createNetworkCard() {
        JPanel addressPortPanel = new JPanel();
        JLabel ipAddressLabel = new JLabel("Ip Address: ");
        addressPortPanel.add(ipAddressLabel);

        ipAddressTextField = new JTextField();
        ipAddressTextField.setPreferredSize(new Dimension(110, 25));
        addressPortPanel.add(ipAddressTextField);

        JLabel portNumLabel = new JLabel("Port: ");
        addressPortPanel.add(portNumLabel);

        portNumTextField = new NumericTextField();
        portNumTextField.setMaxLength(5);
        portNumTextField.setPreferredSize(new Dimension(80, 25));
        addressPortPanel.add(portNumTextField);
        return addressPortPanel;
    }

    public Connection getConnection() throws ConnectionException {
        if (comboBox.getSelectedIndex() == 0) {
            try {
                return new TcpConnection(ipAddressTextField.getText(), portNumTextField.getPortNum());
            } catch (NumberFormatException e) {
                throw new ConnectionException(e.getMessage());
            }
        } else {
            DiscoveredPrinterDriver printer = (DiscoveredPrinterDriver) usbPrinterList.getSelectedItem();
            return new DriverPrinterConnection(printer.printerName);
        }
    }

    public void focusGained(FocusEvent event) {
        if (usbPrinterList != null) {
            int currentIndex = usbPrinterList.getSelectedIndex();
            usbPrinterList.removeAllItems();
            getUsbPrintersAndAddToComboList();
            if ((currentIndex > -1) && (currentIndex < usbPrinterList.getItemCount())) {
                usbPrinterList.setSelectedIndex(currentIndex);
            }
        }
    }

    private void getUsbPrintersAndAddToComboList() {
        DiscoveredPrinterDriver[] discoPrinters;
        try {
            discoPrinters = UsbDiscoverer.getZebraDriverPrinters();
            for (DiscoveredPrinterDriver printer : discoPrinters) {
                usbPrinterList.addItem(printer);
            }

        } catch (ConnectionException e) {
            usbPrinterList.removeAllItems();
            usbPrinterList.addItem("OS not supported");
        }
    }

    public void focusLost(FocusEvent e) {
    }

}
