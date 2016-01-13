using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AGI.STKX;
using System.Collections;



namespace STKSIMUL
{
    public partial class cmdform : Form
    {
        private static cmdform defcmdfrom;
        private static AgSTKXApplication rootapp;
        private static ArrayList cmdhistory;
        private static int p;

        private SARSYS mysarsys;

        /// <summary>
        /// 将默认的构造函数私有化
        /// 实现单例模式
        /// </summary>
        /// <returns></returns>
        public static cmdform createform()
        {
            if ((defcmdfrom == null) || (defcmdfrom.IsDisposed == true))
            {
                defcmdfrom = new cmdform();

            }
            return defcmdfrom;
        }
        static cmdform()
        {
            rootapp = new AgSTKXApplication();
            cmdhistory = new ArrayList();
            p = 0;
            
        }
        private cmdform()
        {
            InitializeComponent();
            mysarsys = SARSYS.createsys();
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {

                IAgExecCmdResult resultmsg;
                cmdhistory.Add(textBox2.Text);

                if (cmdhistory.Count > 100)//最大储存100条历史指令
                    cmdhistory.RemoveAt(0);

                p = cmdhistory.Count;
                textBox1.AppendText("\r\n>>" + textBox2.Text + "\r\n");
                try
                {
                    switch(textBox2.Text)
                    {
                            //可以添加一些自定义指令
                        case "clc"://清屏
                            textBox1.Text = "";
                            break;
                        case "help"://帮助
                            textBox1.AppendText("hello\r\n");
                            break;

                        case "tmpcmd":
                            DateTime dt = DateTime.Now;

                            string st = dt.ToString("r");
                            
                            textBox1.AppendText(st+"\r\n");

                            st = st.Substring(5,20)+".00";

                            textBox1.AppendText(st + "\r\n");



                            //////////////////
                            dt = dt.AddDays(-10);

                            st = dt.ToString("r");
                            
                            textBox1.AppendText(st+"\r\n");

                            st = st.Substring(5,20)+".00";


                            /////////////////
                            textBox1.AppendText(st + "\r\n");

                            dt = DateTime.Parse(st);

                            textBox1.AppendText(dt.ToString() + "\r\n");



                            break;

                        case "playf":
                            mysarsys.playf();
                            break;
                        case "playb":
                            mysarsys.playb();
                            break;
                        case "playp":
                            mysarsys.playp();
                            break;
                        default:
                            resultmsg = rootapp.ExecuteCommand(textBox2.Text);
                            textBox1.AppendText(resultmsg.IsSucceeded.ToString() + "\r\n");
                            for (int ii=0; ii < resultmsg.Count;ii++ )
                            {
                                textBox1.AppendText(resultmsg[ii]+"\r\n");

                            }
                            break;
                    }

                }
                catch
                {
                    textBox1.AppendText("cmdfailed\r\n");
                }

                textBox2.Text = "";
            }
            else if (e.KeyCode == Keys.Up)
            {
                if(p>0)
                {
                    p--;
                    textBox2.Text = cmdhistory[p].ToString();
                    
                }
            }
            else if (e.KeyCode == Keys.Down)
            {
                if(p<cmdhistory.Count-1)
                {
                    p++;
                    textBox2.Text = cmdhistory[p].ToString();
                    
                }
                else
                {
                    p = cmdhistory.Count;
                    textBox2.Text = "";
                }
            }
        }

        private void cmdform_Activated(object sender, EventArgs e)
        {
            this.textBox2.Focus();
        }

        private void cmdform_Enter(object sender, EventArgs e)
        {
            this.textBox2.Focus();
        }

    }
}
