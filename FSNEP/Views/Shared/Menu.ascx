<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<UserPermissionsViewModel>" %>
<ul id="menu">
    <li class="home">
        <%=Html.ActionLink<HomeController>(x=>x.Index(), "Home")%></li>
    <% if (Model.IsTimeRecordUser)
       { %>
    <li class="time">
        <%=Html.ActionLink<TimeRecordController>(x => x.Current(), "Time Records")%>
        <ul id="TimeRecordMenu">
            <li>
                <%=Html.ActionLink<TimeRecordController>(x => x.Current(), "Current time record")%></li>
            <li>
                <%=Html.ActionLink<TimeRecordController>(x => x.History(), "Time Record History")%></li>
        </ul>
        <% } %></li>
    <% if (Model.IsCostShareUser)
       { %>
    <li class="cost">
        <%=Html.ActionLink<CostShareController>(x => x.Current(), "Local Support")%>
        <ul id="CostShareMenu">
            <li>
                <%=Html.ActionLink<CostShareController>(x => x.Current(), "Current Local Support")%></li>
            <li>
                <%=Html.ActionLink<CostShareController>(x => x.History(), "Local Support History")%></li>
        </ul>
        <% } %>
    </li>
    <% if (Model.IsSupervisor)
       { %>
    <li class="super">
        <%=Html.ActionLink<SupervisorController>(x => x.TimeRecordList(), "Supervisor")%>
        <ul id="SupervisorMenu">
            <li>
                <%= Html.ActionLink<SupervisorController>(x=>x.TimeRecordList(), "Time Record Review") %></li>
            <li>
                <%= Html.ActionLink<SupervisorController>(x=>x.CostShareList(), "Local Support Review") %></li>
            <li>
                <%= Html.ActionLink<SupervisorController>(x=>x.Delegate(), "Assign/Remove Delegates") %></li>
        </ul>
        <% } %></li>
    <% if (Model.IsAdmin || Model.IsProjectAdmin)
       { %>
    <li class="admin">
        <%=Html.ActionLink<UserAdministrationController>(x => x.List(null), "Administration")%>
        <ul id="AdministrationMenu">
            <li>
                <%=Html.ActionLink("Lookups", "Projects", "Lookup")%></li>
            <li>
                <%=Html.ActionLink("Associations", "Projects", "Association")%></li>
            <li>
                <%=Html.ActionLink("User Admin", "List", "UserAdministration")%></li>
            <li>
                <%=Html.ActionLink("Local Support Audit", "CostShareHistory", "Audit") %>
            </li>
            <li>
                <%=Html.ActionLink("Time Record Audit", "TimeRecordHistory", "Audit") %>
            </li>
        </ul>
        <% } %></li>
    <% if (Model.IsAdmin || Model.IsProjectAdmin)
       { //Figure out who can see which reports %>
    <li class="reports">
        <%=Html.ActionLink<ReportController>(x => x.TimeRecord(), "Reports")%>
        <ul id="ReportsMenu">
            <li>
                <%= Html.ActionLink("Local Support", "CostShare", "Report") %>
            </li>
            <li>
                <%= Html.ActionLink<ReportController>(x=>x.TimeRecord(), "Time Record") %>
            </li>
        </ul>
        <% } %></li>
</ul>
