<%@ Page Title="" Language="C#" MasterPageFile="~/Speakup.master" AutoEventWireup="true" CodeFile="welcome.aspx.cs" Inherits="welcome" %>
<%-- Modified: 02/12/2010 --%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <style type="text/css">
        .mainheader {
            text-align: center;
            font-family: Verdana, Arial, Helvetica, sans-serif; 
            color: #063366; 
            font-size: 1em; 
            font-weight: bold;
        }
        .tableHeader {
            text-align: center;
            font-family: Verdana, Arial, Helvetica, sans-serif;
            font-size: .8em;
            font-weight: bold;
        }
        .cellText {
            border-style: Solid; 
            border-color: LightGray;
            font: Verdana, Arial, Helvetica, sans-serif;
            font-size: .8em;
        }
        .introHeader {
            font-size: 12pt; 
            font-family: Times New Roman; 
            font: Verdana, Arial, Helvetica, sans-serif;
        }
        .divText {
            text-align: center;
            font-family: Verdana, Arial, Helvetica, sans-serif;
            font-size: .8em;       
        }
        .panelExcel {
            margin: 25px 0 0 0;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="maincontent" Runat="Server">
<asp:Label ID="lblOrgId" runat="server" Visible="false"></asp:Label>
<asp:Label ID="lblRegId" runat="server" Visible="false"></asp:Label>

<asp:Label ID="lblError" runat="server" ForeColor="Red"></asp:Label>

<h1 class="mainheader">Step 2: Choose a Speak Up Survey to Display</h1>
<div class="tableHeader">Speak Up surveys were completed for this <asp:Label ID="lblType1" runat="Server"></asp:Label>:</div>
<asp:Table id="Table1" WIDTH="500"  cellSpacing="0" cellPadding="2" align="center" BorderStyle="Solid" BorderColor="LightGray" runat="server">
<asp:TableRow ID="trSchool" runat="server">
    <asp:TableCell width="95" CssClass="cellText">School Name:</asp:TableCell>
    <asp:TableCell id="SchoolName" width="220" CssClass="cellText" >
        <asp:Label ID="lblSchoolName" runat="server"></asp:Label>
    </asp:TableCell>
</asp:TableRow>
<asp:TableRow>
    <asp:TableCell width="95" CssClass="cellText">
        District Name:
    </asp:TableCell>
    <asp:TableCell id="DistrictName" width="220" CssClass="cellText">
        <asp:Label ID="lblDistrict" runat="server"></asp:Label>
        &nbsp;
    </asp:TableCell>
</asp:TableRow>
<asp:TableRow>
    <asp:TableCell width="95" CssClass="cellText">
        City:
    </asp:TableCell>
    <asp:TableCell id="City" width="220" CssClass="cellText">
        <asp:Label ID="lblCity" runat="server"></asp:Label>
    </asp:TableCell>
</asp:TableRow>
<asp:TableRow>
    <asp:TableCell width="95" CssClass="cellText">
        State:</asp:TableCell>
    <asp:TableCell id="State" width="220" BorderStyle="Solid" BorderColor="LightGray">
        <asp:Label ID="lblState" runat="server"></asp:Label>
    </asp:TableCell>
</asp:TableRow>
<asp:TableRow>
<asp:TableCell width="95" BorderStyle="Solid" BorderColor="LightGray">
        Zip:
    </asp:TableCell>
    <asp:TableCell id="Zip" width="220" BorderStyle="Solid" BorderColor="LightGray">
        <asp:Label ID="lblZIP" runat="server"></asp:Label>
    </asp:TableCell>
</asp:TableRow>
</asp:Table>
<div class="divText">
    If this is the wrong <asp:Label ID="lblGoBack" runat="Server"></asp:Label>, return to <a href="find_school.aspx">Find
<asp:Literal ID="litGoBack" runat="Server" /></a>.
</div>

<h4 class="introHeader">
    Speak Up Surveys at Your <asp:Label ID="lblType2" runat="server"></asp:Label>
</h4>
<div style="text-align: center;">
<asp:Table ID="tblCounts" Width="500" BorderColor="#cccccc" CellPadding="2" CellSpacing="0" BorderStyle="Solid" runat="server">
</asp:Table>
</div>

    <asp:Panel ID="pnlExcel" runat="server" Visible="false" CssClass="panelExcel">
        <asp:LinkButton ID="btnExcel" Text="Download Excel Summary" OnClick="GoToExcelSummary" runat="server"></asp:LinkButton> report of your district’s school results in a side by side comparison format.
    </asp:Panel>

</asp:Content>

