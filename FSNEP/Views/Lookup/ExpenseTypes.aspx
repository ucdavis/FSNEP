<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<FSNEP.Core.Domain.ExpenseType>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Expense Types
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<script type="text/javascript">
    $(function() {
        $("#CreateExpenseForm").validate();
    });
</script>

<% Html.RenderPartial("LookupsHeader"); %>

    <h3><%= Html.Encode(TempData["Message"]) %></h3>
    <h2>Expense Types</h2>

    <%using (Html.BeginForm("CreateExpenseType", "Lookup", FormMethod.Post, new { id = "CreateExpenseForm" }))
      { %>
    
    <fieldset>
        <legend>New Expense Type:</legend>
        <p>
            <%= this.TextBox("name").Label("Expense Type Name: ").MaxLength(50).Class("required") %>
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
                <%= Html.Encode(item) %>
            </td>
            <td>
                <% using (Html.BeginForm("InactivateExpenseType", "Lookup", new { item.ID })) { %>
                
                    <input type="submit" value="Inactivate" />
                
                <%} %>
            </td>
        </tr>
    
    <% } %>

    </table>

</asp:Content>

