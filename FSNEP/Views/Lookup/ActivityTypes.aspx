<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<FSNEP.Core.Domain.ActivityType>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Activity Types
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h3><%= Html.Encode(TempData["Message"]) %></h3>
    <h2>Activity Types</h2>

    <%using (Html.BeginForm("CreateActivityType", "Lookup"))
      { %>
    
    <fieldset>
        <legend>New Activity Type:</legend>
        <p>
            <%= this.TextBox("name").Label("Activity Type Name: ")%>
        </p>
        <p>
            <%= this.TextBox("indicator").MaxLength(2).Label("Indicator: ") %>
        </p>
        <p>
            <label id="activityCategoryId_Label" for="activityCategoryId">Category: </label>
            <%= Html.DropDownList("activityCategoryId", new SelectList((IEnumerable)ViewData["ActivityCategories"], "ID", "Name"))%>
            <%= Html.ActionLink<FSNEP.Controllers.LookupController>(a=>a.ActivityCategories(), "Create New Activity Category") %>
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
                <%= Html.Encode(string.Format("{0} ({1}) -- {2}", item.Name, item.Indicator, item.ActivityCategory.Name)) %>
            </td>
            <td>
                <% using (Html.BeginForm<FSNEP.Controllers.LookupController>(a => a.InactivateActivityType(item.ID)))
                   { %>
                
                    <input type="submit" value="Inactivate" />
                
                <%} %>
            </td>
        </tr>
    
    <% } %>

    </table>

</asp:Content>

