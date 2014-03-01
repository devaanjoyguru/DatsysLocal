using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.UI;

namespace DATSYS.TradingModel.RegressionClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
        }

        void Form1_Load(object sender, EventArgs e)
        {
            
            DrawBars();
        }

        private void DrawBars()
        {
            
            for (int i = 0; i < 100; i++)
            {
                radChartView1.Series[0].DataPoints.Add(i);    
            }
            ChartPanZoomController zoomController = new ChartPanZoomController();
            zoomController.PanZoomMode = ChartPanZoomMode.Horizontal;
            radChartView1.Controllers.Add(zoomController);
            radChartView1.ShowPanZoom = true;
            radChartView1.Zoom(3,1);
        }
    }
}
