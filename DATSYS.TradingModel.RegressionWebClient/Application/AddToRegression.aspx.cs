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
        private DataManager _dataManager = new DataManager();

        protected void Page_Load(object sender, EventArgs e)
        {
            btnaddregression.Click += btnadd_Click;
            //calStartDate.TodaysDate=new DateTime(2012,2,1);
            //calEndDate.TodaysDate=new DateTime(2012,2,10);
        }

        void btnadd_Click(object sender, EventArgs e)
        {
            try
            {

            string instrumentCode = cmbInstrumentCode.SelectedValue;
                DateTime sDate = startDate.SelectedDate.Value; //calStartDate.SelectedDate.Date;
            DateTime eDate = endDate.SelectedDate.Value;
            string strategyName = cmbStrategy.SelectedValue;
            int barinterval = Convert.ToInt32(barInterval.Value);
            bool isdaily = chkIsDaily.Checked;
                string regressionName = txtRegressionName.Text;

           var jobId=  _dataManager.AddRegressionJob(instrumentCode,sDate,eDate,barinterval,strategyName,isdaily, regressionName);

                literalMsg.Text = string.Format("<strong> The regression job is saved. </strong>", jobId);

            }
            catch (Exception exception)
            {
                literalMsg.Text =string.Format("<strong><font style='color:red'>ERROR: {0} </font></strong>", exception);
            }
        }
    }
}