using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DATSYS.TradingModel.DataEntityImplementation;

namespace DATSYS.TradingModel.RegressionWebClient.Application
{
    public partial class ViewRegressionJobs : System.Web.UI.Page
    {
        private DataManager _dataManager = new DataManager();

        protected void Page_Load(object sender, EventArgs e)
        {
            btnrefresh.Click += btnrefresh_Click;

            BindRegressionJobs();
        }

        private void BindRegressionJobs()
        {
            var regressionJobs = _dataManager.GetRegressionJobs();
            regressionJobs = regressionJobs.OrderByDescending(x => x.SubmittedAt).ToList();
            RegressionJobs.DataSource = regressionJobs;
            RegressionJobs.DataBind();
        }

        void btnrefresh_Click(object sender, EventArgs e)
        {
            BindRegressionJobs();
        }
    }
}