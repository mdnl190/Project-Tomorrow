using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using System.Data.SqlClient;
using System.Data;
using SUcodes;

/* Modified: 03/10/2015
 * Bob O'Dell
 * Based off the 2008 version
 * Allows users to select state, district, school and what survey to examine
 */

public partial class welcome : System.Web.UI.Page
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

    // variables displayed on page
    private int regid;
    private string regId;
    private int orgid;
    private string orgId;
    private string state;

    private bool bold = true; // bold text in a table cell
    private bool notBold = false;
    private bool alignCenter = true; // center or left justify text in a table cell
    private bool alignLeft = false;

    private bool oddRow = true; // even/odd row for background

    private DataSet dsResults;
    private DataTable dtInfo;
    private DataTable dtDistrictResults;
    private DataTable dtSchoolResults;
    #endregion

    #region Log and Exception Handling
    private void Log(string sMethod, string sEntryType, string sValueName, string sValue, string sEntryText)
    {
        // log the error into the log database
        sc.LogEntry("Welcome", sMethod, sEntryType, sValueName, sValue, sEntryText);
        // display to the screen if an error
        if (sEntryType == "ERROR") lblError.Text += "Error: " + sEntryText;
    }
    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        //if (!IsPostBack)
        //{
            if (Session["type"] == null) Response.Redirect("find_school.aspx", false);
            else
            {
                string reportType = (string)Session["type"];
                if (reportType == "ORG")
                {
                    orgid = (int)Session["orgid"];
                    
                    lblType1.Text = "district";
                    lblType2.Text = "district";
                    lblGoBack.Text = "district";
                    litGoBack.Text = "district";
                    trSchool.Visible = false;
                    pnlExcel.Visible = true;
                    SetupDist();
                }
                else
                {
                    regid = (int)Session["regid"];
                    lblType1.Text = "school";
                    lblType2.Text = "school";
                    lblGoBack.Text = "school";
                    litGoBack.Text = "school";
                    trSchool.Visible = true;
                    pnlExcel.Visible = false;
                    SetupSch();
                }
            }
        //}
    }

    private void SetupDist()
    {
        oddRow = true; // for row background

        SqlCommand cmd = new SqlCommand("GetThisOrg2", con);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandTimeout = 360;
        cmd.Parameters.AddWithValue("@orgid", orgid);
        SqlDataAdapter adpter = new SqlDataAdapter();
        con.Open();
        adpter.SelectCommand = cmd;
        dsResults = new DataSet();
        adpter.Fill(dsResults);
        con.Close();
        dtInfo = dsResults.Tables[0];
        dtDistrictResults = dsResults.Tables[1];

        DataRow dr = dtInfo.Rows[0];
        if (dtInfo.Rows.Count > 0)
        {
            lblOrgId.Text = orgid.ToString();
            lblRegId.Text = "0"; // placekeeper
            lblDistrict.Text = dr["district"].ToString();
            lblCity.Text = dr["city"].ToString();
            state = dr["state"].ToString();
            lblState.Text = state;
            lblZIP.Text = dr["zip"].ToString();
            
            tblCounts.Rows.Add(HeaderRow("ORG","DISTRICT"));

            foreach (DataRow drDist in dtDistrictResults.Rows)
            {  
                string cntStr = "0";
                if (Convert.ToBoolean(drDist["groupsurvey"].ToString()) == false)
                    cntStr = drDist["DistSurvCnt"].ToString();
                else
                    cntStr = drDist["DistGrpCnt"].ToString();
                AddDistCounts(drDist["name"].ToString(), 
                    cntStr,
                    drDist["survey_id"].ToString(),
                    orgid.ToString(),
                    state);
            }
        }
        con.Close();
    }
    private void AddDistCounts(string surveyTitle, string strDistCount, string survey, string orgId, string state)
    {
        int distCount = 0;
        if (strDistCount.Length > 0) // blank or null means zero count
            distCount = Convert.ToInt32(strDistCount);
        TableRow row = new TableRow();
        if (oddRow) row.BackColor = System.Drawing.Color.LightGray;
        oddRow = !oddRow;
        row.Cells.Add(TextCell(surveyTitle, notBold, alignLeft));
        row.Cells.Add(CountCell(strDistCount, survey, orgId, state, "D"));
        // add in district open ended comment link
        row.Cells.Add(OELink("District", surveyTitle, survey));
        tblCounts.Rows.Add(row);
    }

    private void SetupSch()
    {
        oddRow = true; // for row background

        SqlCommand cmd = new SqlCommand("GetThisBldg2", con);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandTimeout = 360;
        cmd.Parameters.AddWithValue("@regid", regid);
        SqlDataAdapter adpter = new SqlDataAdapter();
        con.Open();
        adpter.SelectCommand = cmd;
        dsResults = new DataSet();
        adpter.Fill(dsResults);
        con.Close();
        dtInfo = dsResults.Tables[0];
        dtSchoolResults = dsResults.Tables[1];
        dtDistrictResults = dsResults.Tables[2];

        if (dtInfo.Rows.Count > 0)
        {
            DataRow dr = dtInfo.Rows[0];
            lblRegId.Text = regid.ToString();
            lblSchoolName.Text = dr["school_name"].ToString();
            lblDistrict.Text = dr["district"].ToString();
            string regType = dr["regtype"].ToString();
            orgId = dr["orgid"].ToString();
            lblOrgId.Text = orgId;
            lblCity.Text = dr["city"].ToString();
            state = dr["state"].ToString();
            lblState.Text = state;
            lblZIP.Text = dr["zip"].ToString();

            tblCounts.Rows.Add(HeaderRow("BLDG", regType)); // need to regtype before setting up header

            if (dtDistrictResults.Rows.Count > 0)
            {
                foreach (DataRow drDist in dtDistrictResults.Rows)
                {
                    string distCntStr = "0";
                    if (Convert.ToBoolean(drDist["groupsurvey"].ToString()) == false)
                        distCntStr = drDist["SurvCnt"].ToString();
                    else
                        distCntStr = drDist["GrpCnt"].ToString();

                    // Get the corresponding schools counts
                    string selectStr = "Survey_id = " + drDist["survey_id"].ToString();
                    DataRow[] drSchRes = dtSchoolResults.Select(selectStr);
                    string schCntStr = "0";
                    if (drSchRes.Length > 0)
                    {
                        if (Convert.ToBoolean(drSchRes[0]["groupsurvey"].ToString()) == false)
                            schCntStr = drSchRes[0]["SurvCnt"].ToString();
                        else
                            schCntStr = drSchRes[0]["GrpCnt"].ToString();
                    }
                    AddSchCounts(drDist["name"].ToString(),
                        schCntStr,
                        distCntStr,
                        drDist["survey_id"].ToString(),
                        regid.ToString(),
                        orgId,
                        state,
                        regType);
                }
            }
            else
            {
                foreach (DataRow drSch in dtSchoolResults.Rows)
                {
                    string cntStr = "0";
                    if (Convert.ToBoolean(drSch["groupsurvey"].ToString()) == false)
                        cntStr = drSch["SurvCnt"].ToString();
                    else
                        cntStr = drSch["GrpCnt"].ToString();
                    AddSchCounts(drSch["name"].ToString(),
                        cntStr, 
                        "0",
                        drSch["survey_id"].ToString(),
                        regid.ToString(),
                        "0",
                        state,
                        regType);
                }
            }
            
        }
        con.Close();
    }
    private void AddSchCounts(string surveyTitle, string schCountStr, string strDistCount, string survey, 
        string regId, string orgId, string state, string regType)
    {
        int distCount = 0;
        int iSchCount = 0;
        // check for condition where count is empty or null which we treat as zero
        if (strDistCount.Length > 0)
            distCount = Convert.ToInt32(strDistCount);
        if (schCountStr.Length > 0)
            iSchCount = Convert.ToInt32(schCountStr);
        TableRow row = new TableRow();
        if (oddRow) row.BackColor = System.Drawing.Color.LightGray;
        oddRow = !oddRow;
        row.Cells.Add(TextCell(surveyTitle, notBold, alignLeft));
        row.Cells.Add(CountCell(schCountStr, survey, regId, state, "S"));
        if (regType == "DISTRICT"  || regType=="PUBSCHOOL") // only show district if public schools
            row.Cells.Add(CountCell(strDistCount, survey, orgId, state, "D"));
        // add in school and district open ended comment link
        row.Cells.Add(OELink("School", surveyTitle, survey));
        if (regType == "DISTRICT" || regType == "PUBSCHOOL") // only show district if public schools
            row.Cells.Add(OELink("District", surveyTitle, survey));
        tblCounts.Rows.Add(row);
    }

    private TableRow HeaderRow(String reportType, string regType)
    {
        TableRow row = new TableRow();
        // Survey Name
        TableCell cell = new TableCell();
        cell.Width = 400;
        Label lbl = new Label();
        lbl.Text = "Survey";
        lbl.Font.Size = FontUnit.Larger;
        lbl.Font.Bold = true;
        cell.Controls.Add(lbl);
        row.Cells.Add(cell);

        // Building Totals
        if (reportType == "BLDG")
        {
            cell = new TableCell();
            cell.Width = 100;
            lbl = new Label();
            lbl.Text = "# of School Surveys";
            lbl.Font.Size = FontUnit.Larger;
            lbl.Font.Bold = true;
            cell.Controls.Add(lbl);
            row.Cells.Add(cell);
        }

        // District Totals
        if (regType == "DISTRICT" || regType == "PUBSCHOOL")
        {
            cell = new TableCell();
            cell.Width = 100;
            lbl = new Label();
            lbl.Text = "# of District Surveys";
            lbl.Font.Size = FontUnit.Larger;
            lbl.Font.Bold = true;
            cell.Controls.Add(lbl);
            row.Cells.Add(cell);
        }

        // Links to Open Ended Responses
        cell = new TableCell();
        if (reportType == "BLDG")
            cell.ColumnSpan = 2;
        lbl = new Label();
        lbl.Text = "Open Ended Responses";
        lbl.Font.Size = FontUnit.Larger;
        lbl.Font.Bold = true;
        cell.Controls.Add(lbl);
        row.Cells.Add(cell);

        return row;
    }
    private TableCell TextCell(string textStr, bool bold, bool alignCenter)
    {
        TableCell cell = new TableCell();
        if (alignCenter)
            cell.HorizontalAlign = HorizontalAlign.Center;
        Label lbl = new Label();
        lbl.Text = textStr;
        lbl.Font.Bold = bold;
        cell.Controls.Add(lbl);
        return cell;
    }
    private TableCell CountCell(string countStr, string survey, string id, string state, string reportType)
    {
        TableCell cell = new TableCell();
        if (countStr == "0")
        {
            // 0 surveys so show as text
            Label lbl = new Label();
            lbl.Text = "0";
            cell.Controls.Add(lbl);
        }
        else
        {
            // we have a count so we need to set up a link
            LinkButton btn = new LinkButton();
            btn.Text = countStr;
            if (reportType == "D") // district link
                btn.Command += new CommandEventHandler(ShowDistReport);
            else
                btn.Command += new CommandEventHandler(ShowSchReport);
            string sCmdArg = id + "|" + state + "|" + survey;
            btn.CommandArgument = sCmdArg;
            cell.Controls.Add(btn);
        }
        return cell;
    }
    private TableCell OELink(string reportType, string surveyName, string surveyId)
    {
        TableCell cell = new TableCell();
        LinkButton btn = new LinkButton();
        btn.Text = reportType;
        btn.Command +=new CommandEventHandler(ShowOpenEndedReport);
        btn.CommandName = surveyName;
        btn.CommandArgument = surveyId;
        cell.Controls.Add(btn);
        return cell;
    }

    protected void ShowDistReport(object sender, CommandEventArgs e)
    {
        string valStr = (string)e.CommandArgument;
        string[] vals = valStr.Split('|');
        string url = "SurveyReport.aspx?District_ID=" + vals[0];
        url += "&State_ID=''"; // + vals[1]
        url += "&Survey_ID=" + vals[2];
        Response.Redirect(url, false);
    }
    protected void ShowSchReport(object sender, CommandEventArgs e)
    {
        string valStr = (string)e.CommandArgument;
        string[] vals = valStr.Split('|');
        string url = "SurveyReport.aspx?School_ID=" + vals[0];
        url += "&State_ID=''"; //+ vals[1]
        url += "&Survey_ID=" + vals[2];
        Response.Redirect(url, false);
    }
    protected void ShowOpenEndedReport(object sender, CommandEventArgs e)
    {
        LinkButton btn = (LinkButton)sender;

        Session["ReportType"] = btn.Text;
        Session["DistrictID"] = lblOrgId.Text;
        Session["DistrictName"] = lblDistrict.Text;
        Session["SchoolID"] = lblRegId.Text;
        Session["SchoolName"] = lblSchoolName.Text;
        Session["SurveyID"] = (string)e.CommandArgument;
        Session["SurveyName"] = (string)e.CommandName;

        lblError.Text = " type=" + btn.Text + " did=" + lblOrgId.Text + " dn=" + lblDistrict.Text;
        lblError.Text += " id=" + lblRegId.Text + " sn=" + lblSchoolName.Text;
        lblError.Text += " survid=" + (string)e.CommandArgument + " survn=" + (string)e.CommandName;

        Response.Redirect("~/OpenEnded/ShowResponses.aspx", false);
    }

    protected void GoToExcelSummary(object sender, EventArgs e)
    {
        Session["ExcelDistId"] = lblOrgId.Text;
        Session["ExcelDistName"] = lblDistrict.Text;
        Response.Redirect("DistrictExcelSummaryReport.aspx", false);
    }
}
