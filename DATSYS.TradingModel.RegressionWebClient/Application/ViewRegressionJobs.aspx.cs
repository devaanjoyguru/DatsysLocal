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
        protected void Page_Load(object sender, EventArgs e)
        {
            gridRegressionJobs.DataSource = DataManager.GetRegressionJobs();
            gridRegressionJobs.DataBind();
        }
    }
}