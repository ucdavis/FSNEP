<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<FSNEP.Core.Domain.ActivityCategory>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Activity Categories
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<script type="text/javascript">
    $(function() {
        $("#CreateActivityCategoryForm").validate();
    });
</script>

<% Html.RenderPartial("LookupsHeader"); %>

    <h3><%= Html.Encode(TempData["Message"]) %></h3>
    <h2>Activity Categories</h2>

    <%using (Html.BeginForm("CreateActivityCategory", "Lookup", new { returnTo = Request.QueryString["returnTo"] }, FormMethod.Post, new { id = "CreateActivityCategoryForm" }))
      { %>
    
    <fieldset>
        <legend>New Activity Category:</legend>
        <p>
            <%= this.TextBox("name").Label("Activity Category Name: ").MaxLength(50).Class("required") %>
        </p>
        <p>
            <input type="submit" value="Add Activity Category" />
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
                <% using (Html.BeginForm("InactivateActivityCategory", "Lookup", new { item.Id })) { %>
                
                    <input type="submit" value="Inactivate" />
                
                <%} %>
            </td>
        </tr>
    
    <% } %>

    </table>

</asp:Content>

