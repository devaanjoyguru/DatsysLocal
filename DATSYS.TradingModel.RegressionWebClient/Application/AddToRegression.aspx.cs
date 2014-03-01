using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DATSYS.TradingModel.DataEntityImplementation;

namespace DATSYS.TradingModel.RegressionWebClient.Application
{
    public partial class AddToRegression : System.Web.UI.Page
    {
        

        protected void Page_Load(object sender, EventArgs e)
        {
            btnadd.Click += btnadd_Click;
            calStartDate.TodaysDate=new DateTime(2012,2,1);
            calEndDate.TodaysDate=new DateTime(2012,2,10);
        }

        void btnadd_Click(object sender, EventArgs e)
        {
            try
            {

            string instrumentCode = ddInstrumentCode.SelectedValue;
            DateTime startDate = calStartDate.SelectedDate.Date;
            DateTime endDate = calEndDate.SelectedDate.Date;
            string strategyName = ddStrategy.SelectedValue;
            int barInterval = Convert.ToInt32(txtBarInterval.Text);
            bool isdaily = chkIsDaily.Checked;

           var jobId=  DataManager.AddRegressionJob(instrumentCode,startDate,endDate,barInterval,strategyName,isdaily);

                literalMsg.Text = string.Format("<strong> The regression job is saved. </strong>", jobId);

            }
            catch (Exception exception)
            {
                literalMsg.Text =string.Format("<strong><font style='color:red'>ERROR: {0} </font></strong>", exception);
            }
        }
    }
}