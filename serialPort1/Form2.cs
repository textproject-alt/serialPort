using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;

namespace serialPort1
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }


        private void Form2_Load(object sender, EventArgs e)
        {
           
            //默认值
            comboBox1.Items.AddRange(SerialPort.GetPortNames());
            comboBox2.Text = "4600";
            serialPort1.DataReceived+=new SerialDataReceivedEventHandler(serialPort1_DataReceived);

        }
  
        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)//串口接收数据事件
        {
            if (!radioButton4.Checked)//如果接收为字符模式
            {
                string str = serialPort1.ReadExisting();//字符串方式读
                textBox2.AppendText(str);//添加数据
            }
            else//接收为数值模式
            {
                byte data;
                data = (byte)serialPort1.ReadByte();
                string str = Convert.ToString(data,16).ToUpper();
                if (str.Length == 1)
                {
                    str = "0" + str;

                }
                else
                {
                    textBox2.AppendText("0x" + str);
                }
            
            }
        
        }
        private void button3_Click(object sender, EventArgs e)
        {
            byte[] data=new byte[1];
            if(serialPort1.IsOpen)//判断是否打开
            {
            if(textBox1.Text!="")
            {
                if (!radioButton1.Checked)
                {
                    try
                    {

                        serialPort1.WriteLine(textBox1.Text);//写数据
                    }
                    catch
                    {
                        MessageBox.Show("串口数据写入错误", "错误");
                        serialPort1.Close();
                        button1.Enabled = true;//打开端口可用
                        button2.Enabled = false;//关闭端口不可用
                    }
                }
                else
                {
                    for (int i = 0; i < (textBox1.Text.Length - textBox1.Text.Length % 2) / 2;i++ )
                    {
                        data[0] = Convert.ToByte(textBox1.Text.Substring(i*2,2),16);
                        serialPort1.Write(data,0,1);

                    }
                    if(textBox1.Text.Length%2!=0)
                    {
                       
                        data[0]=Convert.ToByte(textBox1.Text.Substring(textBox1.Text.Length-1,1),16);
                        serialPort1.Write(data, 0, 1);
                    }
                
                }
            }
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                serialPort1.PortName = comboBox1.Text;//串口号
                serialPort1.BaudRate = Convert.ToInt32(comboBox2.Text, 10);//十进制数据转换
                serialPort1.Open();
                button1.Enabled = false;//打开端口按钮不可用
                button2.Enabled = true;//关闭端口可用

            }
            catch
            {
                MessageBox.Show("端口错误", "错误");

            }

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            serialPort1.Close();
            button1.Enabled = true;//打开端口可用
            button2.Enabled = false;//关闭端口不可用

        }

       
    }
}
