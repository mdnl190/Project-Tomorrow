using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using System.Data.SqlClient;
using System.Data;


/* Modified: 12/02/2010
 * Bob O'Dell
 * This program presents summary reports to the screen for the user/admin
 * based on several parameters that get passed.  They can then select the survey at building
 * or district level that they want to view
 */

public partial class Find_School : System.Web.UI.Page
{
     #region Variables and Constants
    // connection strings
    private static string conStr = WebConfigurationManager.ConnectionStrings["Speakup"].ConnectionString;
    private SqlConnection con = new SqlConnection(conStr);
    private SqlConnection con2 = new SqlConnection(conStr); // second connection for when first may be open
    private SqlConnection con3 = new SqlConnection(conStr); // third connection for when first may be open
    private static string conLogStr = WebConfigurationManager.ConnectionStrings["SpeakupLog"].ConnectionString;
    private SqlConnection conLog = new SqlConnection(conLogStr);
    private DataTable dtSch;
    // common routines
    private SpeakupCommon sc = new SpeakupCommon(); // used for a number of routines including logging

    private bool bBold = true; // bold text in a table cell
    private bool bNotBold = false;
    private bool bCenter = true; // center or left justify text in a table cell
    private bool bLeft = false;

    private bool bOdd = true; // used for row background color
    #endregion

    #region Log and Exception Handling
    private void Log(string sMethod, string sEntryType, string sValueName, string sValue, string sEntryText)
    {
        // log the error into the log database
        sc.LogEntry("Find_School", sMethod, sEntryType, sValueName, sValue, sEntryText);
        // display to the screen if an error
        if (sEntryType == "ERROR") lblError.Text += "Error: " + sEntryText;
    }
    #endregion

   protected void Page_Load(object sender, EventArgs e)
   {
        PeterBlum.DES.Globals.WebFormDirector.Suite_LicenseKey = "600-351203212";
        SetupStateLists();
        lblNotFoundOpt1.Visible = false;
        lblNotFoundOpt2.Visible = false;
        lblNotFoundOpt3.Visible = false;
        if (!IsPostBack)
        {
            mvLookup.SetActiveView(vEntry);
        }
        else
        {
            if (mvLookup.ActiveViewIndex == 0)
            {
                Session["pswrd1"] = txtPswrdOpt1.Text;
                Session["pswrd2"] = txtPswrdOpt2.Text;
            }
            else if (mvLookup.ActiveViewIndex == 1)
            {
                ProcessOption1(sender, e);
            }
            else if (mvLookup.ActiveViewIndex == 2)
            {
                ProcessOption2(sender, e);
            }
        }

   }
   private void SetupStateLists()
   {
        //DataTable dtST = sc.GetStates("USA","NAME");
        DataTable dtST = sc.GetStates("ALL","NAME");
        ddlStOpt1.DataSource = dtST;
        ddlStOpt1.DataValueField = "state_abbr";
        ddlStOpt1.DataTextField = "state_name";
        ddlStOpt1.DataBind();

        ddlStOpt2.DataSource = dtST;
        ddlStOpt2.DataValueField = "state_abbr";
        ddlStOpt2.DataTextField = "state_name";
        ddlStOpt2.DataBind();
        // for schools we add an international option
        ListItem item = new ListItem("International", "INTL");
        ddlStOpt2.Items.Add(item);

        ddlStOpt3.DataSource = dtST;
        ddlStOpt3.DataValueField = "state_abbr";
        ddlStOpt3.DataTextField = "state_name";
        ddlStOpt3.DataBind();
        // lookup for schools also needs an international option
        item = new ListItem("International", "INTL");
        ddlStOpt3.Items.Add(item);
   }
   
