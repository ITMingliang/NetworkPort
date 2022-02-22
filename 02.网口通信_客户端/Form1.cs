using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _02.网口通信_客户端
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
      
        //接收数据
        private void ReceiveMsg()
        {
            while (true)
            {
                byte[] buffer = new byte[1024 * 1024 * 2];
                int len = 0;

                try
                {
                    len = client.Receive(buffer);
                }
                catch (Exception ex)
                {
                    Message("服务器断开连接~");
                    break;

                }

                if (len > 0)
                {
                    string msg = Encoding.Default.GetString(buffer, 0, len);
                    Message("服务器：" + msg);
                }
             
            }
        }

        private void btConnect_Click(object sender, EventArgs e)
        {
            IPEndPoint point = new IPEndPoint(IPAddress.Parse(tbIP.Text), int.Parse(tbPort.Text));

            try
            {
                client.Connect(point);
            }
            catch (Exception ex)
            {
                MessageBox.Show("与服务器连接失败: " + ex.Message);
                return;
            }

            Task.Factory.StartNew(new Action(() =>
            {
                ReceiveMsg();
            }));

            MessageBox.Show("与服务器连接成功~");
            btConnect.Enabled = false;
        }

        private void btsendMsg_Click(object sender, EventArgs e)
        {
            if (client!=null)
            {
                try
                {
                    client.Send(Encoding.Default.GetBytes(tbMsg.Text));
                    Message("客户机：" + tbMsg.Text);

                }
                catch (Exception ex)
                {

                }
            }
            
        }

        string time = "";
        private void Message(string info)
        {
            time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            if (!listView1.InvokeRequired)
            {
                ListViewItem lst = new ListViewItem(time);
                lst.SubItems.Add(info);
                listView1.Items.Insert(listView1.Items.Count, lst);

            }
            else
            {
                Invoke(new Action(() =>
                {
                    ListViewItem lst = new ListViewItem(time, info);
                    lst.SubItems.Add(info);
                    listView1.Items.Insert(listView1.Items.Count, lst);
                }));


            }
        }
    }
}
