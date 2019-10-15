using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cabinet
{
    public partial class Getter : Form
    {
        private User user;
        private string pulled;
        public Getter()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            retrieveContent(textBox2.Text, textBox1.Text);
        }
        public static void retrieveContent(string dirHash, string refrenceHash)
        {
            string[] lines = { "cd "+ System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\Tree\\" + refrenceHash, "ipfs get "+dirHash};
            System.IO.File.WriteAllLines(System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\Script\\" + "pull.bat", lines);
            Process proc = null;
            proc = new Process();
            proc.StartInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\Script\\";
            proc.StartInfo.FileName = "pull.bat";
            //proc.StartInfo.CreateNoWindow = true;
            proc.Start();
            proc.WaitForExit();

            string[] hash = System.IO.File.ReadAllText(System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\Tree\\" + refrenceHash+"\\"+dirHash+"\\Directory.cab").Split('\n');
            for (int i = 0; i < hash.Length; i++)
            {
                if (!Directory.Exists(System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\Tree\\" + refrenceHash+hash[i]))
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

        private void Getter_Load(object sender, EventArgs e)
        {
            
        }
    }
}
 