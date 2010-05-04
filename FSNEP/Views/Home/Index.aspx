﻿<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>
<%@ Import Namespace="FSNEP.Controllers"%>

<asp:Content ID="indexTitle" ContentPlaceHolderID="titleContent" runat="server">
    Home Page
</asp:Content>

<asp:Content ID="indexContent" ContentPlaceHolderID="mainContent" runat="server">
    <h2>Quick Links:</h2>
        
        <h3>Records</h3>
        <ul>
            <li><%= Html.ActionLink("View Time Record #2", "Entry", "TimeRecord", new {id=2}, null) %></li>
            <li><%= Html.ActionLink<TimeRecordController>(x=>x.Current(), "Current time record") %></li>
            <li><%= Html.ActionLink<TimeRecordController>(x=>x.History(), "Time Record History") %></li>
        </ul>
        <h3>Administration</h3>
        <ul>
            <li><%= Html.ActionLink("Lookups (Project)", "Projects", "Lookup") %></li>
            <li><%= Html.ActionLink("Associations (Project/Account)", "Projects", "Association") %></li>
            <li><%= Html.ActionLink("User Admin", "List", "UserAdministration") %></li>
        </ul>
        
</asp:Content>
