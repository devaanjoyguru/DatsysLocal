<%@ Page Title="" Language="C#" MasterPageFile="~/PostLoginDefault.Master" AutoEventWireup="true" CodeBehind="ViewRegressionJobs.aspx.cs" Inherits="DATSYS.TradingModel.RegressionWebClient.Application.ViewRegressionJobs" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
      <div>
          <div style="text-align: center; padding: 10px;">
          <telerik:RadButton runat="server" ID="btnrefresh" Text="Refresh" Skin="MetroTouch"></telerik:RadButton>
      </div>
      <telerik:RadGrid ID="RegressionJobs" runat="server" AutoGenerateColumns="True" Skin="MetroTouch">
      </telerik:RadGrid>
        
        </div>
      
</asp:Content>
