<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<FSNEP.Core.Domain.User>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Remove Delegate
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Remove Delegate</h2>

    <p><%= Html.Encode(Model.FullName) %> is currently assigned as your delegate</p>
    
    <% using(Html.BeginForm()) { %>
        <%= Html.AntiForgeryToken() %>
        
        <%= this.Hidden("UserId").Value(Model.Id) %>
        
        <p>
            <input type="submit" value="Click Here To Remove Your Delegate" />
        </p>

    <% } %>

</asp:Content>