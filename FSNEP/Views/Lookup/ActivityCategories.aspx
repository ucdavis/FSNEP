<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<FSNEP.Core.Domain.ActivityCategory>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Activity Categories
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h3><%= Html.Encode(TempData["Message"]) %></h3>
    <h2>Activity Categories</h2>

    <%using (Html.BeginForm("CreateActivityCategory", "Lookup"))
      { %>
    
    <fieldset>
        <legend>New Activity Category:</legend>
        <p>
            <%= this.TextBox("name").Label("Activity Category Name: ")%>
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
                <%= Html.Encode(item.Name) %>
            </td>
            <td>
                <% using (Html.BeginForm<FSNEP.Controllers.LookupController>(a => a.InactivateActivityCategory(item.ID)))
                   { %>
                
                    <input type="submit" value="Inactivate" />
                
                <%} %>
            </td>
        </tr>
    
    <% } %>

    </table>

</asp:Content>

