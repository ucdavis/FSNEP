<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Reset Password
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Reset Password</h2>
    
    <%  using (Html.BeginForm()) { %>
    <p>
        Answer the following question to receive your password.
    </p>
    <p>
        <%= this.Literal(ViewData["UserName"]).Label("User Name: ")%>
        <%= this.Hidden("username").Value(ViewData["UserName"])%>
    </p>
    <p>
        <%= this.Literal(ViewData["PasswordQuestion"]).Label("Question: ")%>
    </p>
    <p>
       <%= this.TextBox("passwordAnswer").Label("Answer: ")%> 
    </p>
    <p>
        <%= this.SubmitButton("Submit")%>
    </p>
    <h3 class="error"><%= Html.Encode(ViewData["Message"]) %></h3>

    <% } %>
</asp:Content>
