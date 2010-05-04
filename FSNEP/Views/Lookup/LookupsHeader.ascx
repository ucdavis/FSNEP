<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="FSNEP.Controllers"%>
<ul id="lookups-menu">
<li><%= Html.ActionLink<LookupController>(a=>a.Projects(), "Projects") %></li>
<li><%= Html.ActionLink<LookupController>(a=>a.Accounts(), "Accounts") %></li>
<li><%= Html.ActionLink<LookupController>(a=>a.ExpenseTypes(), "Expense Types") %></li>
<li><%= Html.ActionLink<LookupController>(a=>a.ActivityCategories(null), "Activity Categories") %></li>
<li><%= Html.ActionLink<LookupController>(a=>a.ActivityTypes(), "Activity Types") %></li>
<li><%= Html.ActionLink<LookupController>(a=>a.HoursInMonths(), "Hours In Months") %></li>
</ul>