using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.Drawing.Imaging;
using System.IO;


namespace STKSIMUL
{

    public partial class imgsimform : Form
    {
        private string rawname;
        private string info;

        private Bitmap resultBitmap;
        RectangleF sourceRectange;
        Point lastMouseDown;
        Point lastMouseMove;
        public Bitmap ResultBMP
        {
            get { return resultBitmap; }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            this.lastMouseDown = this.lastMouseMove = e.Location;

        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!pictureBox1.Capture) return;
            //DrawReversibleFrame();
            lastMouseMove = e.Location;
            DrawReversibleFrame();
            
            
        }
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
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
                this.sourceRectange = new RectangleF(PointF.Empty, resultBitmap.Size);
            }

            this.Invalidate(true);
        }
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (Math.Abs(sourceRectange.Width * sourceRectange.Height) < 1) sourceRectange = new RectangleF(PointF.Empty, resultBitmap.Size);
            e.Graphics.DrawImage(resultBitmap, (RectangleF)ClientRectangle, sourceRectange, GraphicsUnit.Pixel);
        }

        private void DrawReversibleFrame()
        {
            Rectangle rect = this.RectangleToScreen(Rectangle.FromLTRB(lastMouseMove.X, lastMouseMove.Y, lastMouseDown.X, lastMouseDown.Y));
            //ControlPaint.DrawReversibleFrame(rect, Color.Red, FrameStyle.Dashed);
         
        }

        public imgsimform(string rn,string imginfo)
        {

            InitializeComponent();
            rawname = rn;
            info = imginfo;
            this.Text += "-";
            this.Text += rawname;
            this.Text += "-";
            this.Text += info;
        }

        private void imgsimform_Load(object sender, EventArgs e)
        {

            BinaryReader br = new BinaryReader(File.Open(Application.StartupPath + "\\" + rawname + ".raw", FileMode.Open));
            
            byte[] pix = new byte[br.BaseStream.Length];
            for(long ii=0;ii<br.BaseStream.Length;ii++)
            {
                pix[ii] = br.ReadByte();
            }
            br.Close();
            int he, we;
            he = (int)Math.Sqrt(pix.LongLength);
            we = he;
            resultBitmap = CreateBitmap(pix, we, he);

            pictureBox1.Image = resultBitmap;
            resultBitmap.Save(Application.StartupPath + "\\" + rawname + "_" + info + ".bmp");



        }
        public static Bitmap CreateBitmap(byte[] originalImageData, int originalWidth, int originalHeight)
         {
            //指定8位格式，即256色
             Bitmap resultBitmap = new Bitmap(originalWidth, originalHeight, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);

            //将该位图存入内存中
             MemoryStream curImageStream = new MemoryStream();
             resultBitmap.Save(curImageStream, System.Drawing.Imaging.ImageFormat.Bmp);
             curImageStream.Flush();

            //由于位图数据需要DWORD对齐（4byte倍数），计算需要补位的个数
            int curPadNum = ((originalWidth * 8 + 31) / 32 * 4) - originalWidth;

            //最终生成的位图数据大小
            int bitmapDataSize = ((originalWidth * 8 + 31) / 32 * 4) * originalHeight;

            //数据部分相对文件开始偏移，具体可以参考位图文件格式
            int dataOffset = ReadData(curImageStream, 10, 4);


            //改变调色板，因为默认的调色板是32位彩色的，需要修改为256色的调色板
            int paletteStart = 54;
            int paletteEnd = dataOffset;
            int color = 0;

            for (int i = paletteStart; i < paletteEnd; i += 4)
             {
                byte[] tempColor = new byte[4];
                 tempColor[0] = (byte)color;
                 tempColor[1] = (byte)color;
                 tempColor[2] = (byte)color;
                 tempColor[3] = (byte)0;
                 color++;

                 curImageStream.Position = i;
                 curImageStream.Write(tempColor, 0, 4);
             }

            //最终生成的位图数据，以及大小，高度没有变，宽度需要调整
            byte[] destImageData = new byte[bitmapDataSize];
            int destWidth = originalWidth + curPadNum;

            //生成最终的位图数据，注意的是，位图数据 从左到右，从下到上，所以需要颠倒
            for (int originalRowIndex = originalHeight - 1; originalRowIndex >= 0; originalRowIndex--)
             {
                int destRowIndex = originalHeight - originalRowIndex - 1;

                for (int dataIndex = 0; dataIndex < originalWidth; dataIndex++)
                 {
                    //同时还要注意，新的位图数据的宽度已经变化destWidth，否则会产生错位
                     destImageData[destRowIndex * destWidth + dataIndex] = originalImageData[originalRowIndex * originalWidth + dataIndex];
                 }
             }


            //将流的Position移到数据段   
             curImageStream.Position = dataOffset;

            //将新位图数据写入内存中
             curImageStream.Write(destImageData, 0, bitmapDataSize);

             curImageStream.Flush();

            //将内存中的位图写入Bitmap对象
             resultBitmap = new Bitmap(curImageStream);

            return resultBitmap;
         }
        public static int ReadData(MemoryStream curStream, int startPosition, int length)
         {
            int result = -1;

            byte[] tempData = new byte[length];
             curStream.Position = startPosition;
             curStream.Read(tempData, 0, length);
             result = BitConverter.ToInt32(tempData, 0);

            return result;
         }

        /// <summary>
        /// 向内存流中指定位置，写入数据
        /// </summary>
        /// <param name="curStream"></param>
        /// <param name="startPosition"></param>
        /// <param name="length"></param>
        /// <param name="value"></param>
        public static void WriteData(MemoryStream curStream, int startPosition, int length, int value)
         {
             curStream.Position = startPosition;
             curStream.Write(BitConverter.GetBytes(value), 0, length);
         }

    }
}
