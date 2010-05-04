<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<FSNEP.Controllers.CreateUserViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
    Create User
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Create User</h2>

    <%= Html.ClientSideValidation<FSNEP.Controllers.CreateUserViewModel>() %>        

    <%= Html.ValidationSummary("User creation was unsuccessful. Please correct the errors and try again.") %>

    <% using (Html.BeginForm()) {%>
    <%= Html.AntiForgeryToken() %>
    
    <fieldset>
        <legend>New User Info</legend>
        <table>
            <tr>
                <td><label>
                    Username:</label>
                </td>
                <td>
                    <%= Html.TextBoxFor(u=>u.UserName) %>
                </td>
                <td>
                    <%= Html.ValidationMessageFor(u=>u.UserName) %>
                </td>
            </tr>
            <tr>
                <td><label>
                    Email:</label>
                </td>
                <td>
                    <%= Html.TextBoxFor(u=>u.Email) %>
                </td>
                <td>
                    <%= Html.ValidationMessageFor(u => u.Email)%>
                </td>
            </tr>
            <tr>
                <td><label>
                    Question:</label>
                </td>
                <td>
                    <%= Html.TextBoxFor(u=>u.Question) %>
                </td>
                <td>
                    <%= Html.ValidationMessageFor(u => u.Question)%>
                </td>
            </tr>
            <tr>
                <td><label>
                    Answer:</label>
                </td>
                <td>
                    <%= Html.TextBoxFor(u=>u.Answer) %>
                </td>
                <td>
                    <%= Html.ValidationMessageFor(u=>u.Answer) %>
                </td>
            </tr>
        </table>
    </fieldset>
    <br />
    <% Html.RenderPartial("UserInformation"); %>
    
    <br />
    <input type="submit" value="Create User" />
    
     <% } %>
    <p>
        <%=Html.ActionLink("Back to List", "List", "UserAdministration") %>
    </p>
</asp:Content>
