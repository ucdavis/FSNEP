<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<TimeRecordEntryViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
    Entry
</asp:Content>
<asp:Content ContentPlaceHolderID="AdditionalScripts" runat="server">
    <script src='<%= Url.Content("~/Scripts/jquery.message.min.js") %>' type="text/javascript"></script>
    <script src='<%= Url.Content("~/Scripts/TimeRecordEntry.js") %>' type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

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
        
            <input type="submit" value="Submit Final" />
        
        <% } %>
    </div>
    
</asp:Content>
