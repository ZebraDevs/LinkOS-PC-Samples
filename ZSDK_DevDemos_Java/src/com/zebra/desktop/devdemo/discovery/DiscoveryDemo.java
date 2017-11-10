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

import java.awt.BorderLayout;
import java.awt.CardLayout;
import java.awt.Dimension;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.awt.event.ItemEvent;
import java.awt.event.ItemListener;

import javax.swing.JButton;
import javax.swing.JComboBox;
import javax.swing.JComponent;
import javax.swing.JFrame;
import javax.swing.JLabel;
import javax.swing.JOptionPane;
import javax.swing.JPanel;
import javax.swing.JScrollPane;
import javax.swing.JTextField;

import com.zebra.desktop.devdemo.DemoDialog;
import com.zebra.desktop.devdemo.NumericTextField;
import com.zebra.sdk.comm.ConnectionException;
import com.zebra.sdk.printer.discovery.DiscoveredPrinter;
import com.zebra.sdk.printer.discovery.DiscoveredPrinterDriver;
import com.zebra.sdk.printer.discovery.DiscoveryException;
import com.zebra.sdk.printer.discovery.DiscoveryHandler;
import com.zebra.sdk.printer.discovery.NetworkDiscoverer;
import com.zebra.sdk.printer.discovery.UsbDiscoverer;

public class DiscoveryDemo extends DemoDialog {

    private static final long serialVersionUID = -6630062113471354112L;

    private final class DiscoveryHandlerImpl implements DiscoveryHandler {

        private JComponent widgetToUpdate;

        public DiscoveryHandlerImpl(JComponent widgetToUpdate) {
            this.widgetToUpdate = widgetToUpdate;
            this.widgetToUpdate.setEnabled(false);
            discoveryMethodCombo.setEnabled(false);
            discoveredPrinterListPanel.clearPrinterList();
        }

        public void foundPrinter(DiscoveredPrinter printer) {
            discoveredPrinterListPanel.addPrinter(printer);
        }

        public void discoveryFinished() {
            discoveryMethodCombo.setEnabled(true);
            widgetToUpdate.setEnabled(true);
        }

        public void discoveryError(String message) {
            showErrorAndEnableWidgets(message, widgetToUpdate);
        }
    }

    private JPanel cardPanel;
    private JComboBox discoveryMethodCombo;
    private DiscoveryListPanel discoveredPrinterListPanel;
    private static final String LOCAL_BROADCAST = "Local Broadcast";
    private static final String DIRECTED_BROADCAST = "Directed Broadcast";
    private static final String MULTICAST_BROADCAST = "Multicast Broadcast";
    private static final String SUBNET_SEARCH = "Subnet Search";
    private static final String USB_DRIVER_SEARCH = "Zebra USB Drivers";
    private static final String PRINTERS_NEAR_ME = "Find Printers Near Me";

    public DiscoveryDemo(JFrame owner) {
        super(owner);
        this.setResizable(false);
        this.setLayout(new BorderLayout());

        discoveredPrinterListPanel = new DiscoveryListPanel();

        discoveryMethodCombo = new JComboBox(new String[] { LOCAL_BROADCAST, DIRECTED_BROADCAST, MULTICAST_BROADCAST, SUBNET_SEARCH, USB_DRIVER_SEARCH, PRINTERS_NEAR_ME });
        discoveryMethodCombo.addItemListener(new ItemListener() {

            public void itemStateChanged(ItemEvent evt) {
                CardLayout cl = (CardLayout) (cardPanel.getLayout());
                cl.show(cardPanel, (String) evt.getItem());
            }
        });

        JPanel comboPanel = new JPanel();
        comboPanel.add(discoveryMethodCombo);

        cardPanel = new JPanel();
        cardPanel.setLayout(new CardLayout());

        JPanel localBroadcastCard = createLocalBroadcastCard();
        JPanel directedBroadcastCard = createDirectedBroadcastCard();
        JPanel multicastCard = createMulticastCard();
        JPanel subnetCard = createSubnetCard();
        JPanel usbDriverCard = createUsbCard();
        JPanel findPrintersNearMeCard = createFindNearMeCard();

        cardPanel.add(localBroadcastCard, LOCAL_BROADCAST);
        cardPanel.add(directedBroadcastCard, DIRECTED_BROADCAST);
        cardPanel.add(multicastCard, MULTICAST_BROADCAST);
        cardPanel.add(subnetCard, SUBNET_SEARCH);
        cardPanel.add(usbDriverCard, USB_DRIVER_SEARCH);
        cardPanel.add(findPrintersNearMeCard, PRINTERS_NEAR_ME);

        this.add(comboPanel, BorderLayout.PAGE_START);
        this.add(cardPanel, BorderLayout.CENTER);
        this.add(discoveredPrinterListPanel, BorderLayout.PAGE_END);

    }

