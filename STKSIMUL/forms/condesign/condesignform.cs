using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace STKSIMUL
{
    public partial class condesignform : Form
    {
        private Process pr2;
        public condesignform()
        {
            InitializeComponent();
        }
        private void condesignform_Load(object sender, EventArgs e)
        {
            numericUpDown1.Value = (decimal)0.5;
            numericUpDown2.Value = (decimal)0.5;
            numericUpDown3.Value = (decimal)90;
            numericUpDown4.Value = (decimal)10;
            numericUpDown5.Value = (decimal)1000;
            numericUpDown6.Value = (decimal)6;
            numericUpDown7.Value = (decimal)400;
        }
        private void button1_Click(object sender, EventArgs e)
        {

            pr2 = new Process();
            pr2.StartInfo = new ProcessStartInfo(Application.StartupPath + "\\condesign.exe",
                Application.StartupPath + " " +
                numericUpDown1.Value.ToString() + " " +
                numericUpDown2.Value.ToString() + " " +
                numericUpDown3.Value.ToString() + " " +
                numericUpDown4.Value.ToString() + " " +
                numericUpDown5.Value.ToString() + " " +
                ((double)numericUpDown6.Value*1e9).ToString() + " " +
                ((double)numericUpDown7.Value * 1e6).ToString()
                );
           
            //pr2.StartInfo = new ProcessStartInfo(Application.StartupPath + "\\condesign.exe",Application.StartupPath+" 2.3 1.1 10 11");
            pr2.StartInfo.UseShellExecute = true;
            pr2.Start();

            //pr2 = Process.Start(Application.StartupPath + "\\condesign.exe");
            timer2.Enabled = true;
            button1.Enabled = false;

        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if ((pr2 != null) && (pr2.HasExited == true))
            {
                timer2.Enabled = false;
                button1.Enabled = true;

                StreamReader sr = new StreamReader(Application.StartupPath + "\\result.txt", Encoding.Default);
                String line;
                int linecount = 1;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] sarray = line.Split(new string[1] {"\t" }, StringSplitOptions.RemoveEmptyEntries);
                    
                    if(sarray.Length==7)
                    {
                        ListViewItem itemtmp = new ListViewItem(linecount.ToString());
                        linecount++;
                        for (int ii = 0; ii < 7; ii++)
                        {
                            double tmp = double.Parse(sarray[ii]);
                            /*
                            if (ii == 3)
                            {
                                tmp = tmp / 2.4;
                            }
                            else if(ii==4)
                            {
                                tmp = tmp *12;
                                if(tmp<0.3)
                                {
                                    tmp += 0.2;
                                }
                            }

                            if ((linecount == 5) && (ii == 4))
                            {
                                tmp /= 2;
                            }

                            */


                            itemtmp.SubItems.Add(tmp.ToString());
                        }

                        listView1.Items.Add(itemtmp);
                    }

                }
                sr.Close();

                //Process.Start("notepad.exe", Application.StartupPath + "\\result.txt");

                pr2.Dispose();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
