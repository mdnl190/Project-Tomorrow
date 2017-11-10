using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Web.UI.DataVisualization.Charting;

namespace WebApplication9
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        private static string conStr = WebConfigurationManager.ConnectionStrings["Speakup"].ConnectionString;
        private SqlConnection objConn = new SqlConnection(conStr);
        DataTable dt = new DataTable();

        //protected void Page_Load(object sender, EventArgs e)
        //{
        //    if (!IsPostBack)
        //    {
        //        //this.populateQuestionFilter();
        //    }
        //}

        protected void Page_Load(object o, EventArgs e)
        {
            if (!IsPostBack)
            {
                Chart1.Series.Add("Series2");
                Chart1.Series["Series2"].ChartType = SeriesChartType.Column;
                Chart1.Series["Series2"].Points.AddY(20);
                Chart1.Series["Series2"].ChartArea = "ChartArea1";

                ListItem item;
                item = new ListItem("Question 1", "1");
                QuestionFilter.Items.Add(item);
                item = new ListItem("Question 2", "2");
                QuestionFilter.Items.Add(item);
                item = new ListItem("Question 3", "3");
                QuestionFilter.Items.Add(item);

                QuestionFilter.Text = QuestionFilter.SelectedItem.Value;

            }
        }

        //private void populateQuestionFilter()
        //{
        //    //string constring = .ConnectionStrings["Constring"].ConnectionString;

        //    SqlCommand cmd = new SqlCommand("SELECT DISTINCT question_id,question_text FROM AllQuesOp");
        //    SqlDataAdapter objAdapt = new SqlDataAdapter(cmd);
        //    objAdapt.SelectCommand = cmd;
        //    objConn.Open();
        //    try
        //    {
        //        objAdapt.Fill(dt);
        //        QuestionFilter.DataSource = dt;
        //        QuestionFilter.DataTextField = "Select Question Filter";
        //        QuestionFilter.DataValueField = "question_text";
        //        QuestionFilter.DataBind();
        //        QuestionFilter.Items.Insert(0, new ListItem("====SELECT====", ""));
        //    }
        //    catch(SqlException err)
        //    {
        //        Console.WriteLine("Error");
        //    }
        //    finally
        //    {
        //        objConn.Close();
        //    }


        //}
    }
}