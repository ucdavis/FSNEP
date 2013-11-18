<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<TimeRecordEntryViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
    Entry
</asp:Content>
<asp:Content ContentPlaceHolderID="AdditionalScripts" runat="server">
    <style>
        .notification-message 
        {
            padding: 10px;
            background-color: #F2EBCC;
            border: 1px solid #E4C466;
        }
    </style>
    <%--<link type="text/css" href="<%= Url.Content("~/Content/jquery.jgrowl.css") %>" rel="Stylesheet" />--%>
    <script src='<%= Url.Content("~/Scripts/jquery.jgrowl.min.js") %>' type="text/javascript"></script>
    <script type="text/javascript">
        var imageBase = '<%= Url.Image("") %>';
    </script>
    <script src='<%= Url.Content("~/Scripts/TimeRecordEntry.js") %>' type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
            
    <span class="notification-message">Please print out and sign your time records bi-weekly. <a href="<%= Url.Content("~/Content/FSNEP-reporting-dates.pdf") %>">Click here to view/download a chart of the time periods for FY14</a></span>
    <br/><br/><br/>
    <div style="margin: 0pt auto; width: 400px; color: rgb(24, 75, 93); font-size: 24px; font-weight: bold; text-align: center;">
        <span id="ctl00_ContentPlaceHolder1_lblCalendarInfo">
            <%= Html.Encode(string.Format("Time Record for {0}", Model.TimeRecord.Date.ToString("MMMM yyyy"))) %>
        </span>
    </div>
    
    <br />
            
    <div style="margin: 0pt auto; width: 300px; color: rgb(24, 75, 93); font-size: 12px;
        font-weight: bold; text-align: center;">
        <%= Html.ActionLink("[Click to View Activity Type Definitions]", "ListOfActivitiesAndDescriptions", null, new { target = "_blank" })%>
    </div>
    
    <br />
    
    <% Html.RenderPartial("Calendar"); %>
    
    <div id="SubmitRecord">
        <% using (Html.BeginForm("Submit", "TimeRecord", new {id = Model.TimeRecord.Id})) { %>
            <%= Html.AntiForgeryToken() %>
        
            <input class="submitFinal" type="submit" value="Submit Final" />
        
        <% } %>
    </div>
    
</asp:Content>
