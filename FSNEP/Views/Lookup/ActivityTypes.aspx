<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<FSNEP.Core.Domain.ActivityType>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Activity Types
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<script type="text/javascript">
    $(function() {
        $("#CreateActivityTypeForm").validate({
            rules: {
                indicator: { minlength: 2 }
            }
        });
    });
</script>

<% Html.RenderPartial("LookupsHeader"); %>

    <h3><%= Html.Encode(TempData["Message"]) %></h3>
    <h2>Activity Types</h2>

    <%using (Html.BeginForm("CreateActivityType", "Lookup", FormMethod.Post, new { id = "CreateActivityTypeForm" }))
      { %>
    
    <fieldset>
        <legend>New Activity Type:</legend>
        <p>
            <%= this.TextBox("name").Label("Activity Type Name: ").MaxLength(50).Class("required") %>
        </p>
        <p>
            <%= this.TextBox("indicator").Label("Indicator: ").MaxLength(2).Class("required") %>
        </p>
        <p>
            <label id="activityCategoryId_Label" for="activityCategoryId">Category: </label>
            <%= Html.DropDownList("activityCategoryId", new SelectList((IEnumerable)ViewData["ActivityCategories"], "ID", "Name"), "Select A Category", new {@class="required"})%>
            <%= Html.ActionLink<FSNEP.Controllers.LookupController>(a=>a.ActivityCategories("ActivityType"), "Create New Activity Category") %>
        </p>
        <p>
            <input type="submit" value="Add Activity Type" />
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
                <% using (Html.BeginForm("InactivateActivityType", "Lookup", new { item.Id })){ %>
                
                    <input type="submit" value="Inactivate" />
                
                <% } %>
            </td>
        </tr>
    
    <% } %>

    </table>

</asp:Content>

