<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IList<FSNEP.Core.Domain.ActivityCategory>>" %>
<select id="ActivityType" name="ActivityType" class="required">
    <option selected="selected" value="">Select an Activity Type</option>
    <%
        foreach (var category in Model)
        {%>
    <optgroup label="<%= Html.Encode(category.Name) %>">
        <%
            foreach (var activityType in category.ActivityTypes)
            {%>
        <option value="<%= activityType.Id %>">
            <%= Html.Encode(activityType.Name) %></option>
        <% 
            } %>
    </optgroup>
    <%  
        } %>
</select>
