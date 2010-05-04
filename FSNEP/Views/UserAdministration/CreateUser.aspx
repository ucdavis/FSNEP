<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<FSNEP.Controllers.CreateUserViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
    Create User
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Create User</h2>
        
        <%= Html.ValidationSummary("User creation was unsuccessful. Please correct the errors and try again.") %>

    <% using (Html.BeginForm()) {%>
        
    <fieldset>
        <legend>New User Info</legend>
        <table>
            <tr>
                <td>
                    Username:
                </td>
                <td>
                    <%= Html.TextBoxFor(u=>u.UserName) %>
                </td>
                <td>
                    <%= Html.ValidationMessageFor(u=>u.UserName) %>
                </td>
            </tr>
            <tr>
                <td>
                    Email:
                </td>
                <td>
                    <%= Html.TextBoxFor(u=>u.Email) %>
                </td>
                <td>
                    <%= Html.ValidationMessageFor(u => u.Email)%>
                </td>
            </tr>
            <tr>
                <td>
                    Question:
                </td>
                <td>
                    <%= Html.TextBoxFor(u=>u.Question) %>
                </td>
                <td>
                    <%= Html.ValidationMessageFor(u => u.Question)%>
                </td>
            </tr>
            <tr>
                <td>
                    Answer:
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
        <%=Html.ActionLink("Back to List", "Index") %>
    </p>
</asp:Content>
