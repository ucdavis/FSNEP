<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<div id="menu">
    <ul id="TimeRecordMenu">
        <li>
            <%=Html.ActionLink<TimeRecordController>(x => x.Current(), "Current time record")%></li>
        <li>
            <%=Html.ActionLink<TimeRecordController>(x => x.History(), "Time Record History")%></li>
    </ul>
    <ul id="CostShareMenu">
        <li>
            <%=Html.ActionLink<CostShareController>(x => x.Current(), "Current Cost Share")%></li>
        <li>
            <%=Html.ActionLink<CostShareController>(x => x.History(), "Cost Share History")%></li>
    </ul>
    <ul id="SupervisorMenu">
        <li>
            <%= Html.ActionLink<SupervisorController>(x=>x.TimeRecordList(), "Time Record Review") %></li>
        <li>
            <%= Html.ActionLink<SupervisorController>(x=>x.CostShareList(), "Cost Share Review") %></li>
        <li>
            <%= Html.ActionLink<SupervisorController>(x=>x.Delegate(), "Assign/Remove Delegates") %></li>
    </ul>
    <ul id="AdministrationMenu">
        <li>
            <%=Html.ActionLink("Lookups", "Projects", "Lookup")%></li>
        <li>
            <%=Html.ActionLink("Associations", "Projects", "Association")%></li>
        <li>
            <%=Html.ActionLink("User Admin", "List", "UserAdministration")%></li>
        <li>
            <%=Html.ActionLink("Cost Share Audit", "CostShareHistory", "Audit") %>
        </li>
        <li>
            <%=Html.ActionLink("Time Record Audit", "TimeRecordHistory", "Audit") %>
        </li>
    </ul>
    <ul id="ReportsMenu">
        <li>
            <%= Html.ActionLink("Cost Share", "CostShare", "Report") %>
        </li>
        <li>
            <%= Html.ActionLink<ReportController>(x=>x.TimeRecord(), "Time Record") %>
        </li>
    </ul>
</div>
