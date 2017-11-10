<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="WebApplication9.WebForm1" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>

<!DOCTYPE html>

<html lang="en">
<head runat="server">
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <!-- The above 3 meta tags *must* come first in the head; any other head content must come *after* these tags -->
    <title></title>   
    <!-- Bootstrap -->
    <link href="speakup.css" rel="stylesheet" />
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css" integrity="sha384-BVYiiSIFeK1dGmJRAkycuHAHRg32OmUcww7on3RYdg4Va+PmSTsz/K68vbdEjh4u" crossorigin="anonymous">
</head>

<body>
    <form id="form1" runat="server">
        <div class="wrapper">
            <div class="container">
                <div class="row">
                    <div class="col-sm-1"></div>
                    <div class="col-sm-10">
                        <nav class="navbar navbar-default">
                            <div class="container-fluid">
                                <div class="navbar-header">
                                    <a class="navbar-brand" href="#">Project Tomorrow</a>
                                </div>
                                <ul class="nav navbar-nav">
                                    <li><a href="#">Home</a></li>
                                    <li><a href="#">Surveys</a></li>
                                    <li><a href="#">Logout</a></li>
                                </ul>
                            </div>
                        </nav>
                        <div class="page-header">
                            <h1 class="dashboard">Empty Dashboard <small>Subtext</small></h1>
                        </div>
                        <div class="metricsWrapper">
                            <div class="row">
                                <div class="col-sm-3">
                                    <div class="panel panel-success">
                                        <div class="panel-body">
                                            Panel content
                                        </div>
                                        <div class="panel-footer">Panel footer</div>
                                    </div>
                                </div>
                                <div class="col-sm-3">
                                    <div class="panel panel-success">
                                        <div class="panel-body">
                                            Panel content
                                        </div>
                                        <div class="panel-footer">Panel footer</div>
                                    </div>
                                </div>
                                <div class="col-sm-3">
                                    <div class="panel panel-success">
                                        <div class="panel-body">
                                            Panel content
                                        </div>
                                        <div class="panel-footer">Panel footer</div>
                                    </div>
                                </div>
                                <div class="col-sm-3">
                                    <div class="panel panel-success">
                                        <div class="panel-body">
                                            Panel content
                                        </div>
                                        <div class="panel-footer">Panel footer</div>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="questionfilter">
                                    <div class="input-group">
                                        <div class="input-group-addon">Questions</div>
                                        <asp:DropDownList
                                            ID="QuestionFilter"
                                            CssClass="form-control"
                                            runat="server">
                                        </asp:DropDownList>

                                    </div>
                                </div>
                            </div>
                            <br>
<%--                            <div class="row">
                                <div class="col-sm-6">
                                </div>
                                <div class="col-sm-6"></div>
                            </div>
                            <br>
                            <div class="row">
                                <div class="col-sm-6"></div>
                                <div class="col-sm-6"></div>
                            </div>--%>
                            <asp:Chart ID="Chart1" runat="server">
                                <Series>
                                    <asp:Series Name="Series1"></asp:Series>
                                </Series>
                                <ChartAreas>
                                    <asp:ChartArea Name="ChartArea1"></asp:ChartArea>
                                </ChartAreas>
                            </asp:Chart>
                        </div>

                    </div>
                </div>
                <div class="col-sm-1"></div>
            </div>
        </div>
    </form>

    <!-- Latest compiled and minified JavaScript -->
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js" integrity="sha384-Tc5IQib027qvyjSMfHjOMaLkfuWVxZxUPnCJA7l2mCWNIpG9mGCD8wGNIcPD7Txa" crossorigin="anonymous"></script>

</body>
</html>
