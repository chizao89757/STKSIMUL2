using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace STKSIMUL
{
    public partial class staticpara : Form
    {
        private static staticpara defstaticpara;
        private SARSYS mysarsys;
        /// <summary>
        /// 将默认的构造函数私有化
        /// 实现单例模式
        /// </summary>
        /// <returns></returns>
        public static staticpara createform()
        {
            if ((defstaticpara == null) || (defstaticpara.IsDisposed == true))
            {
                defstaticpara = new staticpara();
            }
            return defstaticpara;
        }


        /// <summary>
        /// 生成函数
        /// </summary>
        private staticpara()
        {
            InitializeComponent();
            mysarsys = SARSYS.createsys();
            //设定默认参数
            button2_Click(null,null);


        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

            switch (treeView1.SelectedNode.Name)
            {
                case "n00"://仿真时间


                    this.groupBox6.Location = new System.Drawing.Point(178, 12);
                    groupBox1.Visible = false;
                    groupBox2.Visible = false;
                    groupBox3.Visible = false;
                    groupBox5.Visible = false;
                    groupBox6.Visible = true;
                    break;

                case "n10" ://发射站轨道


                    this.groupBox2.Location = new System.Drawing.Point(178, 12);
                    groupBox1.Visible = false;
                    groupBox2.Visible = true ;
                    groupBox3.Visible = false;
                    groupBox5.Visible = false ;
                    groupBox6.Visible = false;
                    break;
                case "n11"://发射站雷达参数
                    this.groupBox3.Location = new System.Drawing.Point(178, 12);
                    groupBox1.Visible = false;
                    groupBox2.Visible = false ;
                    groupBox3.Visible = true;
                    groupBox5.Visible = false ;
                    groupBox6.Visible = false;
                    break;
                case "n20"://接收站轨迹
                    this.groupBox1.Location = new System.Drawing.Point(178, 12);
                    groupBox1.Visible = true;
                    groupBox2.Visible = false ;
                    groupBox3.Visible = false;
                    groupBox5.Visible = false ;
                    groupBox6.Visible = false;
                    break;
                case "n21"://接收站雷达参数
                    this.groupBox5.Location = new System.Drawing.Point(178, 12);
                    groupBox1.Visible = false;
                    groupBox2.Visible = false ;
                    groupBox3.Visible = false;
                    groupBox5.Visible = true;
                    groupBox6.Visible = false;
                    break;
                default:
                    groupBox1.Visible = false;
                    groupBox2.Visible = false ;
                    groupBox3.Visible = false;
                    groupBox5.Visible = false;
                    groupBox6.Visible = false;
                    break;

            }
        }

        /// <summary>
        /// 确定
        /// 将参数应用到系统中去
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            mysarsys.SaEccentricity = (double)numericUpDown1.Value;
            mysarsys.Sainclination = (double)numericUpDown2.Value;
            mysarsys.SaSemiMajorAxis = (double)numericUpDown4.Value;
            mysarsys.SaLongAscendingNode = (double)numericUpDown3.Value;
            mysarsys.SaArgOfPerigee = (double)numericUpDown5.Value;

            mysarsys.RaElevation = (double)numericUpDown15.Value;


            if (DateTime.Compare(dateTimePicker1.Value, dateTimePicker2.Value)>0)
            {
                dateTimePicker1.Value = dateTimePicker2.Value;
            }

            if (DateTime.Compare(dateTimePicker1.Value, mysarsys.SimStopTime) > 0)
            {
                mysarsys.SimStopTime = dateTimePicker2.Value;
                mysarsys.SimStartTime = dateTimePicker1.Value;
            }
            else
            {
                mysarsys.SimStartTime = dateTimePicker1.Value;
                mysarsys.SimStopTime = dateTimePicker2.Value;
            }

           


            string[] sarray = textBox1.Text.Split(new string[2] { ",", "\r\n" },StringSplitOptions.RemoveEmptyEntries);
            double[] varray = new double[sarray.Length];
            for(int ii=0;ii<sarray.Length;ii++)
            {
                varray[ii] = double.Parse(sarray[ii]);
            }
            mysarsys.PlWayPoints = varray;

        }
        /// <summary>
        /// 恢复默认
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            //设定默认参数

            numericUpDown1.Value = (decimal)mysarsys.SaEccentricity;
            numericUpDown2.Value = (decimal)mysarsys.Sainclination;
            numericUpDown4.Value = (decimal)mysarsys.SaSemiMajorAxis;
            numericUpDown3.Value = (decimal)mysarsys.SaLongAscendingNode;
            numericUpDown5.Value = (decimal)mysarsys.SaArgOfPerigee;

            numericUpDown15.Value = (decimal)mysarsys.RaElevation;
            textBox1.Text="";

            for (int ii = 0; ii < mysarsys.PlWayPoints.Length;ii+=5 )
            {
                textBox1.Text += mysarsys.PlWayPoints[ii].ToString() + ","
                    + mysarsys.PlWayPoints[ii + 1].ToString() + ","
                    + mysarsys.PlWayPoints[ii + 2].ToString() + ","
                    + mysarsys.PlWayPoints[ii + 3].ToString() + ","
                    + mysarsys.PlWayPoints[ii + 4].ToString() + "\r\n";

            }
                
            dateTimePicker1.Value = mysarsys.SimStartTime;
            dateTimePicker2.Value = mysarsys.SimStopTime;

        }



    }
}
