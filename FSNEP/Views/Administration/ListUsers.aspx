<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<FSNEP.Core.Domain.User>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	ListUsers
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>ListUsers</h2>

    <p>
        Jump to User: <%= Html.DropDownList("dlUserJump", new SelectList(Model, "ID", "FullNameLastFirst", "--Search--"))%> [TODO]
    </p>
    
    <table>
        <tr>
            <th></th>
            <th>
                FirstName
            </th>
            <th>
                LastName
            </th>
            <th>
                Salary
            </th>
            <th>
                BenefitRate
            </th>
            <th>
                FTE
            </th>
            <th>
                ResetPassword
            </th>
            <th>
                IsActive
            </th>
            <th>
                FullName
            </th>
            <th>
                FullNameLastFirst
            </th>
            <th>
                Token
            </th>
            <th>
                ID
            </th>
        </tr>

    <% foreach (var item in Model) { %>
    
        <tr>
            <td>
                <%= Html.ActionLink("Edit", "ModifyUserById", new {id = item.ID}) %>
            </td>
            <td>
                <%= Html.Encode(item.FirstName) %>
            </td>
            <td>
                <%= Html.Encode(item.LastName) %>
            </td>
            <td>
                <%= Html.Encode(String.Format("{0:F}", item.Salary)) %>
            </td>
            <td>
                <%= Html.Encode(String.Format("{0:F}", item.BenefitRate)) %>
            </td>
            <td>
                <%= Html.Encode(String.Format("{0:F}", item.FTE)) %>
            </td>
            <td>
                <%= Html.Encode(item.ResetPassword) %>
            </td>
            <td>
                <%= Html.Encode(item.IsActive) %>
            </td>
            <td>
                <%= Html.Encode(item.FullName) %>
            </td>
            <td>
                <%= Html.Encode(item.FullNameLastFirst) %>
            </td>
            <td>
                <%= Html.Encode(item.Token) %>
            </td>
            <td>
                <%= Html.Encode(item.ID) %>
            </td>
        </tr>
    
    <% } %>

    </table>

    <p>
        <%= Html.ActionLink("Create New", "CreateUser") %>
    </p>

</asp:Content>

