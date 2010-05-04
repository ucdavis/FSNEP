<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<FSNEP.Core.Domain.ExpenseType>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Expense Types
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h3><%= Html.Encode(TempData["Message"]) %></h3>
    <h2>Expense Types</h2>

    <%using (Html.BeginForm("CreateExpenseType", "Lookup"))
      { %>
    
    <fieldset>
        <legend>New Expense Type:</legend>
        <p>
            <%= this.TextBox("name").Label("Expense Type Name: ")%>
        </p>
        <p>
            <input type="submit" value="Add Expense Type" />
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
                <%= Html.Encode(item.Name) %>
            </td>
            <td>
                <% using (Html.BeginForm<FSNEP.Controllers.LookupController>(a => a.InactivateExpenseType(item.ID)))
                   { %>
                
                    <input type="submit" value="Inactivate" />
                
                <%} %>
            </td>
        </tr>
    
    <% } %>

    </table>

</asp:Content>

