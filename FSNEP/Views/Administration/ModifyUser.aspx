<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<FSNEP.Controllers.ModifyUserViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	ModifyUser
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<h2>Modify User</h2>

    <%= Html.ValidationSummary("User modification was unsuccessful. Please correct the errors and try again.") %>

    <% using (Html.BeginForm()) {%>

        <fieldset>
            <legend>User Account Info</legend>
            <p>
            
            </p>
        </fieldset>

        <br />

        <fieldset>
            <legend>User Information</legend>
            <table>
                <tr>
                    <td>First Name: </td>
                    <td><%= Html.TextBoxFor(u=>u.User.FirstName) %></td>
                    <td><%= Html.ValidationMessageFor(u=>u.User.FirstName) %></td>
                </tr>
                <tr>
                    <td>Last Name: </td>
                    <td><%= Html.TextBoxFor(u=>u.User.LastName) %></td>
                    <td><%= Html.ValidationMessageFor(u => u.User.LastName)%></td>
                </tr>
                <tr>
                    <td>Salary: </td>
                    <td><%= Html.TextBoxFor(u=>u.User.Salary) %></td>
                    <td><%= Html.ValidationMessageFor(u=>u.User.Salary) %></td>
                </tr>
                <tr>
                    <td>Benefit Rate: </td>
                    <td><%= Html.TextBoxFor(u=>u.User.BenefitRate) %></td>
                    <td><%= Html.ValidationMessageFor(u=>u.User.BenefitRate) %></td>
                </tr>
                <tr>
                    <td>Supervisor: </td>
                    <td><%= Html.DropDownList("Supervisor", Model.Supervisors, "Select A Supervisor") %></td>
                    <td><%= Html.ValidationMessageFor(u=>u.User.Supervisor) %></td>
                </tr>
            </table>
        </fieldset>
        
        <input type="submit" value="Create User" />

    <% } %>

    <div>
        <%=Html.ActionLink("Back to List", "Index") %>
    </div>

</asp:Content>

