<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<FSNEP.Controllers.UserViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
    ModifyUser
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Modify User</h2>
    <%= Html.ValidationSummary("User modification was unsuccessful. Please correct the errors and try again.") %>
    <% using (Html.BeginForm())
       {%>
    <%--<% Html.RenderPartial("UserInformation"); %>--%>
    <fieldset>
        <legend>User Information</legend>
        <table>
            <tr>
                <td>
                    First Name:
                </td>
                <td>
                    <%= Html.TextBoxFor(u=>u.User.FirstName) %>
                </td>
                <td>
                    <%= Html.ValidationMessageFor(u=>u.User.FirstName) %>
                </td>
            </tr>
            <tr>
                <td>
                    Last Name:
                </td>
                <td>
                    <%= Html.TextBoxFor(u=>u.User.LastName) %>
                </td>
                <td>
                    <%= Html.ValidationMessageFor(u => u.User.LastName)%>
                </td>
            </tr>
            <tr>
                <td>
                    Salary:
                </td>
                <td>
                    <%= Html.TextBox("User.Salary", Model.User.Salary == 0 ? string.Empty : Model.User.Salary.ToString()) %>
                </td>
                <td>
                    <%= Html.ValidationMessageFor(u=>u.User.Salary) %>
                </td>
            </tr>
            <tr>
                <td>
                    FTE:
                </td>
                <td>
                    <%= Html.TextBoxFor(u=>u.User.FTE) %>
                </td>
                <td>
                    <%= Html.ValidationMessageFor(u => u.User.FTE)%>
                </td>
            </tr>
            <tr>
                <td>
                    Benefit Rate:
                </td>
                <td>
                    <%= Html.TextBoxFor(u=>u.User.BenefitRate) %>
                </td>
                <td>
                    <%= Html.ValidationMessageFor(u=>u.User.BenefitRate) %>
                </td>
            </tr>
            <tr>
                <td>
                    Active User?
                </td>
                <td>
                    <%= Html.CheckBox("User.IsActive") %>
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td>
                    Supervisor:
                </td>
                <td>
                    <%= Html.DropDownListFor(a=>a.User.Supervisor, Model.Supervisors, "Select A Supervisor") %>
                </td>
                <td>
                    <%= Html.ValidationMessage("SupervisorID") %>
                </td>
            </tr>
            <tr>
                <td>
                    Projects:
                </td>
                <td>
                    <%= this.MultiSelect("ProjectList").Options(Model.Projects) %>
                </td>
                <td>
                    <%= Html.ValidationMessage("ProjectList") %>
                </td>
            </tr>
            <tr>
                <td>
                    Fund Types:
                </td>
                <td>
                    <%= this.MultiSelect("FundTypeList").Options(Model.FundTypes) %>
                </td>
                <td>
                    <%= Html.ValidationMessage("FundTypeList") %>
                </td>
            </tr>
        </table>
    </fieldset>
    <br />
    <input type="submit" value="Modify User" />
    <% } %>
    <div>
        <%=Html.ActionLink("Back to List", "List", "UserAdministration") %>
    </div>
</asp:Content>
