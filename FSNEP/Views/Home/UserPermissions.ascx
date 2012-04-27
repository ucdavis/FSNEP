<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<UserPermissionsViewModel>" %>
<h3>
    In Production You Can See</h3>
<ul>
    <li>Time Record:
        <%= Model.IsTimeRecordUser %>
    </li>
    <li>Local Support:
        <%= Model.IsCostShareUser %>
    </li>
    <li>Supervisor:
        <%= Model.IsSupervisor %>
    </li>
    <li>Administration:
        <%= Model.IsAdmin %>
    </li>
    <li>Project Admin:
        <%= Model.IsAdmin || Model.IsProjectAdmin%>
    </li>
</ul>
