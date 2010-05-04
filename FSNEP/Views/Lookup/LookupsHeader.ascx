<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%@ Import Namespace="FSNEP.Controllers"%>

<%= Html.ActionLink<LookupController>(a=>a.Projects(), "Projects") %>
<%= Html.ActionLink<LookupController>(a=>a.Accounts(), "Accounts") %>
<%= Html.ActionLink<LookupController>(a=>a.ExpenseTypes(), "Expense Types") %>
<%= Html.ActionLink<LookupController>(a=>a.ActivityCategories(), "Activity Categories") %>
<%= Html.ActionLink<LookupController>(a=>a.ActivityTypes(), "Activity Types") %>
