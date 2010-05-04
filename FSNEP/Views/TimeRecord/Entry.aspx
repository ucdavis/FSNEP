<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<FSNEP.Controllers.TimeRecordEntryViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
    Entry
</asp:Content>
<asp:Content ContentPlaceHolderID="AdditionalScripts" runat="server">
    <script src='<%= Url.Content("~/Scripts/jquery.message.min.js") %>' type="text/javascript"></script>
    <script src='<%= Url.Content("~/Scripts/TimeRecordEntry.js") %>' type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2><%= Html.Encode(string.Format("Time Record for {0}", Model.TimeRecord.Date.ToString("MMMM yyyy"))) %></h2>
            
    <% Html.RenderPartial("Calendar"); %>
    
    <div id="SubmitRecord">
        <% using (Html.BeginForm("Submit", "TimeRecord", new {id = Model.TimeRecord.Id})) { %>
            <%= Html.AntiForgeryToken() %>
        
            <input type="submit" value="Submit Final" />
        
        <% } %>
    </div>
    
</asp:Content>
