<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<UserPermissionsViewModel>" %>

<%@ Import Namespace="FSNEP.Controllers" %>
<asp:Content ID="indexTitle" ContentPlaceHolderID="titleContent" runat="server">
    Home Page
</asp:Content>
<asp:Content ID="indexContent" ContentPlaceHolderID="mainContent" runat="server">
    <h2>
        Quick Links:</h2>
        
    <% Html.RenderPartial("UserPermissions"); %>
    
    <div class="menu-section timerecords">
    <h3>
        Time Records</h3>
        <img src="<%= Url.Content("~/Images/menu-time.png") %>" alt="Time Records" />
    <ul>
        <li>
            <%=Html.ActionLink("View Time Record #2", "Entry", "TimeRecord", new {id = 2}, null)%></li>
        <li>
            <%=Html.ActionLink<TimeRecordController>(x => x.Current(), "Current time record")%></li>
        <li>
            <%=Html.ActionLink<TimeRecordController>(x => x.History(), "Time Record History")%></li>
    </ul></div>
    <div class="menu-section costshare">
    <h3>
        Cost Share</h3>
        <img src="<%= Url.Content("~/Images/menu-share.png") %>" alt="Cost Share" />
    <ul>
        <li>
            <%=Html.ActionLink<CostShareController>(x => x.Current(), "Current Cost Share")%></li>
        <li><%=Html.ActionLink<CostShareController>(x => x.History(), "Cost Share History")%></li>
    </ul></div>
    <div class="menu-section supervisor">
    <h3>Supervisor</h3>
        <img src="<%= Url.Content("~/Images/menu-supe.png") %>" alt="Supervisor" />
    <ul>
        <li><%= Html.ActionLink<SupervisorController>(x=>x.TimeRecordList(), "Time Record Review") %></li>
        <li><%= Html.ActionLink<SupervisorController>(x=>x.CostShareList(), "Cost Share Review") %></li>
        <li><%= Html.ActionLink<SupervisorController>(x=>x.Delegate(), "Assign/Remove Delegates") %></li>
    </ul></div>
    <div class="menu-section admin">
    <h3>
        Administration</h3>
        <img src="<%= Url.Content("~/Images/menu-admin.png") %>" alt="Administration" />
    <ul>
        <li>
            <%=Html.ActionLink("Lookups (Project)", "Projects", "Lookup")%></li>
        <li>
            <%=Html.ActionLink("Associations (Project/Account)", "Projects", "Association")%></li>
        <li>
            <%=Html.ActionLink("User Admin", "List", "UserAdministration")%></li>
        <li>
            <%=Html.ActionLink("Cost Share Audit", "CostShareHistory", "Audit") %>
        </li>
        <li>
            <%=Html.ActionLink("Time Record Audit", "TimeRecordHistory", "Audit") %>
        </li>
            
    </ul></div>
    <div class="menu-section reports">
        <h3>Reports</h3>
        <img src="<%= Url.Content("~/Images/menu-reports.png") %>" alt="Reports" />
    <ul>
        <li>
            <%= Html.ActionLink("Cost Share", "CostShare", "Report") %>
        </li>
        <li>
            <%= Html.ActionLink<ReportController>(x=>x.TimeRecord(), "Time Record") %>
        </li>
    </ul></div>
</asp:Content>
