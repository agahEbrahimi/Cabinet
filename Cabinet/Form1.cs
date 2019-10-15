using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cabinet
{
    public partial class Form1 : Form
    {
        private User user;
        private string userPicked = "a";
       
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            User.ipHash.Add("131", "a");
            User.ipHash.Add("41", "b");
            User.ipHash.Add("187", "c");

            Thread t3 = new Thread(() => listen());
            t3.Start();
        }
        public void send(string tree, string address, string key, string type, string otherAddress, Boolean b)
        {
            Boolean done = false;
            Boolean exception_thrown = false;
            Socket sending_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram,
            ProtocolType.Udp);
            string txt = "";
            if (type == "B")
            {
                txt += System.IO.File.ReadAllText(getPath("Tree\\global.tree")) + "_" + user.getPublicHash() + "_" + type;
            }
            else if (type == "S")
            {
                txt += key + "_" + address + "_" + type;
            }
            else if (type == "T")
            {
                txt += b.ToString() + "_" +type;
            }
            else
            {
                txt += System.IO.File.ReadAllText(getPath("Tree\\global.tree")) + "_" + user.getPublicHash() + "_" + type;
            }
            IPAddress send_to_address;
            if (otherAddress == null)
            {
                send_to_address = IPAddress.Parse("192.168.43.255");
            }
            else
            {
                
                send_to_address = IPAddress.Parse("192.168.43."+User.ipHash[otherAddress]);

            }

            IPEndPoint sending_end_point = new IPEndPoint(send_to_address, 11000);

            while (!done)
            {
                string text_to_send = txt;
                if (text_to_send.Length == 0)
                {
                    done = true;
                }
                else
                {

                    byte[] send_buffer = Encoding.ASCII.GetBytes(text_to_send);
                    try
                    {
                        sending_socket.SendTo(send_buffer, sending_end_point);
                        done = true;
                    }
                    catch (Exception send_exception)
                    {
                        exception_thrown = true;
                    }
                    if (exception_thrown == false)
                    {

                    }
                    else
                    {
                        exception_thrown = false;
                    }
                }
            }
        }

        private static int count = 0;
        public void listen()
        {
            const int listenPort = 11000;
            bool done = false;
            UdpClient listener = new UdpClient(listenPort);
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, listenPort);
            string received_data;
            byte[] receive_byte_array;
            try
            {
                while (!done)
                {
                    receive_byte_array = listener.Receive(ref groupEP);
                    received_data = Encoding.ASCII.GetString(receive_byte_array, 0, receive_byte_array.Length);
                    string[] recieved = received_data.Split('_');
                    MessageBox.Show(recieved[0]);
                    if (recieved[recieved.Length-1] == "B")
                    {
                        //(string tree, string address, string key, string type, string otherAddress, Boolean b
                        send(null, null, null, "T", recieved[1], Tree.validateTree(Tree.readTree(null,recieved[0],null), recieved[1]));
                        Tree.globalTree = Tree.readTree(null, recieved[0], null);
                        string st = ((Node)(((Node)(((Node)(Tree.globalTree[recieved[1]])).getConnected()[recieved[1]])).getConnected()["Directory"])).getValueHash();
                        retrieveContent(st, recieved[1]);

                    }
                    else if (recieved[recieved.Length-1] == "S")
                    {
                        using (StreamWriter sw = File.AppendText(getPath("\\Tree\\" + user.getPublicHash() + "\\Key.cab")))
                        {
                            sw.WriteLine(recieved[0]+"/"+recieved[1]);
                        }
                    }
                    else if (recieved[recieved.Length-1] == "T")
                    {

                        if (recieved[0] == "false")
                        {
                            count--;
                        }
                        else
                        {
                            count++;
                        }
                    }
                    else
                    {
                        // System.IO.File.ReadAllText(getPath("Tree\\global.tree")) + "_" + user.getPublicHash() + "_" + type

                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            listener.Close();
        }
        public static void retrieveContent(string dirHash, string refrenceHash)
        {
            string[] lines = { "cd " + System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\Tree\\" + refrenceHash, "ipfs get " + dirHash };
            System.IO.File.WriteAllLines(System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\Script\\" + "pull.bat", lines);
            Process proc = null;
            proc = new Process();
            proc.StartInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\Script\\";
            proc.StartInfo.FileName = "pull.bat";
            //proc.StartInfo.CreateNoWindow = true;
            proc.Start();
            proc.WaitForExit();

            string[] hash = System.IO.File.ReadAllText(System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\Tree\\" + refrenceHash + "\\" + dirHash + "\\Directory.cab").Split('\n');
            for (int i = 0; i < hash.Length; i++)
            {
                if (!Directory.Exists(System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\Tree\\" + refrenceHash + hash[i]))
                {
                    string[] lines1 = { "cd " + System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\Tree\\" + refrenceHash, "ipfs get " + hash[i] };
                    System.IO.File.WriteAllLines(System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\Script\\" + "pull.bat", lines1);
                    proc = new Process();
                    proc.StartInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\Script\\";
                    proc.StartInfo.FileName = "pull.bat";
                    //proc.StartInfo.CreateNoWindow = true;
                    proc.Start();
                    proc.WaitForExit();
                }

            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog fdlg = new OpenFileDialog();
            fdlg.Title = "Select file";
            fdlg.InitialDirectory = @"c:\";
            fdlg.Filter = "All Files(*.*)|*.*";
            fdlg.FilterIndex = 1;
            fdlg.RestoreDirectory = true;
            string sFileName = "";
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                sFileName = fdlg.FileName;

                string[] arrAllFiles = fdlg.FileNames; //used when Multiselect = true      
            }
            string dirHash = addIPFS(sFileName);
            //user.userTree.getNode(user.getPublicHash()).getNode("Directory")

            using (StreamWriter sw = File.AppendText(getPath("\\Tree\\"+userPicked+"\\Directory.cab")))
            {
                sw.WriteLine(dirHash);
            }



        }
        public static string addIPFS(string sFileName)
        {
            string[] lines = { "cd " + sFileName.Substring(0, sFileName.LastIndexOf("\\")), "ipfs add -w " + sFileName.Substring(sFileName.LastIndexOf("\\") + 1, sFileName.Length - sFileName.LastIndexOf("\\") - 1) + " > added.cab" };
            System.IO.File.WriteAllLines(System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\Script\\" + "add.bat", lines);
            Process proc = null;
            proc = new Process();


            proc.StartInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\Script\\";
            proc.StartInfo.FileName = "add.bat";
            //proc.StartInfo.CreateNoWindow = true;
            proc.Start();
            proc.WaitForExit();

            string[] hash = System.IO.File.ReadAllText(sFileName.Substring(0, sFileName.LastIndexOf("\\")) + "\\added.cab").Split(' ');

            return(hash[hash.Length - 2]);
        }
        public static string getPath(string s)
        {
            return System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath)+"\\" + s;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            user = new User(textBox1.Text, textBox2.Text);
            User.user = this.user;
            Tree.globalTree = Tree.readTree(getPath("Tree\\global.tree"),null,user);
            //user.userTree = (Node)(Tree.globalTree[user.getPublicHash()]);
            /*
            user.userTree.addNode(user.getPublicHash(), null);
            user.userTree.getNode(user.getPublicHash()).addNode("Directory", addIPFS(getPath("Script\\Directory.cab")));
            user.userTree.getNode(user.getPublicHash()).addNode("Key", addIPFS(getPath("Script\\Key.cab")));
            */

        }
        
        private void button3_Click(object sender, EventArgs e)
        {
            string ipfsPath = addIPFS(getPath("Tree\\" + userPicked + "\\Directory.cab"));
            //string tree, string address, string key, string type, string otherAddress, Boolean b
            send(System.IO.File.ReadAllText(getPath("Tree\\global.tree")), user.getPublicHash(), null, "B", null, false);
            user.userTree.getNode(userPicked).getNode("Directory").changeVal(ipfsPath);
            ((Node)Tree.globalTree[userPicked]).getNode(user.getPublicHash());//.getNode("Directory").changeVal(user.encryptHash(ipfsPath));
            System.IO.File.WriteAllText(getPath("Tree\\global.tree"), Tree.exportTree());


        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                userPicked = "a";

            }
            else
            {
                userPicked = "b";

            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                userPicked = "a";

            }
            else
            {
                userPicked = "b";

            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(user.encryptHash("Hello Darkness my old Friend1"));
            //M/essageBox.Show(user.decryptHash(user.encryptHash("Hello Darkness my old Friend1")));
            Getter f = new Getter();
            f.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
           
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
        }
    }
}