   protected void ProcessOption1(object sender, EventArgs e)
   {
       // they might hit enter, so we try to figure out what they want
       if (txtDistNameOpt1.Text.Length == 0)
           ProcessOption2(sender, e);
       else
       {
           SqlCommand cmdChk = new SqlCommand("ReportDistAccess", con);
           cmdChk.CommandType = CommandType.StoredProcedure;
           cmdChk.Parameters.AddWithValue("@mailstate", ddlStOpt1.SelectedValue);
           cmdChk.Parameters.AddWithValue("@distname", "%" + txtDistNameOpt1.Text + "%");
           cmdChk.Parameters.AddWithValue("@password", (string)Session["pswrd1"]);
           con.Open();
           SqlDataAdapter aptrChk = new SqlDataAdapter();
           aptrChk.SelectCommand = cmdChk;
           try
           {
               DataTable dtDist = new DataTable();
               aptrChk.Fill(dtDist);
               if (dtDist.Rows.Count == 0)
               {
                   lblNotFoundOpt1.Visible = true;
               }
               else if (dtDist.Rows.Count == 1)
               {
                   int iOrgId = Convert.ToInt32(dtDist.Rows[0]["organization_id"].ToString());
                   Session["orgid"] = iOrgId;
                   Session["type"] = "ORG";
                   Response.Redirect("welcome.aspx", false);
               }
               else
               {
                   ShowDistrictList(dtDist);
               }
           }
           catch (SqlException err)
           {
               Log("ProcessOption1", "ERROR", "", "", "Couldn't read districts. " + err.Message);
           }
           finally
           {
               con.Close();
           }
       }
   }
   private void ShowDistrictList(DataTable dtDistricts)
   {
       tblDistList.Rows.Clear();
       bool bOdd = true;
       TableRow rowHdr = new TableRow();
       rowHdr.Cells.Add(TextCell("District", bBold, bCenter));
       rowHdr.Cells.Add(TextCell("City", bBold, bCenter));
       tblDistList.Rows.Add(rowHdr);

       foreach (DataRow drDist in dtDistricts.Rows)
       {
           TableRow row = new TableRow();
           if (bOdd) row.BackColor = System.Drawing.Color.LightGray;
           bOdd = !bOdd;
           row.Cells.Add(LinkCell(drDist["orgname"].ToString(),drDist["organization_id"].ToString(),"ORG"));
           row.Cells.Add(TextCell(drDist["mailcity"].ToString(),bNotBold,bLeft));
           tblDistList.Rows.Add(row);
       }

       mvLookup.SetActiveView(vDistrict);
   }

    protected void ProcessOption2(object sender, EventArgs e)
   {
       // they might hit enter, so we try to figure out what they want
       if (txtSchNameOpt2.Text.Length == 0)
           ProcessOption3(sender, e);
       else
       {
           SqlCommand cmdChk = new SqlCommand("ReportBldgAccess", con);
           cmdChk.CommandType = CommandType.StoredProcedure;
           cmdChk.Parameters.AddWithValue("@mailstate", ddlStOpt2.SelectedValue);
           cmdChk.Parameters.AddWithValue("@bldgname", "%" + txtSchNameOpt2.Text + "%");
           cmdChk.Parameters.AddWithValue("@password", (string)Session["pswrd2"]);
           con.Open();
           SqlDataAdapter aptrChk = new SqlDataAdapter();
           aptrChk.SelectCommand = cmdChk;
           try
           {
               DataTable dtSch = new DataTable();
               aptrChk.Fill(dtSch);
               if (dtSch.Rows.Count == 0)
               {
                   lblNotFoundOpt2.Visible = true;
               }
               else if (dtSch.Rows.Count == 1)
               {
                   int iOrgId = Convert.ToInt32(dtSch.Rows[0]["registration_id"].ToString());
                   Session["regid"] = iOrgId;
                   Session["type"] = "BLDG";
                   Response.Redirect("welcome.aspx", false);
               }
               else
               {
                   ShowSchoolList(dtSch);
               }
           }
           catch (SqlException err)
           {
               Log("ProcessOption2", "ERROR", "", "", "Couldn't read buildings. " + err.Message);
           }
           finally
           {
               con.Close();
           }
       }
   }
   private void ShowSchoolList(DataTable dtSchools)
   {
       tblSchList.Rows.Clear();
       bool bOdd = true;
       TableRow rowHdr = new TableRow();
       rowHdr.Cells.Add(TextCell("School", bBold, bCenter));
       rowHdr.Cells.Add(TextCell("City", bBold, bCenter));
       tblSchList.Rows.Add(rowHdr);
       foreach (DataRow drSch in dtSchools.Rows)
       {
           TableRow row = new TableRow();
           if (bOdd) row.BackColor = System.Drawing.Color.LightGray;
           bOdd = !bOdd;
           row.Cells.Add(LinkCell(drSch["registrantname"].ToString(),drSch["registration_id"].ToString(),"SCH"));
           row.Cells.Add(TextCell(drSch["mailcity"].ToString(),bNotBold,bLeft));
           tblSchList.Rows.Add(row);
       }
       mvLookup.SetActiveView(vSchool);
   }

