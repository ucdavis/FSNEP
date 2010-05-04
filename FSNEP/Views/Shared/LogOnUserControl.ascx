<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>
<%
    if (Request.IsAuthenticated) {
%>
        Welcome <b><%= Html.Encode(Page.User.Identity.Name) %></b>! — 
        <%= Html.ActionLink("Log out", "LogOff", "Account") %> | <%= Html.ActionLink("Change your password", "ChangePassword", "Account") %>
<%
    }
    else {
%> 
        | <%= Html.ActionLink("Log in", "LogOn", "Account") %> |
<%
    }
%>
