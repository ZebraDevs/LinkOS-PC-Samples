/**********************************************
 * CONFIDENTIAL AND PROPRIETARY
 *
 * The information contained herein is the confidential and the exclusive property of
 * ZIH Corp. This document, and the information contained herein, shall not be copied, reproduced, published,
 * displayed or distributed, in whole or in part, in any medium, by any means, for any purpose without the express
 * written consent of ZIH Corp.
 *
 * Copyright ZIH Corp. 2009
 *
 * ALL RIGHTS RESERVED
 ***********************************************
 *
 * File: ZebraSync.java
 * Description: This java program acts as an interface between the database.zbi program and a MySQL database.
 * 				It prompts the user for information to connect to a MySQL, then opens a connection to the database
 * 				as well as a TCP socket connection to listen for incoming SQL queries from the ZBI program.  It then
 * 				sends the queries to the database and read back the resulting data and sends it back to the ZBI program.
 *
 * Revision: 1.0
 * Date: 010/27/2010
 */



import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Statement;
import java.io.*;
import java.net.*;

public class ZebraSync {
	private static int port=4444, maxConnections=0;
	  // Listen for incoming connections and handle them
	public static void main(String [] args) throws IOException{
        InputStreamReader converter = new InputStreamReader(System.in);
        BufferedReader in = new BufferedReader(converter);

        System.out.println("Enter Server Name: ");
        String serverName = in.readLine();
        System.out.println("Enter Database Name: ");
        String mydatabase = in.readLine();
        System.out.println("Enter Username: ");
        String username = in.readLine();
        System.out.println("Enter Password: ");
        String password = in.readLine();

        // Create a connection to the database
		Connection conn = null;
        try {
            // Load the JDBC driver
            String driverName = "com.mysql.jdbc.Driver";
            Class.forName(driverName);

            // Create a connection to the database
            String url = "jdbc:mysql://" + serverName +  "/" + mydatabase; // a JDBC url
            conn = DriverManager.getConnection(url, username, password);
        } catch (ClassNotFoundException e) {

        	System.out.println(e);
            System.exit(0);

        }
        catch (SQLException ex) {
        	System.out.println(ex);
            System.exit(0);

        }
        System.out.println("DataBase Connection established");


        int i=0;

        try{
          ServerSocket listener = new ServerSocket(port);
          Socket server;
          System.out.println("Server Started.......... ");
          while((i++ < maxConnections) || (maxConnections == 0)){


            server = listener.accept();
            myProcess conn_c= new myProcess(server,conn);
            Thread t = new Thread(conn_c);
            t.start();
          }
        } catch (IOException ioe) {
          System.out.println("IOException on socket listen: " + ioe);
          ioe.printStackTrace();
        }
	}
}
class myProcess implements Runnable {
    private Socket server;
    private String input;
    private Connection conn;

    myProcess(Socket server,Connection connect) {
      this.server=server;
      this.conn=connect;
    }

    @SuppressWarnings("deprecation")
	public void run () {

      input="";



      try {
        // Get input from the client
    	  BufferedReader in = new BufferedReader(new InputStreamReader(server.getInputStream()));

          input=in.readLine();

          System.out.println ("SQL QUERY RECEIVED: " + input);


      } catch (IOException ioe) {
        System.out.println("IOException on socket listen: " + ioe);
        ioe.printStackTrace();
      }
      try {
    	  PrintStream out = new PrintStream(server.getOutputStream());
    	  // Query database
          Statement s = conn.createStatement();

          ResultSet r = s.executeQuery(input);
          while (r.next())
          {
                  System.out.println ("Data 1: " + r.getString("data1") + "Data 2: " + r.getString("data2") );
                  out.println("Data 1: " + r.getString("data1") + "Data 2: " + r.getString("data2"));
          }
  }
  catch (Exception e) {
          System.out.println(e);
          }
  try{
      server.close();
  }
      catch (Exception ioe){
          System.out.println(ioe);
      }
    }
}
