<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<FSNEP.Core.Domain.TimeRecord>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	History
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>History</h2>

    <table>
        <tr>
            <th></th>
            <th>
                Month
            </th>
            <th>
                Year
            </th>
            <th>
                Status
            </th>
            <th>
                Print
            </th>
        </tr>

    <% foreach (var item in Model) { %>
    
        <tr>
            <td>
                <%= Html.ActionLink("Select", "Entry", new { id = item.Id} ) %>
            </td>
            <td>
                <%= Html.Encode(item.MonthName) %>
            </td>
            <td>
                <%= Html.Encode(item.Year) %>
            </td>
            <td>
                <%= Html.Encode(item.Status.Name) %>
            </td>
            <td>
                PRINT PDF (TODO)
            </td>
        </tr>
    
    <% } %>

    </table>

</asp:Content>