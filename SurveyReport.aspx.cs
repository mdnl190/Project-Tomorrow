using System;
using System.Data;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using Microsoft.Reporting.WebForms;
using System.Web.Configuration;
using SUcodes;

/* Modified: 12/02/2015 - Bob O'Dell
 * numerous modifications before Feb 2017 but not delineated
 * 02/07/2017 - Added code to recognize integer questions and remove the % (and prefaced with Avg:)
 * 02/19/2017 - Added check for if group survey to determine question header
 * This is an adaptation of the original page that used repeaters.  I have replaced that will dynamically generated tables.
 * This is a preference and style rather than a performance improvement
 * This pulls in all of the data for a survey for a particular, school, district or state and displays it
 */

public partial class SurveyReportTest : System.Web.UI.Page
{
    #region Variables and Constants
    DataSet reportData = new DataSet();
    DataTable reportHeaders = new DataTable();
    DataTable reportQuestions = new DataTable();
    DataTable reportAnswers = new DataTable();
    DataTable reportSurveysTaken = new DataTable();
    DataTable reportEntity = new DataTable();
    bool isGroupSurvey;

    int intImageCounter = 0;

    private static string conStr = WebConfigurationManager.ConnectionStrings["Speakup"].ConnectionString;
    private SqlConnection objConn = new SqlConnection(conStr);
    // common routines  
    private SpeakupCommon sc = new SpeakupCommon(); // used for a number of routines including logging

    string mainSiteURL = ConfigurationManager.AppSettings["MainSiteUrl"].ToString(); // used for the image URL
    bool bold = true; // whether to bold or not bold text
    bool notBold = false;
    #endregion

    #region Log and Exception Handling
    private void Log(string methodName, string entryType, string valueName, string valueEntry, string userText, string sysText)
    {
        // log the error into the log database
        sc.LogEntry("Admin/EditSurveyMenu", methodName, entryType, valueName, valueEntry, userText + " " + sysText);
        // display to the screen if an error
        // display to the screen if an error
        if (entryType == "ERROR")
        {
            lblError.Text += "Error: " + userText;
            if (WebConfigurationManager.AppSettings["ApplicationMode"] == SUcodes.AppEmailMode.Debug)
                lblError.Text += " " + sysText;
        }
    }
    #endregion


    #region Properties
    public int SurveyID
    {
        get
        {
            return (int)ViewState["surveyID"];
        }
        set
        {
            ViewState["surveyID"] = value;
        }       
    }
    
    public int SchoolID
    {
        get
        {
            return (int)ViewState["schoolID"];
        }
        set
        {
            ViewState["schoolID"] = value;
        }       
    }

    public int DistrictID
    {
        get
        {
            return (int)ViewState["districtID"];
        }
        set
        {
            ViewState["districtID"] = value;
        }
    }
    
    public int QuestionID
    {
        get
        {
            return (int)ViewState["QuestionID"];
        }
        set
        {
            ViewState["QuestionID"] = value;
        }
    }
    
    public decimal AvgHrs
    {
        get
        {
            return (decimal)ViewState["AvgHrs"];
        }
        set
        {
            ViewState["AvgHrs"] = value;
        }
    }
    
    public decimal NatAvgHrs
    {
        get
        {
            return (decimal)ViewState["NatAvgHrs"];
        }
        set
        {
            ViewState["NatAvgHrs"] = value;
        }
    }
    