   protected void ProcessOption3(object sender, EventArgs e)
   {
       // they might hit enter, so we try to figure out what they want
       if ( txtNameOpt3.Text.Length != 0)
       {
           // set up the header for school or district
           if (rblType3.SelectedValue == "DISTRICT")
           {
               lblLookupType.Text = "District";
               lblUnknownHdr.Text = "To obtain your district's Admin Password from the district contact, click";
               lblUnknownHdr.Text += " on the contact. If you were unable to locate your district";
               lblUnknownHdr.Text += " or obtain your Admin Password, ";
           }
           else  // looking for school
           {
               lblLookupType.Text = "School";
               lblUnknownHdr.Text = "To obtain your school's Admin Password from the school or district contact, click";
               lblUnknownHdr.Text += " on the contact. If you were unable to locate your school";
               lblUnknownHdr.Text += " or obtain your Admin Password, ";
           }
           SqlCommand cmdChk = new SqlCommand("ReportBldgListNoPswrd", con);
           cmdChk.CommandType = CommandType.StoredProcedure;
           cmdChk.Parameters.AddWithValue("@mailstate", ddlStOpt3.SelectedValue);
           if (rblType3.SelectedValue == "DISTRICT")
               cmdChk.Parameters.AddWithValue("@dist", "%" + txtNameOpt3.Text + "%");
           else
               cmdChk.Parameters.AddWithValue("@bldg", "%" + txtNameOpt3.Text + "%");
           con.Open();
           try
           {
               SqlDataReader rdrChk = cmdChk.ExecuteReader();
               if (rdrChk.HasRows)
               {
                   tblLookup.Rows.Add(HeaderRow(lblLookupType.Text));
                   while (rdrChk.Read())
                   {
                       TableRow row = new TableRow();
                       if (bOdd) row.BackColor = System.Drawing.Color.LightGray;
                       bOdd = !bOdd;
                       // school city state email
                       row.Cells.Add(TextCell(rdrChk["name"].ToString(), bNotBold, bLeft));
                       row.Cells.Add(TextCell(rdrChk["mailcity"].ToString(), bNotBold, bLeft));
                       row.Cells.Add(TextCell(rdrChk["mailstate"].ToString(), bNotBold, bLeft));
                       string semail = "";
                       if (rdrChk["orgcontactemail"].ToString().Length > 0)
                       {
                           semail += "District: <a href=\"mailto:" + rdrChk["orgcontactemail"].ToString() + "\">";
                           semail += rdrChk["orgcontactfirst"].ToString() + " " + rdrChk["orgcontactlast"].ToString();
                           semail += "</a><br />";
                       }
                       row.Cells.Add(TextCell(semail, bNotBold, bLeft));

                       tblLookup.Rows.Add(row);
                   }
                   mvLookup.SetActiveView(vLookup);
               }
               else
               {
                   lblNotFoundOpt3.Visible = true;
               }
           }
           catch (SqlException err)
           {
               Log("ProcessOption3", "ERROR", "", "", "Couldn't read buildings. " + err.Message);
           }
           finally
           {
               con.Close();
           }
       }
   }
   
    private TableRow HeaderRow(string sType)
   {
       TableRow row = new TableRow();
       row.Cells.Add(TextCell(sType, bBold, bCenter));
       row.Cells.Add(TextCell("City", bBold, bCenter));
       row.Cells.Add(TextCell("State", bBold, bCenter));
       row.Cells.Add(TextCell("Email", bBold, bCenter));
       return row;
   }
   private TableCell TextCell(string sText, bool bBold, bool bCenter)
   {
       TableCell cell = new TableCell();
       if (bCenter)
        cell.HorizontalAlign = HorizontalAlign.Center;
       Label lbl = new Label();
       lbl.Text = sText;
       lbl.Font.Bold = bBold;
       cell.Controls.Add(lbl);
       return cell;
   }
   private TableCell LinkCell(string sText, string sId, string sType)
   {
       TableCell cell = new TableCell();
       LinkButton btn = new LinkButton();
       btn.ID = sType + sId;
       btn.Text = sText;
       if (sType == "ORG")
           btn.Command += new CommandEventHandler(GoToDistRep);
       else
           btn.Command += new CommandEventHandler(GoToSchRep);
       btn.CommandArgument = sId;
       cell.Controls.Add(btn);
       return cell;
   }

   private void GoToDistRep(object sender, CommandEventArgs e)
   {
       int iOrgId = Convert.ToInt32(e.CommandArgument);
       Session["orgid"] = iOrgId;
       Session["type"] = "ORG";
       Response.Redirect("welcome.aspx", false);

   }
   private void GoToSchRep(object sender, CommandEventArgs e)
   {
       int iRegId = Convert.ToInt32(e.CommandArgument);
       Session["regid"] = iRegId;
       Session["type"] = "BLDG";
       Response.Redirect("welcome.aspx", false);

   }
}
