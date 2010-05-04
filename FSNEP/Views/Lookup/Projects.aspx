<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<FSNEP.Core.Domain.Project>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Projects
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<script type="text/javascript">
    $(function() {
        $("#CreateProjectForm").validate();
    });
</script>

<% Html.RenderPartial("LookupsHeader"); %>

    <h3><%= Html.Encode(TempData["Message"]) %></h3>
    <h2>Projects</h2>

    <%using (Html.BeginForm("CreateProject", "Lookup", FormMethod.Post, new {id="CreateProjectForm"})) { %>    
    <%= Html.AntiForgeryToken() %>
    
    <fieldset>
        <legend>New Project:</legend>
        <p>
            <%= this.TextBox("name").Label("Project Name: ").MaxLength(50).Class("required") %>
        </p>
        <p>
            <input type="submit" value="Add Project" />
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
                <form method="post" action="<%= Url.Action("InactivateProject", new { item.Id }) %>">
                    <%= Html.AntiForgeryToken() %>
                    <input type="submit" value="Inactivate" />
                </form>
            </td>
        </tr>
    
    <% } %>

    </table>

</asp:Content>