    private JPanel createFindNearMeCard() {
        JPanel findPrintersNearMeCard = new JPanel();
        final JButton discoverButton = new JButton("Discover Printers");
        findPrintersNearMeCard.add(discoverButton);

        discoverButton.addActionListener(new ActionListener() {

            public void actionPerformed(ActionEvent e) {
                try {
                    discoverButton.setEnabled(false);
                    NetworkDiscoverer.findPrinters(new DiscoveryHandlerImpl(discoverButton));
                } catch (DiscoveryException e1) {
                    DemoDialog.showErrorDialog(DiscoveryDemo.this, e1.getMessage(), "Discovery Error!");
                }
            }
        });
        return findPrintersNearMeCard;
    }

    private JPanel createUsbCard() {
        JPanel usbDriverCard = new JPanel();
        JButton discoverButton = new JButton("Discover Printers");

        discoverButton.addActionListener(new ActionListener() {

            public void actionPerformed(ActionEvent e) {
                discoveredPrinterListPanel.clearPrinterList();
                try {
                    for (DiscoveredPrinterDriver printer : UsbDiscoverer.getZebraDriverPrinters()) {
                        discoveredPrinterListPanel.addPrinter(printer);
                    }
                } catch (ConnectionException e1) {
                    return;
                }

            }
        });
        usbDriverCard.add(discoverButton);
        return usbDriverCard;
    }

    private JPanel createDirectedBroadcastCard() {
        JPanel directedBroadcastCard = new JPanel();
        directedBroadcastCard.add(new JLabel("IP Address: "));

        final JTextField directedAddress = new JTextField();
        directedAddress.setPreferredSize(new Dimension(200, 25));
        directedBroadcastCard.add(directedAddress);

        final JButton discoverButton = new JButton("Discover Printers");
        directedBroadcastCard.add(discoverButton);
        discoverButton.addActionListener(new ActionListener() {

            public void actionPerformed(ActionEvent e) {
                try {
                    discoverButton.setEnabled(false);
                    NetworkDiscoverer.directedBroadcast(new DiscoveryHandlerImpl(discoverButton), directedAddress.getText());
                } catch (DiscoveryException e1) {
                    showErrorAndEnableWidgets(e1.getMessage(), discoverButton);
                }
            }
        });

        return directedBroadcastCard;
    }

    private JPanel createLocalBroadcastCard() {
        JPanel localBroadcastCard = new JPanel();
        final JButton discoverButton = new JButton("Discover Printers");
        localBroadcastCard.add(discoverButton);
        discoverButton.addActionListener(new ActionListener() {

            public void actionPerformed(ActionEvent e) {
                try {
                    discoverButton.setEnabled(false);
                    NetworkDiscoverer.localBroadcast(new DiscoveryHandlerImpl(discoverButton));
                } catch (DiscoveryException e1) {
                    showErrorAndEnableWidgets(e1.getMessage(), discoverButton);
                }
            }
        });
        return localBroadcastCard;
    }

    private JPanel createSubnetCard() {
        JPanel subnetCard = new JPanel();

        subnetCard.add(new JLabel("Subnet Range: "));

        final JTextField subnetRange = new JTextField();
        subnetRange.setPreferredSize(new Dimension(200, 25));
        subnetCard.add(subnetRange);

        final JButton discoverButton = new JButton("Discover Printers");
        subnetCard.add(discoverButton);
        discoverButton.addActionListener(new ActionListener() {

            public void actionPerformed(ActionEvent e) {
                try {
                    discoverButton.setEnabled(false);
                    NetworkDiscoverer.subnetSearch(new DiscoveryHandlerImpl(discoverButton), subnetRange.getText());
                } catch (DiscoveryException e1) {
                    showErrorAndEnableWidgets(e1.getMessage(), discoverButton);
                }
            }
        });

        return subnetCard;
    }

    private JPanel createMulticastCard() {
        JPanel masterPanel = new JPanel();

        masterPanel.add(new JLabel("Number of hops:"));

        final NumericTextField numHopsTextField = new NumericTextField("5", 3);

        numHopsTextField.setPreferredSize(new Dimension(100, 25));
        masterPanel.add(numHopsTextField);

        final JButton discoverButton = new JButton("Discover Printers");
        masterPanel.add(discoverButton);

        discoverButton.addActionListener(new ActionListener() {

            public void actionPerformed(ActionEvent arg0) {
                int numberOfHops = Integer.parseInt(numHopsTextField.getText());

                try {
                    NetworkDiscoverer.multicast(new DiscoveryHandlerImpl(discoverButton), numberOfHops);
                } catch (NumberFormatException e) {
                    showErrorAndEnableWidgets(e.getMessage(), discoverButton);
                } catch (DiscoveryException e) {
                    showErrorAndEnableWidgets(e.getMessage(), discoverButton);
                }

            }
        });

        JScrollPane multicastCard = new JScrollPane(masterPanel);
        multicastCard.setVerticalScrollBarPolicy(JScrollPane.VERTICAL_SCROLLBAR_AS_NEEDED);
        return masterPanel;
    }

    private void showErrorAndEnableWidgets(String errorMessage, JComponent componentToUpdate) {
        JOptionPane.showMessageDialog(cardPanel, errorMessage, "Discovery Error", JOptionPane.ERROR_MESSAGE);
        componentToUpdate.setEnabled(true);
        discoveryMethodCombo.setEnabled(true);
    }
}
