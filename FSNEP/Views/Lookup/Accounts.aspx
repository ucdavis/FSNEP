<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<FSNEP.Core.Domain.Account>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Accounts
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h3><%= Html.Encode(TempData["Message"]) %></h3>
    <h2>Accounts</h2>

    <%using (Html.BeginForm("CreateAccount", "Lookup")) { %>
    
    <fieldset>
        <legend>New Project:</legend>
        <p>
            <%= this.TextBox("name").Label("Account Name: ")%>
        </p>
        <p>
            <%= this.TextBox("indirectCostPercent").Label("Indirect Cost %: ") %>
        </p>
        <p>
            <input type="submit" value="Add Account" />
        </p>
    </fieldset>
    
    <% } %>

    <br />

    <table>
        <tr>
            <th>
                Name
            </th>
            <th></th>
        </tr>

    <% foreach (var item in Model) { %>
    
        <tr>
            <td>
                <%= Html.Encode(string.Format("{0} ({1}%)", item.Name, item.IndirectCostPercent)) %>
            </td>
            <td>
                <% using (Html.BeginForm<FSNEP.Controllers.LookupController>(a => a.InactivateAccount(item.ID)))
                   { %>
                
                    <input type="submit" value="Inactivate" />
                
                <%} %>
            </td>
        </tr>
    
    <% } %>

    </table>

</asp:Content>

