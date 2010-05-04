<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<FSNEP.Controllers.CostShareEntryViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Entry
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2><%= Html.Encode(string.Format("Cost Share for {0}", Model.CostShare.Date.ToString("MMMM yyyy"))) %></h2>
    
    <% using(Html.BeginForm("AddEntry", "CostShare")) {%>
        
        Form here for new cost share
    
    <%
       }%>

    
</asp:Content>