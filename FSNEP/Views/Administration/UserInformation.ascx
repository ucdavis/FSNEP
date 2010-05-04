<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<FSNEP.Controllers.UserViewModel>" %>
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
                Supervisor:
            </td>
            <td>
                <%= Html.DropDownList("SupervisorID", Model.Supervisors, "Select A Supervisor") %>
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
