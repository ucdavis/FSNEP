<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<FSNEP.Core.Domain.User>" %>

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
                    <td><%= Html.TextBoxFor(u=>u.FirstName) %></td>
                    <td><%= Html.ValidationMessageFor(u=>u.FirstName) %></td>
                </tr>
                <tr>
                    <td>Last Name: </td>
                    <td><%= Html.TextBoxFor(u=>u.LastName) %></td>
                    <td><%= Html.ValidationMessageFor(u => u.LastName)%></td>
                </tr>
                <tr>
                    <td>Salary: </td>
                    <td><%= Html.TextBoxFor(u=>u.Salary) %></td>
                    <td><%= Html.ValidationMessageFor(u=>u.Salary) %></td>
                </tr>
                <tr>
                    <td>Benefit Rate: </td>
                    <td><%= Html.TextBoxFor(u=>u.BenefitRate) %></td>
                    <td><%= Html.ValidationMessageFor(u=>u.BenefitRate) %></td>
                </tr>
                <tr>
                    <td>Supervisor: </td>
                    <td><%= Html.DropDownList("SupervisorID", (SelectList)ViewData["Supervisors"], "Select A Supervisor") %></td>
                    <td><%= Html.ValidationMessage("SupervisorID") %></td>
                </tr>
                <tr>
                    <td>Projects: </td>
                    <td><%= this.MultiSelect("ProjectList").Options((MultiSelectList)ViewData["Projects"]) %></td>
                    <td><%= Html.ValidationMessage("ProjectList") %></td>
                </tr>
                <tr>
                    <td>Fund Types: </td>
                    <td><%= this.MultiSelect("FundTypeList").Options((MultiSelectList)ViewData["FundTypes"]) %></td>
                    <td><%= Html.ValidationMessage("FundTypeList") %></td>
                </tr>
            </table>
        </fieldset>
        
        <% if (Model.IsTransient()) { %>
                <input type="submit" value="Create User" />
        <% } else { %>
                <input type="submit" value="Modify User" />
        <% } %>
    
    <% } %>

    <div>
        <%=Html.ActionLink("Back to List", "Index") %>
    </div>

</asp:Content>

