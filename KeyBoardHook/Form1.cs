using System;
using System.Windows.Forms;
using KeyBoardHook.KeyLogger.Service;

namespace KeyBoardHook
{
    public partial class Form1 : Form
    {
        private KeyLoggerService KeyLoggerService ; 
       
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (KeyLoggerService == null)
            {
                KeyLoggerService = new KeyLoggerService(this.textBox1);
            }
            KeyLoggerService.Start();
        }
        
        private void button2_Click(object sender, EventArgs e)
        {
            if (KeyLoggerService != null)
            {
                KeyLoggerService.Stop();
                textBox1.Text = "";
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1.SelectionStart = textBox1.Text.Length;

            textBox1.ScrollToCaret(); 
        }

       
    }
}