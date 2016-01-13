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
    public partial class simform : Form
    {
        private static simform defsimform;
        private SARSYS mysarsys;

        /// <summary>
        /// 将默认的构造函数私有化
        /// 实现单例模式
        /// </summary>
        /// <returns></returns>
        public static simform createform()
        {
            if ((defsimform == null) || (defsimform.IsDisposed == true))
            {
                defsimform = new simform();
            }
            return defsimform;
        }


        /// <summary>
        /// 生成函数
        /// </summary>
        private simform()
        {
            InitializeComponent();
            mysarsys = SARSYS.createsys();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
                mysarsys.playp();

            
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
                mysarsys.playf();
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked)
                mysarsys.playb();
        }

        private void simform_FormClosing(object sender, FormClosingEventArgs e)
        {
            mysarsys.playp();
        }

        private void simform_Load(object sender, EventArgs e)
        {
            radioButton1.Checked = true;
            numericUpDown1.Value = (decimal)mysarsys.SimStep;

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            mysarsys.SimStep = (double)numericUpDown1.Value;
        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton6.Checked)
                mysarsys.focusearth();
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton5.Checked)
                mysarsys.focussate();
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton4.Checked)
                mysarsys.focusplane();
        }

    }
}
