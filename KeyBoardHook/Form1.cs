using System;
using System.Windows.Forms;
using ClassLibrary1;
using KeyBoardHook.Common.Native;
using KeyBoardHook.KeyLogger.Service;

namespace KeyBoardHook
{
    public partial class Form1 : Form
    {
        private KeyLoggerService KeyLoggerService ;

        // [DllImport("ClassLibrary1.dll", EntryPoint="add")]
        // public static extern bool add(int num); //引用外部连接，这个是第三方开发的一个读com口的，需要把RFIDLIB.dll这个文件
        public Form1()
        {
            InitializeComponent();
            // global::MyLib.MyLib _myLib = new global::MyLib.MyLib();
            // MessageBox.Show("引入 内部 DLL 项目" + _myLib.add(1) + "");
            // MessageBox.Show("引入 外部 DLL 项目" + new MyLibEx().add(1) + "");

        }
      
        private void button1_Click(object sender, EventArgs e)
        {
            if (KeyLoggerService == null)
            {
                KeyLoggerService = new KeyLoggerService(this.textBox1,this.textBox2,this.comboBox1);
            }
            KeyLoggerService.Start(comboBox1.Text,textBox3.Text, textBox4.Text);
        }
        
        private void button2_Click(object sender, EventArgs e)
        {
            if (KeyLoggerService != null)
            {
                KeyLoggerService.Stop();
                textBox1.Text = "";
                textBox2.Text = "";
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox1.SelectionStart = textBox1.Text.Length;

            textBox1.ScrollToCaret(); 
        }

       
    }
}