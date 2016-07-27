/*
 * Created by SharpDevelop.
 * User: Administrator
 * Date: 2007/07/02
 * Time: 12:50
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace KeyLoggerTest
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm
	{
		private KeyLogger.Keylogger keylogger;
			
		[STAThread]
		public static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}
		
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// TODO: Add constructor code after the InitializeComponent() call.
			//
		}
		
		void Button1Click(object sender, System.EventArgs e)
		{
			string fileName=textBox1.Text.Trim();
			keylogger=new KeyLogger.Keylogger(fileName);
			keylogger.startLoging();
		}
		
		void Button2Click(object sender, System.EventArgs e)
		{
			if(keylogger!=null&&keylogger.HasStart)
			{
				keylogger.stopLoging();
			}
		}

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
	}
}
