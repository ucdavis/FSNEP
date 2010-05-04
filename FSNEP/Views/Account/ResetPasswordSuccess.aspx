<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Reset Password Success
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Reset Password Success</h2>

    <p>
        Your password has been reset. We will send you an email shortly which contains your temporary password: 
        Please change it to something you will remember by visiting the <%= Html.ActionLink("Password Change page", "ChangePassword") %>.
    </p>

</asp:Content>
