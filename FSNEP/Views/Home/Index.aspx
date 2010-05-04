<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<UserPermissionsViewModel>" %>

<%@ Import Namespace="FSNEP.Controllers" %>
<asp:Content ID="indexTitle" ContentPlaceHolderID="titleContent" runat="server">
    Home Page
</asp:Content>
<asp:Content ID="indexContent" ContentPlaceHolderID="mainContent" runat="server">
    <h2>
        Quick Links:</h2>
        
    <% Html.RenderPartial("UserPermissions"); %>
    
    <h3>
        Time Records</h3>
    <ul>
        <li>
            <%=Html.ActionLink("View Time Record #2", "Entry", "TimeRecord", new {id = 2}, null)%></li>
        <li>
            <%=Html.ActionLink<TimeRecordController>(x => x.Current(), "Current time record")%></li>
        <li>
            <%=Html.ActionLink<TimeRecordController>(x => x.History(), "Time Record History")%></li>
    </ul>
    <h3>
        Cost Share</h3>
    <ul>
        <li>
            <%=Html.ActionLink<CostShareController>(x => x.Current(), "Current Cost Share")%></li>
        <li><%=Html.ActionLink<CostShareController>(x => x.History(), "Cost Share History")%></li>
    </ul>
    <h3>Supervisor</h3>
    <ul>
        <li><%= Html.ActionLink<SupervisorController>(x=>x.TimeRecordList(), "Time Record Review") %></li>
        <li><%= Html.ActionLink<SupervisorController>(x=>x.CostShareList(), "Cost Share Review") %></li>
        <li><%= Html.ActionLink<SupervisorController>(x=>x.Delegate(), "Assign/Remove Delegates") %></li>
    </ul>
    <h3>
        Administration</h3>
    <ul>
        <li>
            <%=Html.ActionLink("Lookups (Project)", "Projects", "Lookup")%></li>
        <li>
            <%=Html.ActionLink("Associations (Project/Account)", "Projects", "Association")%></li>
        <li>
            <%=Html.ActionLink("User Admin", "List", "UserAdministration")%></li>
        <li>
            <%=Html.ActionLink("Cost Share Audit", "History", "CostShareAudit") %>
        </li>
            
    </ul>
        <h3>Reports</h3>
    <ul>
        <li>
            <%= Html.ActionLink("Cost Share", "CostShare", "Report") %>
        </li>
        <li>
            <%= Html.ActionLink<ReportController>(x=>x.TimeRecord(), "Time Record") %>
        </li>
    </ul>
</asp:Content>
