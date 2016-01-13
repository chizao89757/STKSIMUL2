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
    public partial class STK3D : Form
    {
        private static STK3D defSTK3D;

        /// <summary>
        /// 将默认的构造函数私有化
        /// 实现单例模式
        /// </summary>
        /// <returns></returns>
        public static STK3D createform()
        {
            if ((defSTK3D == null) || (defSTK3D.IsDisposed == true))
            {
                defSTK3D = new STK3D();
            }
            return defSTK3D;

        }
        private STK3D()
        {
            InitializeComponent();
          
        }

        private void axAgUiAxVOCntrl1_OLEDragDrop(object sender, AxAGI.STKX.IAgUiAxVOCntrlEvents_OLEDragDropEvent e)
        {

        }

    }
}
