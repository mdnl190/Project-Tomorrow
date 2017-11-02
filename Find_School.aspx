<%@ Page Title="" Language="C#" MasterPageFile="~/Speakup.master" AutoEventWireup="true" CodeFile="Find_School.aspx.cs" Inherits="Find_School" %>
<%-- Modified: 02/16/2010 --%>
<%-- 09/01/2017 - Changed des validators to asp validators --%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="maincontent" Runat="Server">

<asp:Label ID="lblError" runat="server" ForeColor="Red"></asp:Label>


<asp:MultiView ID="mvLookup" runat="server">
    <asp:View ID="vEntry" runat="server">
        <h3>Find Your Speak Up Survey Data</h3>
        <p>To access your Speak Up survey results, please use one of the <b>three</b> options below.</p>
        <p><b><u>2016 Preliminary Data Reports:</u></b> Please note that this data has not yet been validated, final results will be available in early February 2017.</p>
        <p style="background-color:yellow; visibility: hidden;" >Print the results or copy and paste them into your own file or this <a href="http://www.tomorrow.org/speakup/downloads/promoMaterials/SpeakUp_data_template.xls">Speak Up Data Excel Template</a>.</p>

        <!-- Option 1 - District Results -->
        <asp:Table ID="tblDistrict" runat="server" Width="750">
            <asp:TableRow>
                <asp:TableCell Height="50" VerticalAlign="Bottom">
                    <asp:Image ImageUrl="images/option01.gif" Width="100" Height="33" runat="server" />
                </asp:TableCell>
                <asp:TableCell HorizontalAlign="Left" ColumnSpan="2" VerticalAlign="Bottom">
                    <h2>View District Results</h2>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell></asp:TableCell>
                <asp:TableCell ColumnSpan="2">
                    Enter the state or province, district name (up to 10 characters), district admin password,
                    and click on "Look Up Data" to begin
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell></asp:TableCell>
                <asp:TableCell>State/Province</asp:TableCell>
                <asp:TableCell>
                    <asp:DropDownList ID="ddlStOpt1" runat="server" AppendDataBoundItems="true">
                        <asp:ListItem Value="">[Select State/Province]</asp:ListItem>
                    </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="rfvStOpt1" runat="server" ControlToValidate="ddlStOpt1" ErrorMessage="Please select a state" InitialValue="0" ValidationGroup="grpOption1" Display="Dynamic" CssClass="lblError" />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell></asp:TableCell>
                <asp:TableCell>District Name</asp:TableCell>
                <asp:TableCell>
                    <asp:TextBox ID="txtDistNameOpt1" runat="server" MaxLength="10" Width="150"></asp:TextBox>
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtDistNameOpt1" ErrorMessage="Please enter up to 10 characters of the district name" Display="Dynamic" ValidationGroup="grpOption1" CssClass="lblError" ID="rtvDistNameOpt1" />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell></asp:TableCell>
                <asp:TableCell>District Admin Password</asp:TableCell>
                <asp:TableCell>
                    <asp:TextBox ID="txtPswrdOpt1" runat="server" MaxLength="20" TextMode="Password" Width="150"></asp:TextBox>
                             <asp:RequiredFieldValidator runat="server" ControlToValidate="txtPswrdOpt1" ErrorMessage="Please enter the district admin password" Display="Dynamic" ValidationGroup="Info" CssClass="lblError" ID="rtvPwrdOpt1" />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell></asp:TableCell>
                <asp:TableCell ColumnSpan="2" HorizontalAlign="Center">
                    <asp:Button ID="btnOption1" runat="server" Text="Look Up Data" OnClick="ProcessOption1" /><br />
                <br /><asp:ValidationSummary  ID="valsumOpt1" runat=server HeaderText="There were errors on the page:" ValidationGroup="grpOption1" />
                    <asp:Label ID="lblNotFoundOpt1" runat="server" ForeColor="Red"
                    Text="Could not find that combination of state, district name and password, please try again."></asp:Label>
                </asp:TableCell>
            </asp:TableRow>
    
        <%-- Option 2 - School Results --%>
            <asp:TableRow>
                <asp:TableCell ColumnSpan="3" Height="10">&nbsp;</asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell Height="50" VerticalAlign="Bottom">
                    <asp:Image ID="Image1" ImageUrl="images/option02.gif" Width="100" Height="33" runat="server" />
                </asp:TableCell>
                <asp:TableCell HorizontalAlign="Left" ColumnSpan="2" VerticalAlign="Bottom">
                    <h2>View School Results</h2>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell></asp:TableCell>
                <asp:TableCell ColumnSpan="2">
                    Enter the state or province, school name (up to 10 characters), school or district admin password,
                    and click on "Look Up Data" to begin
                    (Schools outside of the U.S./Canada: Please select "International" under State/Province) 
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell></asp:TableCell>
                <asp:TableCell>State/Province</asp:TableCell>
                <asp:TableCell>
                    <asp:DropDownList ID="ddlStOpt2" runat="server" AppendDataBoundItems="true">
                        <asp:ListItem Value="">[Select State/Province]</asp:ListItem>
                    </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="ddlStOpt2" ErrorMessage="Please select a state" InitialValue="0" ValidationGroup="grpOption2" Display="Dynamic" CssClass="lblError" />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell></asp:TableCell>
                <asp:TableCell>School Name</asp:TableCell>
                <asp:TableCell>
                    <asp:TextBox ID="txtSchNameOpt2" runat="server" MaxLength="10" Width="150"></asp:TextBox>
                             <asp:RequiredFieldValidator runat="server" ControlToValidate="txtSchNameOpt2" ErrorMessage="Please enter up to 10 characters of the school name" Display="Dynamic" ValidationGroup="grpOption2" CssClass="lblError" ID="RequiredFieldValidator1" />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell></asp:TableCell>
                <asp:TableCell>Admin Password<br />(School or District)</asp:TableCell>
                <asp:TableCell>
                    <asp:TextBox ID="txtPswrdOpt2" runat="server" MaxLength="20" TextMode="Password" Width="150"></asp:TextBox>
                             <asp:RequiredFieldValidator runat="server" ControlToValidate="txtPswrdOpt2" ErrorMessage="Please enter the school or district admin password" Display="Dynamic" ValidationGroup="grpOption2" CssClass="lblError" ID="RequiredFieldValidator2" />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell></asp:TableCell>
                <asp:TableCell ColumnSpan="2" HorizontalAlign="Center">
                    <asp:Button ID="btnOption2" runat="server" Text="Look Up Data" OnClick="ProcessOption2" /><br />
                <br /><asp:ValidationSummary  ID="ValidationSummary3" runat=server HeaderText="There were errors on the page:" ValidationGroup="grpOption2" />
                    <asp:Label ID="lblNotFoundOpt2" runat="server" ForeColor="Red"
                    Text="Could not find that combination of state, building name and password, please try again."></asp:Label>
                </asp:TableCell>
            </asp:TableRow>
        

        <%-- Option3 - Unknown Password --%>
            <asp:TableRow>
                <asp:TableCell ColumnSpan="3" Height="10">&nbsp;</asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell Height="50" VerticalAlign="Bottom">
                    <asp:Image ID="Image2" ImageUrl="images/option03.gif" Width="100" Height="33" runat="server" />
                </asp:TableCell>
                <asp:TableCell HorizontalAlign="Left" ColumnSpan="2" VerticalAlign="Bottom">
                    <h2>Need additional help?</h2>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell></asp:TableCell>
                <asp:TableCell ColumnSpan="2">
                    <p>If you can't remember your password or aren't sure who your district or school contact is, 
                    we can help. Send us an email to 
                    <a href="mailto:speakup@tomorrow.org">speakup@tomorrow.org</a>
                    or look up your Speak Up contact below and email them directly</p> 
                    <p>For best search results, select the state and enter the first few letters (up to 10) 
                    of the district or school name.</p>
                    <p>(Schools outside of the U.S./Canada: Please select "International" under State) </p>
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell></asp:TableCell>
                <asp:TableCell>State</asp:TableCell>
                <asp:TableCell>
                    <asp:DropDownList ID="ddlStOpt3" runat="server" AppendDataBoundItems="true">
                        <asp:ListItem Value="">[Select State]</asp:ListItem>
                    </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="ddlStOpt3" ErrorMessage="Please select a state" InitialValue="0" ValidationGroup="grpOption3" Display="Dynamic" CssClass="lblError" />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell></asp:TableCell>
                <asp:TableCell>Look Up</asp:TableCell>
                <asp:TableCell>
                    <asp:RadioButtonList ID="rblType3" runat="server" CssClass="label" ForeColor="Black" BackColor="White">
                        <asp:ListItem Value="DISTRICT">District Name</asp:ListItem>
                        <asp:ListItem Value="SCHOOL" Selected="True">School Name</asp:ListItem>
                    </asp:RadioButtonList>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ControlToValidate="rblType3" ErrorMessage="Please select what to look up" ValidationGroup="grpOption3" Display="Dynamic" CssClass="lblError" />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell></asp:TableCell>
                <asp:TableCell>Name</asp:TableCell>
                <asp:TableCell>
                    <asp:TextBox ID="txtNameOpt3" runat="server" MaxLength="10" Width="150"></asp:TextBox>
                             <asp:RequiredFieldValidator runat="server" ControlToValidate="txtNameOpt3" ErrorMessage="please enter up to 10 characters of the school or district name" Display="Dynamic" ValidationGroup="grpOption3" CssClass="lblError" ID="RequiredFieldValidator5" />
                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell></asp:TableCell>
                <asp:TableCell ColumnSpan="2" HorizontalAlign="Center">
                    <asp:Button ID="btnOption3" runat="server" Text="Find School/District" OnClick="ProcessOption3"
                     Group="grpOption3" /><br />
                <br /><asp:ValidationSummary  ID="ValidationSummary1" runat=server HeaderText="There were errors on the page:" ValidationGroup="grpOption3" />
                    <asp:Label ID="lblNotFoundOpt3" runat="server" ForeColor="Red"
                    Text="Could not find that combination of state and district or building name, please try again."></asp:Label>
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
    </asp:View>
    <asp:View ID="vDistrict" runat="server">
         <h3>Find Your Speak Up Survey Data</h3>
       <p>Choose your district from the list below</p>
        <!-- Scrolling window -->
        <div style="BORDER-RIGHT: 1px solid lightblue; BORDER-TOP: 1px solid lightblue; BORDER-LEFT: 1px solid lightblue; BORDER-BOTTOM: 1px solid lightblue; OVERFLOW: auto; WIDTH: 100%; HEIGHT: 300px;">
            <asp:Table ID="tblDistList" runat="server"></asp:Table>
        </div>
    </asp:View>
    <asp:View ID="vSchool" runat="server">
         <h3>Find Your Speak Up Survey Data</h3>
       <p>Choose your school from the list below</p>
        <!-- Scrolling window -->
        <div style="BORDER-RIGHT: 1px solid lightblue; BORDER-TOP: 1px solid lightblue; BORDER-LEFT: 1px solid lightblue; BORDER-BOTTOM: 1px solid lightblue; OVERFLOW: auto; WIDTH: 100%; HEIGHT: 300px;">
            <asp:Table ID="tblSchList" runat="server"></asp:Table>
        </div>
    </asp:View>
    <asp:View ID="vLookup" runat="server">
         <h3>Find Your <asp:Label ID="lblLookupType" runat="server"></asp:Label> Speak Up Contact</h3>
       <p><asp:Label ID="lblUnknownHdr" runat="server"></asp:Label>
         <a href="mailto:speakup@tomorrow.org">please contact the Speak Up Team</a></p>
        <!-- Scrolling window -->
        <div style="BORDER-RIGHT: 1px solid lightblue; BORDER-TOP: 1px solid lightblue; BORDER-LEFT: 1px solid lightblue; BORDER-BOTTOM: 1px solid lightblue; OVERFLOW: auto; WIDTH: 100%; HEIGHT: 300px;">
            <asp:Table ID="tblLookup" runat="server" CellPadding="5"></asp:Table>
        </div>
    </asp:View>
</asp:MultiView>
</asp:Content>

