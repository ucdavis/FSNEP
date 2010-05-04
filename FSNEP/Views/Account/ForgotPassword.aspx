<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Forgot Password
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Forgot Password</h2>
    
    <% using(Html.BeginForm())
       {%>
    
    <p>
        Enter your username to receive your password
    </p>
    <p>
        <%= this.TextBox("id").Label("User Name: ") %>
    </p>
    <p>
        <input type="submit" value="Submit" />
    </p>
    <h3 class="error"><%= Html.Encode(ViewData["Message"]) %></h3>

    <%
       }%>
</asp:Content>
