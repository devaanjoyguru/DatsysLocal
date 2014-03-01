<%@ Page Title="" Language="C#" MasterPageFile="~/PostLoginDefault.Master" AutoEventWireup="true" CodeBehind="AddToRegression.aspx.cs" Inherits="DATSYS.TradingModel.RegressionWebClient.Application.AddToRegression" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div>
        <table style="width:95%" border="1">
            <tr>
                <td>Select Instrument code</td>
                <td>
                    <asp:DropDownList runat="server" ID="ddInstrumentCode">
                        <Items>
                            <asp:ListItem Text="FGBL" Value="FGBL" Selected="True"></asp:ListItem>
                            <asp:ListItem Text="FGBM" Value="FGBM"></asp:ListItem>
                        </Items>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>Select Strategy</td>
                <td>
                    <asp:DropDownList runat="server" ID="ddStrategy">
                        <Items>
                            <asp:ListItem Text="StrategySevenBar" Value="StrategySevenBar" Selected="True"></asp:ListItem>
                            <asp:ListItem Text="StrategyDailySevenBar" Value="StrategyDailySevenBar"></asp:ListItem>
                        </Items>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>Select Regression dates</td>
                <td>
                    <div style="float: left;">
                    Start date
                    <asp:Calendar runat="server" ID="calStartDate"></asp:Calendar>
                    </div>
                    <div style="float: right;">
                    End date
                    <asp:Calendar runat="server" ID="calEndDate"></asp:Calendar>
                    </div>
                </td>
            </tr>
            <tr>
                <td>Select Bar Interval</td>
                <td>
                    <asp:TextBox runat="server" TextMode="SingleLine" Text="30" ID="txtBarInterval"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>Is Daily Strategy</td>
                <td>
                    <asp:CheckBox runat="server" ID="chkIsDaily"/>
                </td>
            </tr>
            <tr>
                <td colspan="2" style="text-align: center; padding: 10px;">
                    <asp:Button runat="server" ID="btnadd" Text="Add to regression"/>
                </td>
                
            </tr>
        </table>
        <br/><br/>
        <asp:Literal runat="server" ID="literalMsg"></asp:Literal>
    </div>
</asp:Content>
