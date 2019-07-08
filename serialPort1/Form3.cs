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
    public partial class Form3 : Form
    {

        SerialPort sp1 =new SerialPort();
    
        public Form3()
        {
            InitializeComponent();

        }
        private void Form3_Load(object sender, EventArgs e)
        {
            //设置默认值，对应前端波特率、数字位、校验位、停止位
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;
            comboBox4.SelectedIndex = 0;
            comboBox5.SelectedIndex = 0;

            string[] strCom = SerialPort.GetPortNames();
            if (strCom == null)
            {
                MessageBox.Show("本机没有可用串口", "Error");
                return;

            }
            foreach (string com in System.IO.Ports.SerialPort.GetPortNames())
            {
                comboBox1.Items.Add(com);

            }

            comboBox1.SelectedIndex = 0;
            sp1.BaudRate = 9600;
            Control.CheckForIllegalCrossThreadCalls = false;
            sp1.DataReceived += new SerialDataReceivedEventHandler(Sp1_DataReceived);
            //获取或设置一个值，该值在串行通信过程中启用数据终端就绪 (DTR) 信号
            sp1.DtrEnable = true;
            //获取或设置一个值，该值指示在串行通信中是否启用请求发送 (RTS) 信号
            sp1.RtsEnable = true;
            //设置数据读取超时为1秒
            sp1.ReadTimeout = 1000;
            sp1.Close();
          
          
        
        }
        void Sp1_DataReceived(object sender,SerialDataReceivedEventArgs e)
        {
           
            if (sp1.IsOpen)//判断是否打开串口
            {
                try
                {
                    Byte[] receivedData = new byte[sp1.BytesToRead];//创建接收字节数组
                    sp1.Read(receivedData, 0, receivedData.Length);//读取数据
                    AddContent(new UTF8Encoding().GetString(receivedData));//用UTF8传输中文不会乱码
                    

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "错误提示");
                    textBox_send.Text = "";

                }

            }
            else
            {
                MessageBox.Show("请打开某个串口", "错误提示");

            }
        
        
        }

      
        //将接收到的内容显示出来
        private void AddContent(string content)
        {
            this.BeginInvoke(new MethodInvoker(delegate
              {
                  textBox_receive.AppendText(content); 
                  textBox_receive.AppendText("\r\n");
                  //记录收到的字符个数
                  //lblRevCount.Text = (int.Parse(lblRevCount.Text) + content.Length).ToString();
              }));


        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!sp1.IsOpen)
            {
                try
                {
                    //设置串口号
                    string serialName = comboBox1.SelectedItem.ToString();
                    sp1.PortName = serialName;

                    //设置各串口设置
                    string strBaudRate = comboBox2.Text;
                    string strDateBits = comboBox3.Text;
                    string strStopBits = comboBox5.Text;

                    Int32 iBaudRate = Convert.ToInt32(strBaudRate);
                    Int32 iDateBits = Convert.ToInt32(strDateBits);

                    sp1.BaudRate = iBaudRate;//波特率
                    sp1.DataBits = iDateBits;//数据位

                    switch (comboBox5.Text)//停止位
                    {
                        case "1":
                            sp1.StopBits = StopBits.One;
                            break;
                        case "1.5":
                            sp1.StopBits = StopBits.OnePointFive;
                            break;
                        case "2":
                            sp1.StopBits = StopBits.Two;
                            break;
                        default:
                            MessageBox.Show("Error:参数不正确!", "Error");
                            break;

                    }

                    switch (comboBox4.Text)//校验位
                    {
                        case "无":
                            sp1.Parity = Parity.None;
                            break;
                        case "奇校验":
                            sp1.Parity = Parity.Odd;
                            break;
                        case "偶校验":
                            sp1.Parity = Parity.Even;
                            break;
                        default:
                            MessageBox.Show("Erroe:参数不正确!", "Error");
                            break;

                    }
                    if (sp1.IsOpen == true)//如果打开状态，则先关闭
                    {
                        sp1.Close();
                        
                   
                       

                    }

                    //设置控件不可用
                    comboBox1.Enabled = false;
                    comboBox2.Enabled = false;
                    comboBox3.Enabled = false;
                    comboBox4.Enabled = false;
                    comboBox5.Enabled = false;

                   
                    sp1.Open();//打开串口
                   
                    button1.Text = "关闭串口";


                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error:" + ex.Message, "Error");
                    return;

                }

            }
            else
            {
                //恢复控件，设置控件可用
                comboBox1.Enabled = true;
                comboBox2.Enabled = true;
                comboBox3.Enabled = true;
                comboBox4.Enabled = true;
                comboBox5.Enabled = true;

                sp1.Close();//关闭串口
            
                button1.Text = "打开串口";

            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
           
           
            byte[] sendData = null;
            if (!sp1.IsOpen)
            {
                MessageBox.Show("请先打开串口", "Error");
                return;

            }
           
            string strSend = textBox_send.Text;
            try
            {

                sendData = Encoding.UTF8.GetBytes(textBox_send.Text.Trim());
                sp1.Write(sendData, 0, sendData.Length);//写入数据
                textBox_receive.Text = strSend;
              

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:" + ex.Message, "Error");

            }



        }

     
       



   
     

      
    }
}
