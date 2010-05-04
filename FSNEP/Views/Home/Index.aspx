<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="indexTitle" ContentPlaceHolderID="titleContent" runat="server">
    Home Page
</asp:Content>

<asp:Content ID="indexContent" ContentPlaceHolderID="mainContent" runat="server">
    <h2><%= Html.Encode(ViewData["Message"]) %></h2>
    
    <h3>Quick Links:</h3>
        
        <p>
            <%= Html.ActionLink("Project", "Projects", "Lookup") %>
        </p>
        <p>
            <%= Html.ActionLink("User Admin", "List", "UserAdministration") %>
        </p>
    
</asp:Content>
