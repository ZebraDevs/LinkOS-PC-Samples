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
import java.awt.Point;

import javax.swing.JDialog;
import javax.swing.JFrame;
import javax.swing.JOptionPane;

public class DemoDialog extends JDialog {

    private static final long serialVersionUID = 7246864556471418030L;

    public static final int DEFAULT_HEIGHT = 375;
    public static final int DEFAULT_WIDTH = 600;

    public DemoDialog(JFrame owner) {
        this(owner, DEFAULT_WIDTH, DEFAULT_HEIGHT);
    }

    public DemoDialog(JFrame owner, int width, int height) {
        super(owner, true);
        this.setLocation(getCenterPoint(owner));
        this.setSize(width, height);
        this.setResizable(false);
        this.setDefaultCloseOperation(JDialog.DISPOSE_ON_CLOSE);
    }

    private static Point getCenterPoint(final JFrame demoDialog) {
        Point centerPoint = GraphicsEnvironment.getLocalGraphicsEnvironment().getCenterPoint();
        int x = centerPoint.x - demoDialog.getWidth() / 2;
        int y = centerPoint.y - demoDialog.getHeight() / 2;
        return new Point(x, y);
    }

    public static void showErrorDialog(JDialog parent, String message, String title) {
        JOptionPane.showMessageDialog(parent, message, title, JOptionPane.ERROR_MESSAGE);
    }
}
