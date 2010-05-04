<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<UserViewModel>" %>
<%@ Import Namespace="FSNEP.Core.Domain"%>
    
<%= Html.ClientSideValidation<User>("User") %>
<%= Html.HiddenFor(a=>a.User.Id) %>
<fieldset>
    <legend>User Information</legend>
    <table>
        <tr>
            <td>
                <label>First Name:</label>
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
                <label>Last Name:</label>
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
                <label>Salary:</label>
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
                <label>FTE:</label>
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
                <label>Benefit Rate:</label>
            </td>
            <td>
                <%= Html.TextBoxFor(u=>u.User.BenefitRate) %>
            </td>
            <td>
                <%= Html.ValidationMessageFor(u=>u.User.BenefitRate) %>
            </td>
        </tr>
        <tr>
            <td><label>Active User?</label></td>
            <td>
                <%= Html.CheckBox("User.IsActive") %>
            </td>
            <td></td>
        </tr>
        <tr>
            <td>
                <label>Supervisor:</label>
            </td>
            <td>
                <%= this.Select("User.Supervisor")
                                    .FirstOption("--Select A Supervisor--")
                                    .HideFirstOptionWhen(Model.User.Supervisor != null)
                                    .Options(Model.Supervisors, s=>s.Id, s=>s.FullNameLastFirst)
                                    .Selected(Model.User.Supervisor != null ? Model.User.Supervisor.Id : Guid.Empty) %>
            </td>
            <td>
                <%= Html.ValidationMessageFor(a=>a.User.Supervisor) %>
            </td>
        </tr>
        <tr>
            <td>
                <label>Projects:</label>
            </td>
            <td>
                <%= this.MultiSelect("User.Projects").Options(Model.Projects, a=>a.Id, a=>a.Name).Selected(Model.User.Projects.Select(a=>a.Id)) %>
            </td>
            <td>
                <%= Html.ValidationMessageFor(a=>a.User.Projects) %>
            </td>
        </tr>
        <tr>
            <td>
                <label>Fund Types:</label>
            </td>
            <td>
                <%= this.MultiSelect("User.FundTypes").Options(Model.FundTypes, a=>a.Id, a=>a.Name).Selected(Model.User.FundTypes.Select(a=>a.Id)) %>
            </td>
            <td>
                <%= Html.ValidationMessage("User.FundTypes") %>
            </td>
        </tr>
    </table>
</fieldset>

<br />

<fieldset>
<legend>Roles</legend>
    <table>
        <tr>
            <td>
                <label>Roles:</label>
            </td>
            <td>
                <%= Html.CheckBoxListUnordered("RoleList").Options(Model.AvailableRoles).Selected(Model.UserRoles) %>
            </td>
            <td>
                <%= Html.ValidationMessage("RoleList") %>
            </td>
        </tr>
    </table>
</fieldset>

<div>
    Subordinates:
    
    <% if (Model.Subordinates.Count() == 0)
       { %>
        None
    <% }
       else
       { %>
    <ul>
    <% foreach (var sub in Model.Subordinates)
       { %>
        <li><%= Html.Encode(sub.FullName)%></li>
    <% } %>
    </ul>
    <% } %>
</div>