<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<FSNEP.Controllers.UserViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	ModifyUser
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<h2>Modify User</h2>

    <%= Html.ValidationSummary("User modification was unsuccessful. Please correct the errors and try again.") %>

    <% using (Html.BeginForm()) {%>

        <h3>
            Modifying <%= Html.Encode(string.Format("{0} ({1})", Model.User.FullName, Model.User.UserName)) %>
        </h3>

        <% Html.RenderPartial("UserInformation"); %>
        
        <br />
        <input type="submit" value="Modify User" />
        
    <% } %>

    <div>
        <%=Html.ActionLink("Back to List", "List", "UserAdministration") %>
    </div>

</asp:Content>

