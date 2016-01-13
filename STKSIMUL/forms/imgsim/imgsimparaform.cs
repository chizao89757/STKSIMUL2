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
    public partial class imgsimparaform : Form
    {
        private Bitmap mybmp;


        int mymode;//0：未加载图片 1：已加载图片可缩放 2：已点击剖面显示按钮

        int[] pointpos;

        #region 杂项
        public imgsimparaform()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region 参数设置
        /// <summary>
        /// 导入配置文件button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.InitialDirectory = Application.StartupPath;
            openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.Multiselect = false;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                loadconfigfile(openFileDialog1.FileName);

            }

        }
        /// <summary>
        /// 导出配置文件button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            SaveFileDialog SaveFileDialog1 = new SaveFileDialog();
            SaveFileDialog1.InitialDirectory = Application.StartupPath;
            SaveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            SaveFileDialog1.FilterIndex = 1;
            if (SaveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                saveasconfigfile(SaveFileDialog1.FileName);
            }
        }
        /// <summary>
        /// 点击树形图时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            switch (treeView1.SelectedNode.Name)
            {
                case "n00": //使用配置文件
                    this.groupBox3.Location = new System.Drawing.Point(156, 20);
                    groupBox3.Visible = true;
                    groupBox4.Visible = false ;
                    groupBox5.Visible = false;


                    break;

                case "n10": //平台参数设置
                    this.groupBox4.Location = new System.Drawing.Point(156, 20);
                    groupBox3.Visible = false;
                    groupBox4.Visible = true;
                    groupBox5.Visible = false;


                    break;
                case "n20": //目标设置
                    this.groupBox5.Location = new System.Drawing.Point(156, 20);
                    groupBox3.Visible = false;
                    groupBox4.Visible = false ;
                    groupBox5.Visible = true;


                    break;
                default:
                    groupBox3.Visible = false;
                    groupBox4.Visible = false ;
                    groupBox5.Visible = false;

                    break;
            }

        }

        /// <summary>
        /// 窗口load时，加载默认配置文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void imgsimparaform_Load(object sender, EventArgs e)
        {
            mymode = 0;
            loadconfigfile(Application.StartupPath + "\\defconf.txt");
        }
        /// <summary>
        /// 导出配置文件
        /// </summary>
        /// <param name="configfilepath"></param>
        private void saveasconfigfile(string configfilepath)
        {
            FileStream fs = new FileStream(configfilepath, FileMode.OpenOrCreate, FileAccess.Write);//创建写入文件 
            fs.Seek(0, SeekOrigin.Begin);
            fs.SetLength(0);

            StreamWriter sw = new StreamWriter(fs);



            sw.WriteLine("Targets=" + textBox1.Text.Replace("\r\n",";"));

            sw.WriteLine("LocationR=" + textBox2.Text);
            sw.WriteLine("f0=" + numericUpDown1.Value.ToString());
            sw.WriteLine("B=" + numericUpDown2.Value.ToString());
            sw.WriteLine("v=" + numericUpDown3.Value.ToString());
            sw.WriteLine("mode=" + comboBox1.SelectedIndex.ToString());
            sw.WriteLine("size=" + comboBox2.SelectedIndex.ToString());


            sw.Flush();
            sw.Close();
            fs.Close();

        }
        /// <summary>
        /// 导入配置文件
        /// </summary>
        /// <param name="configfilepath"></param>
        private void loadconfigfile(string configfilepath)
        {
            StreamReader sr = new StreamReader(configfilepath, Encoding.Default);
            String line;

            while ((line = sr.ReadLine()) != null)
            {
                string[] sarray = line.Split(new string[1] { "=" }, StringSplitOptions.RemoveEmptyEntries);
                if (sarray.Length == 2)
                {
                    switch (sarray[0])
                    {
                        case "Targets":
                            {
                                textBox1.Text = "";
                                string[] sarray1 = sarray[1].Split(new string[1] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                                pointpos = new int[2 * sarray1.Length];
                                for (int ii = 0; ii < sarray1.Length; ii++)
                                {
                                    textBox1.AppendText(sarray1[ii]);
                                    if (ii < sarray1.Length - 1)
                                        textBox1.AppendText("\r\n");

                                    string[] sarray2 = sarray1[ii].Split(new string[1] { "," }, StringSplitOptions.RemoveEmptyEntries);

                                    pointpos[ii * 2] = int.Parse(sarray2[0]);
                                    pointpos[ii * 2 + 1] = int.Parse(sarray2[1]);
                                }
                            }

                            break;
                        case "LocationR":
                            textBox2.Text = sarray[1];

                            break;
                        case "f0":
                            numericUpDown1.Value = (decimal)double.Parse(sarray[1]);

                            break;
                        case "B":
                            numericUpDown2.Value = (decimal)double.Parse(sarray[1]);

                            break;
                        case "mode":
                            comboBox1.SelectedIndex = int.Parse(sarray[1]);

                            break;
                        case "v":
                            numericUpDown3.Value = (decimal)double.Parse(sarray[1]);

                            break;
                        case "size":
                            comboBox2.SelectedIndex = int.Parse(sarray[1]);

                            break;
                        default:

                            break;


                    }
                }


            }
            sr.Close();
        }



        #endregion

        #region 成像仿真

        private Process pr;

        private void timer1_Tick(object sender, EventArgs e)
        {
            if ((pr != null) && (pr.HasExited == true))
            {
                timer1.Enabled = false;
                
                string mode;

                if(comboBox1.SelectedIndex==0)
                    mode="聚束";
                else
                    mode="条带";

                this.Text = "成像仿真-正在生成图像，请稍候";
                pr.Dispose();
                imgsimform isf1 = new imgsimform("Echo",mode);
                isf1.Show();
                imgsimform isf2 = new imgsimform("BpImage",mode);
                isf2.Show();
                
                button1.Enabled = true;



                mybmp = isf2.ResultBMP;
                this.pictureBox1.Image = mybmp;

                button3.Enabled = true;
                mymode = 1;
                this.Text = "成像仿真";
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {

            FileStream fs = new FileStream(Application.StartupPath + "\\conf.txt", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            //开始写入
            sw.Write("Targets="+textBox1.Text.Replace("\r\n",";")+"\r\n");

            
            sw.Write("LocationR=" + textBox2.Text + "\r\n");

            sw.Write("f0=" + numericUpDown1.Value.ToString() + "\r\n");
            sw.Write("B=" + numericUpDown2.Value.ToString() + "\r\n");
            sw.Write("v=" + numericUpDown3.Value.ToString() + "\r\n");
            sw.Write("mode=" +comboBox1.SelectedIndex.ToString() + "\r\n");


            //清空缓冲区
            sw.Flush();
            //关闭流
            sw.Close();
            fs.Close();


            pr = new Process();
            pr.StartInfo = new ProcessStartInfo(Application.StartupPath + "\\kernel.exe", Application.StartupPath + "\\conf.txt");
            pr.StartInfo.UseShellExecute = true;
            pr.Start();
            button1.Enabled = false;
            timer1.Enabled = true;
            button3.Enabled = false;
        }
        #endregion

        #region 性能评估

        RectangleF sourceRectange;
        Point lastMouseDown;
        Point lastMouseMove;

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (mymode == 1)
            {
                lastMouseMove = e.Location;
                DrawReversibleFrame();
                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    float l = sourceRectange.Left + sourceRectange.Width * lastMouseDown.X / this.ClientRectangle.Width;
                    float t = sourceRectange.Top + sourceRectange.Height * lastMouseDown.Y / this.ClientRectangle.Height;
                    float r = sourceRectange.Left + sourceRectange.Width * e.X / this.ClientRectangle.Width;
                    float b = sourceRectange.Top + sourceRectange.Height * e.Y / this.ClientRectangle.Height;
                    sourceRectange = RectangleF.FromLTRB(l, t, r, b);
                }
                else
                {
                    this.sourceRectange = new RectangleF(PointF.Empty, mybmp.Size);
                }

                this.Invalidate(true);
            }
            else if (mymode == 2)
            {
                //显示剖面

                panel1.BackgroundImage = Image.FromFile(Application.StartupPath + "\\po1.jpg");
                panel2.BackgroundImage = Image.FromFile(Application.StartupPath + "\\po2.jpg");

                button3.Text = "剖面显示";
                mymode = 1;
            }

        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (mymode == 1)
            {
                if (!pictureBox1.Capture) return;
                //DrawReversibleFrame();
                lastMouseMove = e.Location;
                DrawReversibleFrame();
            }

        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (mymode == 1)
            {
                this.lastMouseDown = this.lastMouseMove = e.Location;
            }

        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (mymode == 1)
            {
                if (Math.Abs(sourceRectange.Width * sourceRectange.Height) < 1) sourceRectange = new RectangleF(PointF.Empty, mybmp.Size);
                e.Graphics.DrawImage(mybmp, (RectangleF)ClientRectangle, sourceRectange, GraphicsUnit.Pixel);
            }

        }
        private void DrawReversibleFrame()
        {
            Rectangle rect = this.RectangleToScreen(Rectangle.FromLTRB(lastMouseMove.X, lastMouseMove.Y, lastMouseDown.X, lastMouseDown.Y));
            //ControlPaint.DrawReversibleFrame(rect, Color.Red, FrameStyle.Dashed);

        }


        //剖面显示
        private void button3_Click(object sender, EventArgs e)
        {
            mymode = 2;
            button3.Text = "剖面显示\r\n请点击选点";

        }

        #endregion






    }
}
