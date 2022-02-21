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

namespace _01.网口通信_服务端
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Socket server =new Socket (AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
        private Dictionary<string,Socket> Clientlist = new Dictionary<string,Socket>();
        string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");


        private void btStart_Click(object sender, EventArgs e)
        {
            IPEndPoint point = new IPEndPoint(IPAddress.Parse(tbIP.Text), int.Parse(tbPort.Text));

            try
            {
                server.Bind(point);
            }
            catch (Exception ex)
            {
                MessageBox.Show("无法启动服务器：" + ex.Message);
              
            }
            server.Listen(3);

            Task.Factory.StartNew(new Action( () =>
            {
                Listens();
            }
            ));

            MessageBox.Show("服务器启动成功~");
            btStart.Enabled = false;

        }

        Socket Client;

        private void Listens()
        {
            while (true)
            {
                Client = server.Accept();
                string  client =Client.RemoteEndPoint.ToString();
                //MessageBox.Show(client + ":连接了服务器");

                Message(client + ": 连接上了服务器");
                Clientlist.Add(client,Client);

                Task.Factory.StartNew(new Action((() =>
                {
                    ReceiveMsg(Client);
                })));
            }
        }

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

        private void btSend_Click(object sender, EventArgs e)
        {
            if (Client.Connected)
            {
                try
                {
                    Client.Send(Encoding.Default.GetBytes(tbsendMsg.Text));
                    Message("服务机：" + tbsendMsg.Text);
                }
                catch (Exception ex)
                {

                   
                }
            }
        }

        private  void ReceiveMsg( Socket Client )
        {
            while (true)
            {
                DateTime a = DateTime.Now;
                byte[] buffer = new byte[10000];
                int len = 0;
                string client = Client.RemoteEndPoint.ToString();

                try
                {
                    len = Client.Receive(buffer);
                }
                catch (Exception ex)
                {
                    Message(client + "：失去连接~");
                    break;
                    
                }

                if (len > 0)
                {
                    string msg=Encoding.Default.GetString(buffer, 0, len);
                    Message("客户端-"+client + ": " + msg);
                }
                else
                {
                    Message(client + ": 失去连接~");
                    Clientlist.Remove(client);  
                }
            }
        }

    }
}
