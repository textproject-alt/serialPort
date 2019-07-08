using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;
using System.Xml;




namespace serialPort1
{
    public partial class Form1 : Form
    {

        SerialPort sp1 = new SerialPort();


        public Form1()
        {
            InitializeComponent();
        }
    
        private long receive_count = 0; //接收字节计数
        private StringBuilder sb = new StringBuilder();//为了避免在接收处理函数中反复调用
        private void Form1_Load(object sender, EventArgs e)
        {
           

            comboBox1.Items.AddRange(SerialPort.GetPortNames());
            sp1.BaudRate = 9600;
            //设置默认值
            comboBox2.Text = "9600";
            comboBox3.Text = "8";
            comboBox4.Text = "None";
            comboBox5.Text = "1";

            //spReceive.DataReceived += new SerialDataReceivedEventHandler(Port_DataReceived);
            sp1.DataReceived += new SerialDataReceivedEventHandler(Sp1_DataReceived);
        }


        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                //根据当前串口属性来判断是否打开
                if (sp1.IsOpen)
                {
                    //串口已经处于打开状态
                    sp1.Close();    //关闭串口
                    button1.Text = "打开串口";
                    button1.BackColor = Color.ForestGreen;
                    comboBox1.Enabled = true;
                    comboBox2.Enabled = true;
                    comboBox3.Enabled = true;
                    comboBox4.Enabled = true;
                    comboBox5.Enabled = true;
                    textBox_receive.Text = "";  //清空接收区
                    textBox_send.Text = "";     //清空发送区
                }
                else
                {
                    //串口已经处于关闭状态，则设置好串口属性后打开
                    comboBox1.Enabled = false;
                    comboBox2.Enabled = false;
                    comboBox3.Enabled = false;
                    comboBox4.Enabled = false;
                    comboBox5.Enabled = false;
                    serialPort1.PortName = comboBox1.Text;
                    serialPort1.BaudRate = Convert.ToInt32(comboBox2.Text);
                    serialPort1.DataBits = Convert.ToInt16(comboBox3.Text);

                    if (comboBox4.Text.Equals("None"))
                        serialPort1.Parity = System.IO.Ports.Parity.None;
                    else if (comboBox4.Text.Equals("Odd"))
                        serialPort1.Parity = System.IO.Ports.Parity.Odd;
                    else if (comboBox4.Text.Equals("Even"))
                        serialPort1.Parity = System.IO.Ports.Parity.Even;
                    else if (comboBox4.Text.Equals("Mark"))
                        serialPort1.Parity = System.IO.Ports.Parity.Mark;
                    else if (comboBox4.Text.Equals("Space"))
                        serialPort1.Parity = System.IO.Ports.Parity.Space;

                    if (comboBox5.Text.Equals("1"))
                        serialPort1.StopBits = System.IO.Ports.StopBits.One;
                    else if (comboBox5.Text.Equals("1.5"))
                        serialPort1.StopBits = System.IO.Ports.StopBits.OnePointFive;
                    else if (comboBox5.Text.Equals("2"))
                        serialPort1.StopBits = System.IO.Ports.StopBits.Two;

                    sp1.Open();     //打开串口
                    button1.Text = "关闭串口";
                    button1.BackColor = Color.Firebrick;


                }

            }
            catch (Exception ex)
            {
                //捕获到异常，创建一个新的对象，之前的不可以再用
                sp1 = new System.IO.Ports.SerialPort();
                //刷新COM口选项
                comboBox1.Items.Clear();
                comboBox1.Items.AddRange(SerialPort.GetPortNames());
                //响铃并显示异常给用户
                System.Media.SystemSounds.Beep.Play();
                button1.Text = "打开串口";
                button1.BackColor = Color.ForestGreen;
                MessageBox.Show(ex.Message);
                comboBox1.Enabled = true;
                comboBox2.Enabled = true;
                comboBox3.Enabled = true;
                comboBox4.Enabled = true;
                comboBox5.Enabled = true;

            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox_receive.Text = "";  //清空接收文本框
            textBox_send.Text = "";     //清空发送文本框
            receive_count = 0;          //计数清零
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                //判断串口是否打开
                if (sp1.IsOpen)
                {
                    //串口处于开启状态，将发送区文本发送
                   
                    sp1.Write(textBox_send.Text.Trim());//写数据

                }


            }
            catch (Exception ex)
            {
                //捕获到异常，创建一个新的对象，之前的不可以再用
                sp1 = new System.IO.Ports.SerialPort();
                //刷新COM口选项
                comboBox1.Items.Clear();
                comboBox1.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames());
                //响铃并显示异常给用户
                System.Media.SystemSounds.Beep.Play();
                button1.Text = "打开串口";
                button1.BackColor = Color.ForestGreen;
                MessageBox.Show(ex.Message);
                comboBox1.Enabled = true;
                comboBox2.Enabled = true;
                comboBox3.Enabled = true;
                comboBox4.Enabled = true;
                comboBox5.Enabled = true;

            }
        }

         void Sp1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
           
            int num = sp1.BytesToRead;//获取接收缓冲区的字节数
            byte[] received_buf = new byte[num];//声明一个大小为num的字节数据用于存放读出的byte型数据

            receive_count += num;               //接收字节计数变量增加nun
            sp1.Read(received_buf, 0, num);//读取接收缓冲区中num个字节到byte数组中


            //遍历数组进行字符串转化及拼接
            foreach (byte b in received_buf)
            {
                sb.Append(b.ToString());
            }
            if (radioButton2.Checked)
            {
                //选中HEX模式显示
                foreach (byte b in received_buf)
                {
                    sb.Append(b.ToString("X2") + ' ');    //将byte型数据转化为2位16进制文本显示,用空格隔开
                }

            }
            else
            {
                //选中ASCII模式显示
                sb.Append(Encoding.ASCII.GetString(received_buf));  //将整个数组解码为ASCII数组

            }
            try
            {
                //因为要访问UI资源，所以需要使用invoke方式同步ui
                Invoke((EventHandler)(delegate
                {
                    textBox_receive.AppendText(sb.ToString());

                }
                  )
                );
            }
            catch (Exception ex)
            {
                //响铃并显示异常给用户
                System.Media.SystemSounds.Beep.Play();
                MessageBox.Show(ex.Message);


            }

        }

       

       

       

       

       
    }
}
