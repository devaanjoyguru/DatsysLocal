using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DATSYS.TradingModel.RegressionWebClient
{
    public partial class Login : System.Web.UI.Page
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {
            loginctrl.Authenticate += loginctrl_Authenticate;
        }

        void loginctrl_Authenticate(object sender, AuthenticateEventArgs e)
        {
            e.Authenticated = true;
            Response.Redirect("Application/AddToRegression.aspx");
        }
    }
}