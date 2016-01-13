using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Diagnostics;
//using AGI.STKX;


namespace STKSIMUL
{

    public partial class mainform : Form
    {
        #region 杂

        #region 一些成员/属性
        private string scfilepathstr = null;
        private SARSYS mysarsys;

        
        

        #region 一些子窗口
        //拿着这些窗口的句柄，在仿真时需要改变其enable属性
        private staticpara setparaform;
        private simform simf;
        
        private STK2D STK2Dform;
        private STK3D STK3Dform;

        Image backpic;
        #endregion

        #endregion

        #region 主窗口相关

        /// <summary>
        /// 构造方法
        /// </summary>
        public mainform()
        {
            InitializeComponent();
            mysarsys = SARSYS.createsys();
            mysarsys.ScPathChange += refreshScenario;
            

        }
        private void mainform_Load(object sender, EventArgs e)
        {
            dToolStripMenuItem_Click(sender, e);
            dToolStripMenuItem1_Click(sender, e);
            backpic = Image.FromFile(Application.StartupPath + "\\backgroundpic.jpg");
            this.BackgroundImage = backpic;
            this.BackgroundImageLayout = ImageLayout.Stretch;
        }
        private void mainform_SizeChanged(object sender, EventArgs e)
        {
            this.BackgroundImage = null;
            this.Invalidate();
            this.BackgroundImage = backpic;
            this.Invalidate();
        }
        /// <summary>
        /// 关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mainform_FormClosing(object sender, FormClosingEventArgs e)
        {
            STK2Dform.Close();
            STK3Dform.Close();
            mysarsys.unloadsc();
            
        }

        #endregion

        #region sarsys相关事件处理函数
        /// <summary>
        /// 新的工程，需要进行一些更新工作
        /// </summary>
        private void refreshScenario()
        {
            this.Text = "SAR综合仿真演示系统   " + mysarsys.ScPath;

            //MessageBox.Show(scPath);
        }
        #endregion


        #endregion


        #region 菜单栏

        #region 菜单栏操作
        /// <summary>
        /// 菜单状态枚举体
        /// </summary>
        enum menustate
        {
            no_open_file = 0,
            edit_file = 1,
            start_demo = 2,

        }
        /// <summary>
        /// 根据软件状态更改菜单栏各项属性（主要是enable）
        /// </summary>
        /// <param name="state"></param>
        private void menustatechange(menustate state)
        {
            switch (state)
            {
                case menustate.no_open_file:

                    break;
                case menustate.edit_file:

                    break;
                case menustate.start_demo:

                    break;
                default:

                    break;
            }
        }
        #endregion

        #region 文件
        private void 新建NToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mysarsys.newsc();
            mysarsys.newsarsys();
            scfilepathstr = null;
        }

        private void 打开OToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = Application.StartupPath;
            openFileDialog1.Filter = "scenario files (*.sc)|*.sc|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.Multiselect = false;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                scfilepathstr = openFileDialog1.FileName;

                //打开scfilepathstr目录处的工程文件
                mysarsys.loadsc(scfilepathstr);
                if(mysarsys.checkifdefsys()==false)//未检测到存在sarsys
                {
                    DialogResult dr = MessageBox.Show("在当前打开的scenario中未检测到SARSYS，是否在当前打开的scenario中新建一个？否则将关闭此scenario", "未检测到SARSYS！",MessageBoxButtons.YesNo,MessageBoxIcon.Question);
                    if(dr==DialogResult.Yes)
                    {
                        mysarsys.newsarsys();
                    }
                    else
                    {
                        mysarsys.unloadsc();
                    }
                }
                else
                {
                    mysarsys.readsarsys();
                }
            }


        }

        private void 保存SToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (scfilepathstr == null)//第一次保存需指定目录
            {
                SaveFileDialog SaveFileDialog1 = new SaveFileDialog();
                SaveFileDialog1.InitialDirectory = Application.StartupPath;
                SaveFileDialog1.Filter = "scenario files (*.sc)|*.sc|All files (*.*)|*.*";
                SaveFileDialog1.FilterIndex = 1;
                if (SaveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    
                    scfilepathstr = SaveFileDialog1.FileName;
                
                    
                    //保存到scfilepathstr下
                    mysarsys.savesc(scfilepathstr);
                   
                }
            }
            else
            {
                //直接保存到scfilepathstr下
                mysarsys.savesc(scfilepathstr);
            }
        }

        private void 另存为AToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //改变默认工作目录
            SaveFileDialog SaveFileDialog1 = new SaveFileDialog();
            SaveFileDialog1.InitialDirectory = Application.StartupPath;
            SaveFileDialog1.Filter = "scenario files (*.sc)|*.sc|All files (*.*)|*.*";
            SaveFileDialog1.FilterIndex = 1;
            if (SaveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                scfilepathstr = SaveFileDialog1.FileName;

                mysarsys.savesc(scfilepathstr);
            }
        }

        private void 退出XToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.Exit();
        }




        #endregion

        #region 编辑


        #endregion

        #region 工具
        private void 命令窗口ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cmdform mycmd = cmdform.createform();
            mycmd.Show();
            mycmd.Focus();
        }
        #endregion

        #region 窗口
        /// <summary>
        /// 2D窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dToolStripMenuItem_Click(object sender, EventArgs e)
        {
            STK2Dform = STK2D.createform();
            STK2Dform.Show();
            STK2Dform.Focus();
        }
        /// <summary>
        /// 3D窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            STK3Dform = STK3D.createform();
            STK3Dform.Show();
            STK3Dform.Focus();
        }
        #endregion

        #region 构型设计
        private void 构型设计ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            condesignform f = new condesignform();
            f.Show();
        }


        #endregion

        #region 演示仿真
        private void 仿真控制ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            simf = simform.createform();
            simf.MdiParent = this;
            simf.WindowState = FormWindowState.Maximized;

            simf.Show();
            simf.Focus();
            this.Width++;
            this.Invalidate();
            this.Width--;

            
        }


        private void 模型参数修改ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setparaform = staticpara.createform();
            setparaform.MdiParent = this;
            setparaform.WindowState = FormWindowState.Maximized;

            setparaform.Show();
            setparaform.Focus();

            this.Width++;
            this.Invalidate();
            this.Width--;

            
        }
        #endregion

        #region 成像仿真

        private void 成像仿真ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            imgsimparaform ispf = new imgsimparaform();
            ispf.Show();
        }



        #endregion

        #region 帮助
        private void 关于AToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 aboutbox = new AboutBox1();
            aboutbox.ShowDialog();
        }
        private void 帮助ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        #endregion

        #endregion

    }
}
