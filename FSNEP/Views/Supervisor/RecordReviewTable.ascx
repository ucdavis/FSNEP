<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<FSNEP.Core.Domain.Record>>" %>

<table class="t-widget t-grid">
    <thead>
        <tr>
            <th class="t-header"></th>
            <th class="t-header">Name</th>
            <th class="t-header t-last-header">Status</th>
        </tr>
    </thead>
    <tbody>
      <%--  <% string lastUserName = string.Empty; %>
        <% foreach (var record in Model) { %>
        <% string currentUserName = record.User.FullName; %>
        
        <% if (currentUserName != lastUserName) { %>
        
            <tr class="ui-widget-header">
                <td colspan="4"><h3><%= Html.Encode(currentUserName)%></h3></td>
            </tr>
        
        <% lastUserName = currentUserName; %>
        <% } %>
           <tr>
            <td><%= Html.ActionLink("Select", (string)ViewData["ReviewAction"], new {id = record.Id}) %></td>
            <td><%= Html.Encode(record.MonthName)%></td>
            <td><%= Html.Encode(record.Year)%></td>
            <td><%= Html.Encode(record.Status.Name)%></td>
           </tr>   --%>    
           
        <% int lastMonth = 0; %>
        <% foreach (var record in Model) { %>
            <% int currentMonth = record.Month; %>
            <% if (currentMonth != lastMonth) { %>
            
                <tr class="ui-widget-header">
                     <td colspan="4"><h3><%= Html.Encode(record.MonthName)%> <%= Html.Encode(record.Year)%> </h3></td>
                </tr>
            
            <% lastMonth = currentMonth; %>
            <% } %>
            
               <tr>
                <td><%= Html.ActionLink("Select", (string)ViewData["ReviewAction"], new {id = record.Id}) %></td>
                <td><%= Html.Encode(record.User.FullName)%></td>
                <td><%= Html.Encode(record.Status.Name)%></td>
              </tr>   
       
                        
        <% } %>
    </tbody>

</table>