    public string StateID
    {
        get
        {
            return (string)ViewState["StateID"];
        }
        set
        {
            ViewState["StateID"] = value;
        }
    }
    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        GetQueryStrings();
        GetReportData();
        DisplayReportData();
    }

    #region obtain and display report data
    private void GetReportData()
    {
        //First step, initialize some local vars... (WQM) 01/30/2008
        //SurveyID = 4;
        //SchoolID = 16319;
            
        SqlCommand cmd = new SqlCommand("GetReportDetails", objConn);
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@SurveyID", SurveyID);
        cmd.Parameters.AddWithValue("@SchoolID", SchoolID);
        cmd.Parameters.AddWithValue("@DistrictID", DistrictID);
        if (StateID == "''") StateID = "";
        cmd.Parameters.AddWithValue("@StateID", StateID);
        cmd.CommandTimeout = 900;
        SqlDataAdapter objAdapt = new SqlDataAdapter();
        //lblError.Text += "suid=" + SurveyID + "*schid=" + SchoolID + "*distid=" + DistrictID + "*stid=" + StateID + "*";
        //return;
        objAdapt.SelectCommand = cmd;
        objConn.Open();
        try
        {
            objAdapt.Fill(reportData);
            reportHeaders = reportData.Tables[0];
            reportQuestions = reportData.Tables[1];
            reportAnswers = reportData.Tables[2];
            reportSurveysTaken = reportData.Tables[3];
            reportEntity = reportData.Tables[4];
            /*lblError.Text += "<br>Tables=" + reportData.Tables.Count.ToString();
            foreach (DataTable dt in reportData.Tables)
            {
                lblError.Text += "<br>" + dt.TableName + "=" + dt.Rows.Count.ToString();
            }
            return;*/
        }
        catch (SqlException err)
        {
            Log("GetReportData", "ERROR", "", "", "Error Retrieving Report Data", "suid=" + SurveyID.ToString() + "*schid=" + SchoolID.ToString() + "*distid=" + DistrictID.ToString() + "*stid=" + StateID + "*");
        }
        finally
        {
            objConn.Close();
        }

    }
    private void DisplayReportData()
    {
        if (reportData.Tables.Count > 0)
        {
            // put up the general report headers
            DataRow drGeneralDetails = reportHeaders.Rows[0];
            lblReportHeading.Text = drGeneralDetails["title"].ToString();
            lblReportHeading2.Text = drGeneralDetails["name"].ToString();
            isGroupSurvey = Convert.ToBoolean(drGeneralDetails["GroupSurvey"].ToString());

            lblTotalSurveyStudents.Text = "Results based on " + reportSurveysTaken.Rows[0]["SurveysTaken"].ToString() + " survey(s).";
            lblTotalSurveyStudents.Text += "<br /><i>Note: Survey responses are based upon the number of individuals that responded to the specific question."; 

            // display the individual questions
            BuildTable();
        }
        // Display School, District or State Name
        if (reportEntity.Rows.Count > 0)
            DisplayEntityName(reportEntity.Rows[0]);

        // Display Special Survey Notes
        DisplaySpecialNotes();
    }
    private void DisplayEntityName(DataRow drEntity)
    {
        {
            if (SchoolID != 0)
            {
                if (drEntity["EntityName"] != null)
                {
                    lblName.Font.Bold = true;
                    lblName.Text = "School: " + drEntity["EntityName"].ToString();
                }
            }
            else if (DistrictID != 0)
            {
                if (drEntity["EntityName"] != null)
                {
                    lblName.Font.Bold = true;
                    lblName.Text = "District: " + drEntity["EntityName"].ToString();
                }
            }
            else if (StateID != "" && StateID != null)
            {
                if (drEntity["EntityName"] != null)
                {
                    lblName.Font.Bold = true;
                    lblName.Text = "State: " + drEntity["EntityName"].ToString();
                }
            }
        }

    }
    private void BuildTable()
    {
        DataColumn[] colsParent, colsChild;
        colsParent = new DataColumn[] { reportQuestions.Columns["Survey_Id"], reportQuestions.Columns["Question_id"] };
        colsChild = new DataColumn[] { reportAnswers.Columns["Survey_Id"], reportAnswers.Columns["Question_id"] };
        reportData.Relations.Add("QuestionAnswer", colsParent, colsChild);
        //reportData.Relations.Add("QuestionAnswer", reportQuestions.Columns["question_Id"], reportAnswers.Columns["question_Id"]);

        // we need to keep track of sort since grid questions have a number of related questions
        string lastSort = ""; // use a string since we only need a comparison

        foreach (DataRow question in reportQuestions.Rows)
        {
            string quesSort = question["question_sort"].ToString();
            string quesType = question["question_type"].ToString();
            string gridText = question["question_grid_header"].ToString(); // blank unless a grid question
            string quesText = question["question_text"].ToString();
            if (quesSort != lastSort) 
            {
                // if this is a new question need to check if it is a grid question
                if (quesType == QuestionType.Grid || quesType == QuestionType.LikertGrid) // if a grid question, we need put 
                {
                    tblReport.Rows.Add(GridQuestionRow(quesSort, gridText));
                    tblReport.Rows.Add(QuestionRow("", quesText)); // if sort is blank, no question # will be displayed
                }
                else
                    tblReport.Rows.Add(QuestionRow(quesSort, quesText)); // if sort is blank, no question # will be displayed
            }
            else // if this is true, it's the second question or later in a grid
            {
                tblReport.Rows.Add(QuestionRow("", quesText)); // if sort is blank, no question # will be displayed
            }
            lastSort = quesSort;  
            if (quesType != QuestionType.Text)
            {
                // all questions have the header (if not a text question)
                tblReport.Rows.Add(OptionHeadersRow(quesType));
                bool evenRow = false;
                foreach (DataRow answer in question.GetChildRows("QuestionAnswer"))
                {
                    string response = answer["option_text"].ToString();
                    string numReponses = answer["num_students_per_option"].ToString();
                    if (numReponses.Length == 0) numReponses = "0";
                    string percResponse = answer["perc_students"].ToString();
                    if (percResponse.Length == 0) percResponse = "0";
                    string percNatlResponse = answer["perc_national"].ToString();
                    if (percNatlResponse.Length == 0) percNatlResponse = "0";
                    // if a school we will want to show the district percent, if a district we will want to show the state percent
                    // nothing to do if a state since national always included
                    string percNextLevel = "";
                    if (SchoolID != 0)
                    {
                        percNextLevel = answer["dist_perc_students"].ToString();
                        if (percNextLevel.Length == 0) percNextLevel = "0";
                    }
                    else if (DistrictID != 0)
                    {
                        percNextLevel = answer["state_perc_students"].ToString();
                        if (percNextLevel.Length == 0) percNextLevel = "0";
                    }
                    else
                        percNextLevel = "";
                    tblReport.Rows.Add(ResponseRow(response, numReponses, percResponse, percNextLevel, percNatlResponse, evenRow, quesType));
                    evenRow = !evenRow;
                }
            }
            else
                tblReport.Rows.Add(OpenEndedQuesRow());
        } 
    }
    private void DisplaySpecialNotes()
    {
        if (SurveyID == 6)
        {
            lblSpecialNotes.Text = "Please note: The K-2 Group survey required the survey taker to enter the " +
            "total number of students responding to each of the questions.  In an effort to include all student " +
            "responses, we reviewed all of the non-numeric responses and assigned the appropriate values.  In cases " +
            "where we could not ascertain the group count, we assigned a value of 1.  If you have specific questions " +
            "about your group survey, please contact our office at speakup@tomorrow.org";
        }
   }
    #endregion

    #region Display Table Handlers
    private TableRow GridQuestionRow(string sort, string gridText)
    {
        TableRow row = new TableRow();
        // add in the question number if sort non-blank
        row.Cells.Add(QuesNumCell(sort));

         TableCell cell = new TableCell();
       // add the question text
        cell = new TableCell();
        cell.ColumnSpan = 4;
        cell.HorizontalAlign = HorizontalAlign.Left;
        cell.VerticalAlign = VerticalAlign.Middle;
        cell.Font.Bold = true;
        cell.Text = gridText;
        row.Cells.Add(cell);

        return row;
    }
    private TableRow QuestionRow(string sort, string quesText)
    {
        TableRow row = new TableRow();
        // add in the question number if sort non-blank
        row.Cells.Add(QuesNumCell(sort));

        TableCell cell = new TableCell();
        // add the question text
        cell.ColumnSpan = 4;
        cell.HorizontalAlign = HorizontalAlign.Left;
        cell.VerticalAlign = VerticalAlign.Middle;
        cell.Font.Bold = true;
        cell.Text = quesText;
        row.Cells.Add(cell);

        return row;
    }
    private TableRow OptionHeadersRow(string quesType)
    {
        TableRow row = new TableRow();
        row.Font.Size = FontUnit.Point(9);
        row.BackColor = System.Drawing.ColorTranslator.FromHtml("#75ACD3");
        row.Cells.Add(SpacerCell());
        row.Cells.Add(TextCell("Response", 46, bold, HorizontalAlign.Left));
        row.Cells.Add(TextCell("# of Responses", 18, bold, HorizontalAlign.Right));
        if (quesType != QuestionType.Integer || isGroupSurvey) // if it is not an integer question or if it is and it is a group survey
        {
            row.Cells.Add(TextCell("% Responses", 18, bold, HorizontalAlign.Right));
            if (SchoolID != 0) // if a school we show the district percent
                row.Cells.Add(TextCell("District %", 18, bold, HorizontalAlign.Right));

            else if (DistrictID != 0) // if a district we show the state percent
                row.Cells.Add(TextCell("State %", 18, bold, HorizontalAlign.Right));
            row.Cells.Add(TextCell("National %", 18, bold, HorizontalAlign.Right));
        }
        else // integer question and not a group survey
        {
            row.Cells.Add(TextCell("Average of Responses", 18, bold, HorizontalAlign.Right));
            if (SchoolID != 0) // if a school we show the district percent
                row.Cells.Add(TextCell("District Average", 18, bold, HorizontalAlign.Right));

            else if (DistrictID != 0) // if a district we show the state percent
                row.Cells.Add(TextCell("State Average", 18, bold, HorizontalAlign.Right));
            row.Cells.Add(TextCell("National Average", 18, bold, HorizontalAlign.Right));
        }

        return row;
    }
    private TableRow ResponseRow(string respText, string numResp, string percResp, string percNextLevel, string percNatlResp, bool evenRow, string quesType)
    {
        TableRow row = new TableRow();
        if (evenRow) // alternating grey background on options
            row.BackColor = System.Drawing.ColorTranslator.FromHtml("#EFEFEF");
        row.Cells.Add(SpacerCell());
        row.Cells.Add(TextCell(respText, 46, notBold, HorizontalAlign.Left));
        row.Cells.Add(TextCell(numResp, 18, notBold, HorizontalAlign.Right));
        if (quesType != QuestionType.Integer || isGroupSurvey)
        {
            row.Cells.Add(TextCell(percResp + "%", 18, notBold, HorizontalAlign.Right));
            if (percNextLevel.Length > 0)
                row.Cells.Add(TextCell(percNextLevel + "%", 18, notBold, HorizontalAlign.Right));
            row.Cells.Add(TextCell(percNatlResp + "%", 18, notBold, HorizontalAlign.Right));
        }
        else
        {
            row.Cells.Add(TextCell(percResp, 18, notBold, HorizontalAlign.Right));
            if (percNextLevel.Length > 0)
                row.Cells.Add(TextCell(percNextLevel, 18, notBold, HorizontalAlign.Right));
            row.Cells.Add(TextCell(percNatlResp, 18, notBold, HorizontalAlign.Right));
        }
        return row;
    }
    private TableRow OpenEndedQuesRow()
    {
        TableRow row = new TableRow();

        row.Cells.Add(SpacerCell());

        TableCell cell = new TableCell();
        cell.ColumnSpan = 4;
        cell.Text = "<i>Note:</i>You can print your school or district open-ended responses from the survey print screen.  If you need assistance, please contact <a href='mailto:speakup@tomorrow.org'>speakup@tomorrow.org</a>";
        row.Cells.Add(cell);
        return row;
    }

    private TableCell TextCell(string cellText, int widthPerc, bool isBold, HorizontalAlign textAlign)
    {
        TableCell cell = new TableCell();
        cell.Width = Unit.Percentage(widthPerc);
        cell.HorizontalAlign = textAlign;
        cell.Font.Bold = isBold;
        cell.Text = cellText;

        return cell;
    }
    private TableCell SpacerCell()
    {
        TableCell cell = new TableCell();
        return cell;
    }
    private TableCell QuesNumCell(string quesNum)
    {
        TableCell cell = new TableCell();
        cell.VerticalAlign = VerticalAlign.Top;
        if (quesNum.Length > 0) // only add a literal if a question number is available (not available with grids)
        {
            Literal lit = new Literal();
            lit.Text = "<div class=\"numberBack\">" + quesNum + "</div>";
            cell.Controls.Add(lit);
        }
        return cell;
    }
    /* obselete in 2014 version of survey
    private Image GetImage(string sort)
    {
        string imgURL = "";
        if (sort.Length == 0)
            imgURL = "Images/transparent_spacer.gif";
        else if (sort.Length == 1) // add a leading zero to images 1 - 9
            imgURL = "Images/0" + sort + ".gif";
        else // sort >= 10
            imgURL = "Images/" + sort + ".gif";

        Image img = new Image();
        img.Width = 33;
        img.ImageUrl = mainSiteURL + imgURL;

        return img;
    }    
     */
    #endregion

    #region Events
    private void GetQueryStrings()
    {
        /*try
        {*/
            if (Request.QueryString["school_id"] != null)
            {
                SchoolID = int.Parse(Request.QueryString["school_id"]);
            }
            else
            {
                SchoolID = 0;
            }

            if (Request.QueryString["survey_id"] != null)
            {
                SurveyID = int.Parse(Request.QueryString["survey_id"]);
            }
            else if (Session["SURVEYID"] != null) // added 2010 for SETDA
            {
                SurveyID = Convert.ToInt32(Session["SURVEYID"]);
            }
            else
            {
                SurveyID = 0;
            }
            
            if (Request.QueryString["district_id"] != null)
            {
                DistrictID = int.Parse(Request.QueryString["district_id"]);
            }
            else
            {
                DistrictID = 0;
            }
            
            if (Request.QueryString["state_id"] != null && Request.QueryString["state_id"] != "")
            {
                StateID = Request.QueryString["state_id"].ToString().Trim();
            }
            else if (Session["STATE"] != null) // added 2010 for SETDA
            {
                StateID = (string)Session["STATE"];
            }
            else
            {
                StateID = "''";
            }
        //}
        //catch (Exception ex)
        //{
            //throw ex;
        //}
    }

    public string formatPercentage(string strPercent)
    {
        return strPercent + "%";
    }
    #endregion

}




