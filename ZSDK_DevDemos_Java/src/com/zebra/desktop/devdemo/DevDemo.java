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

import java.awt.GraphicsEnvironment;
import java.awt.GridLayout;
import java.awt.Point;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;

import javax.swing.JButton;
import javax.swing.JDialog;
import javax.swing.JFrame;

import com.zebra.desktop.devdemo.connectionbuilder.ConnectionBuilderDemo;
import com.zebra.desktop.devdemo.connectivity.ConnectivityDemo;
import com.zebra.desktop.devdemo.discovery.DiscoveryDemo;
import com.zebra.desktop.devdemo.imageprint.ImagePrintDemo;
import com.zebra.desktop.devdemo.listformats.ListFormatsDemo;
import com.zebra.desktop.devdemo.magcard.MagCardDemo;
import com.zebra.desktop.devdemo.printerstatus.PrinterStatusDemo;
import com.zebra.desktop.devdemo.profile.ProfileDemo;
import com.zebra.desktop.devdemo.sendfile.SendFileDemo;
import com.zebra.desktop.devdemo.settings.SettingsDemo;
import com.zebra.desktop.devdemo.smartcard.SmartCardDemo;
import com.zebra.desktop.devdemo.storedformat.StoredFormatDemo;

public class DevDemo {

    public static void main(String args[]) {
        final JFrame devDemoFrame = new JFrame("Zebra Developer Demos");
        devDemoFrame.setSize(300, 650);
        devDemoFrame.setLayout(new GridLayout(0, 1));

        final String[] demoTitles = new String[] { "Connectivity", "Discovery", "Image Print", "List Formats", "Mag Card", "Printer Status", "Smart Card", "Send File", "Stored Format", "Settings",
                "Profile", "Connection Builder" };
        final JDialog[] demoDiags = generateDemoDialogArray(devDemoFrame);

        for (int i = 0; i < demoTitles.length; ++i) {
            final int index = i;
            JButton demoButton = new JButton(demoTitles[i]);
            demoButton.addActionListener(new ActionListener() {

                public void actionPerformed(ActionEvent e) {
                    demoDiags[index].setTitle(demoTitles[index] + " Demo");
                    demoDiags[index].setVisible(true);
                }
            });
            devDemoFrame.getContentPane().add(demoButton);

        }

        devDemoFrame.setLocation(getCenterPoint(devDemoFrame));
        devDemoFrame.setVisible(true);
        devDemoFrame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
    }

    private static Point getCenterPoint(final JFrame devDemoFrame) {
        Point centerPoint = GraphicsEnvironment.getLocalGraphicsEnvironment().getCenterPoint();
        int x = centerPoint.x - devDemoFrame.getWidth() / 2;
        int y = centerPoint.y - devDemoFrame.getHeight() / 2;
        return new Point(x, y);
    }

    private static JDialog[] generateDemoDialogArray(final JFrame devDemoFrame) {
        return new JDialog[] { new ConnectivityDemo(devDemoFrame), new DiscoveryDemo(devDemoFrame), new ImagePrintDemo(devDemoFrame), new ListFormatsDemo(devDemoFrame), new MagCardDemo(devDemoFrame),
                new PrinterStatusDemo(devDemoFrame), new SmartCardDemo(devDemoFrame), new SendFileDemo(devDemoFrame), new StoredFormatDemo(devDemoFrame), new SettingsDemo(devDemoFrame),
                new ProfileDemo(devDemoFrame), new ConnectionBuilderDemo(devDemoFrame) };
    }
}
