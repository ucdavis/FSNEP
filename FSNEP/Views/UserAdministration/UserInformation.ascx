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
            <td>Active User?</td>
            <td>
                <%= Html.CheckBox("User.IsActive") %>
            </td>
            <td></td>
        </tr>
        <tr>
            <td>
                Supervisor:
            </td>
            <td>
                <%= Html.DropDownListFor(u=>u.User.Supervisor, new SelectList(Model.Supervisors, "ID", "FullNameLastFirst"), "Select A Supervisor") %>
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
                <%= Html.ListBox("User.Projects", new MultiSelectList(Model.Projects, "ID", "Name", Model.User.Projects.Select(a=>a.ID))) %>
                <%--<%= this.MultiSelect("User.Projects").Options(new MultiSelectList(Model.Projects, "ID", "Name", Model.User.Projects.Select(a=>a.ID)))%>--%>
                <%--<%= this.MultiSelect("ProjectList").Options(Model.Projects) %>--%>
            </td>
            <td>
                <%= Html.ValidationMessage("User.Projects") %>
            </td>
        </tr>
        <tr>
            <td>
                Fund Types:
            </td>
            <td>
                <%
                    var fundtypes = Model.FundTypes;
                    var fundtypesids = new[] { 2 }; //Model.User.FundTypes.Select(a => a.ID);
                    int i = 0;
                    
                    
                     %>
            
                <%= this.MultiSelect("User.FundTypes").Options(new MultiSelectList(Model.FundTypes, "ID", "Name")).Selected(fundtypesids) %>
                <%--<%= this.MultiSelect("FundTypeList").Options(Model.FundTypes) %>--%>
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
                Roles:
            </td>
            <td>
                <%= this.CheckBoxList("RoleList").Options(Model.AvailableRoles).Selected(Model.UserRoles) %>
            </td>
            <td>
                <%= Html.ValidationMessage("RoleList") %>
            </td>
        </tr>
    </table>
</fieldset>