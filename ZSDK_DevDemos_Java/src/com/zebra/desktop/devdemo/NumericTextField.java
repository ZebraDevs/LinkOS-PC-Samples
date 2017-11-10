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

import java.awt.event.KeyEvent;
import java.awt.event.KeyListener;

import javax.swing.JTextField;

public class NumericTextField extends JTextField implements KeyListener {

    /**
	 * 
	 * 
	 */
    private static final long serialVersionUID = -2162631149455947036L;
    private int maxLength;
    private static final int DEFAULT_MAX_LENGTH = 3;

    public NumericTextField() {
        this("");
    }

    public NumericTextField(String text) {
        this(text, DEFAULT_MAX_LENGTH);
    }

    public NumericTextField(String text, int maxLength) {
        super(text);
        this.addKeyListener(this);
        this.maxLength = maxLength;
    }

    public void setMaxLength(int maxLength) {
        this.maxLength = maxLength;
    }

    public void keyTyped(KeyEvent key) {
        if (Character.isDigit(key.getKeyChar()) == false)
            key.consume();
        else if (this.getText().length() >= maxLength)
            key.consume();
    }

    public void keyPressed(KeyEvent e) {
    }

    public void keyReleased(KeyEvent e) {
    }

    public int getPortNum() {
        return Integer.parseInt(getText());
    }

}
