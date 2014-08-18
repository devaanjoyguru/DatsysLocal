<%@ Page Title="" Language="C#" MasterPageFile="~/PostLoginDefault.Master" AutoEventWireup="true" CodeBehind="AddToRegression.aspx.cs" Inherits="DATSYS.TradingModel.RegressionWebClient.Application.AddToRegression" %>
<%@ Register assembly="Telerik.Web.UI" namespace="Telerik.Web.UI" tagprefix="telerik" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
   
    <div>
        <table style="width:95%" border="1">
            <tr>
                
                <td>Enter a name or description </td>
                <td>
                    <telerik:RadTextBox runat="server" ID="txtRegressionName" Skin="MetroTouch" Width="300px">
                        
                    </telerik:RadTextBox>
                </td>
            </tr>
            
            <tr>
                <td>Select Instrument code</td>
                <td>
                    
                   
                    <telerik:RadComboBox ID="cmbInstrumentCode" Runat="server" Skin="MetroTouch">
                        <Items>
                            <telerik:RadComboBoxItem runat="server" Text="FGBL" Value="FGBL"/>
                            <telerik:RadComboBoxItem runat="server" Text="FGBM" Value="FGBM"/>
                        </Items>
                    </telerik:RadComboBox>
                </td>
            </tr>
            <tr>
                <td>Select Strategy</td>
                <td>
                   
                    <telerik:RadComboBox ID="cmbStrategy" Runat="server" Skin="MetroTouch" Width="300px">
                        <Items>
                            <telerik:RadComboBoxItem runat="server" Text="Strategy 7 Bar" Value="StrategySevenBar"/>
                            <telerik:RadComboBoxItem runat="server" Text="Strategy Daily 7 Bar" Value="StrategyDailySevenBar"/>
                        </Items>
                    </telerik:RadComboBox>
                </td>
            </tr>
            <tr>
                <td>Select Regression dates</td>
                <td>
                    <div style="float: left;">
                    Start date
                    
                        <telerik:RadDatePicker runat="server" Skin="MetroTouch" ID="startDate"></telerik:RadDatePicker>
                    </div>
                    <div style="float: left;">
                    End date
                   
                        <telerik:RadDatePicker runat="server" Skin="MetroTouch" ID="endDate"></telerik:RadDatePicker>
                    </div>
                </td>
            </tr>
            <tr>
                <td> Select Environment(s)</td>
                <td> Micro
                     <telerik:RadComboBox runat="server" ID="cmbMicroStrategy" Skin="MetroTouch" Width="250px">
                         <Items>
                             <telerik:RadComboBoxItem runat="server" Text="Micro Environment" Value="Micro"/>
                             
                         </Items>
                     </telerik:RadComboBox>
                    Macro
                     <telerik:RadComboBox runat="server" ID="cmbMacroStrategy" Skin="MetroTouch" Width="250px">
                         <Items>
                             <telerik:RadComboBoxItem runat="server" Text="Macro Environment" Value="Macro"/>
                             
                         </Items>
                     </telerik:RadComboBox>
                </td>
            </tr>
            <tr>
                <td>Select Bar Interval</td>
                <td>
                    <telerik:RadNumericTextBox runat="server" ID="barInterval" Skin="MetroTouch" MinValue="1"></telerik:RadNumericTextBox>
                   
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
                    <telerik:RadButton runat="server" ID="btnaddregression" Text="Start Regression" Skin="MetroTouch"></telerik:RadButton>
                   
                </td>
                
            </tr>
        </table>
        <br/><br/>
        <asp:Literal runat="server" ID="literalMsg"></asp:Literal>
    </div>
       
</asp:Content>
