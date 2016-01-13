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
    public partial class STK2D : Form
    {
        private static STK2D defSTK2D;

        /// <summary>
        /// 将默认的构造函数私有化
        /// 实现单例模式
        /// </summary>
        /// <returns></returns>
        public static STK2D createform()
        {
            if ((defSTK2D == null) || (defSTK2D.IsDisposed == true))
            {
                defSTK2D = new STK2D();

            }
            return defSTK2D;
        }

        private STK2D()
        {
            InitializeComponent();
            //this.axAgUiAx2DCntrl1.Application.ExecuteCommand("New / Scenario Test"); 
        }

    }
}
