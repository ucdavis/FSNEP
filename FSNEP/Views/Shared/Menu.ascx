<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<UserPermissionsViewModel>" %>

<ul id="menu">
    <li class="home"><%=Html.ActionLink<HomeController>(x=>x.Index(), "Home")%></li>
            
    <li class="time"><%=Html.ActionLink<TimeRecordController>(x => x.Current(), "Time Records")%>
    <% if (Model.IsTimeRecordUser || true) { %>
    <ul id="TimeRecordMenu">
        <li>
            <%=Html.ActionLink<TimeRecordController>(x => x.Current(), "Current time record")%></li>
        <li>
            <%=Html.ActionLink<TimeRecordController>(x => x.History(), "Time Record History")%></li>
    </ul>
    <% } %></li>
    <li class="cost"><%=Html.ActionLink<CostShareController>(x => x.Current(), "Cost Share")%>
    <% if (Model.IsTimeRecordUser || true)
       { %>    
    <ul id="CostShareMenu">
        <li>
            <%=Html.ActionLink<CostShareController>(x => x.Current(), "Current Cost Share")%></li>
        <li>
            <%=Html.ActionLink<CostShareController>(x => x.History(), "Cost Share History")%></li>
    </ul>
    <% } %></li>
    <li class="super"><%=Html.ActionLink<SupervisorController>(x => x.TimeRecordList(), "Supervisor")%>
    <% if (Model.IsTimeRecordUser || true)
       { %>    
    <ul id="SupervisorMenu">
        <li>
            <%= Html.ActionLink<SupervisorController>(x=>x.TimeRecordList(), "Time Record Review") %></li>
        <li>
            <%= Html.ActionLink<SupervisorController>(x=>x.CostShareList(), "Cost Share Review") %></li>
        <li>
            <%= Html.ActionLink<SupervisorController>(x=>x.Delegate(), "Assign/Remove Delegates") %></li>
    </ul>
    <% } %></li>
    <li class="admin"><%=Html.ActionLink<UserAdministrationController>(x => x.List(null), "Administration")%>
    <% if (Model.IsTimeRecordUser || true)
       { %>    
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
    <% } %></li>
    <li class="reports"><%=Html.ActionLink<ReportController>(x => x.TimeRecord(), "Reports")%>
    <% if (false || true)
       { //Figure out who can see which reports %>
    <ul id="ReportsMenu">
        <li>
            <%= Html.ActionLink("Cost Share", "CostShare", "Report") %>
        </li>
        <li>
            <%= Html.ActionLink<ReportController>(x=>x.TimeRecord(), "Time Record") %>
        </li>
    </ul>
    <% } %></li>
    </ul>