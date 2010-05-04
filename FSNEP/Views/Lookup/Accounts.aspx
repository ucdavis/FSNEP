<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<FSNEP.Core.Domain.Account>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Accounts
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<script type="text/javascript">
    $(function() {
        $("#CreateAccountForm").validate({
            rules: {
                indirectCostPercent: { range: [0, 30] }
            }
        });
    });
</script>

<% Html.RenderPartial("LookupsHeader"); %>

    <h3><%= Html.Encode(TempData["Message"]) %></h3>
    <h2>Accounts</h2>

    <%using (Html.BeginForm("CreateAccount", "Lookup", FormMethod.Post, new { id = "CreateAccountForm" }))
      { %>
    
    <fieldset>
        <legend>New Project:</legend>
        <p>
            <%= this.TextBox("name").Label("Account Name: ").MaxLength(50).Class("required") %>
        </p>
        <p>
            <%= this.TextBox("indirectCostPercent").Label("Indirect Cost %: ").MaxLength(2).Class("required digits") %>
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
                <%= Html.Encode(item) %>
            </td>
            <td>
                <% using (Html.BeginForm("InactivateAccount", "Lookup", new { item.ID })) { %>
                
                    <input type="submit" value="Inactivate" />
                
                <%} %>
            </td>
        </tr>
    
    <% } %>

    </table>

</asp:Content>

