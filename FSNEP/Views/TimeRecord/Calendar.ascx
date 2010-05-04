<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<FSNEP.Controllers.TimeRecordEntryViewModel>" %>

<div style="width: 847px; margin: 0 auto;">
    <img src="../Images/cal_sunday.gif" alt="sunday" style="margin-left: 1px;" /><img
        src="../Images/cal_monday.gif" alt="monday" style="margin-left: 1px;" /><img src="../Images/cal_tuesday.gif"
            alt="tuesday" style="margin-left: 1px;" /><img src="../Images/cal_wednesday.gif"
                alt="wednesday" style="margin-left: 1px;" /><img src="../Images/cal_thursday.gif"
                    alt="thursday" style="margin-left: 1px;" /><img src="../Images/cal_friday.gif" alt="friday"
                        style="margin-left: 1px;" /><img src="../Images/cal_saturday.gif" alt="saturday"
                            style="margin-left: 1px;" /></div>

<div class="calendarDiv">
    <% foreach (var day in Model.CalendarDays)
       {%>
    <%  string dayId = day.IsActive ? day.Day.ToString() : string.Empty;
        string className = day.IsActive ? "ActiveDay" : "InactiveDay";
    %>
    <div id="day<%= dayId %>" class="calendarStyle <%= className %>">
        <div class="calendarDate <%= day.IsCurrent ? "CurrentDay" : string.Empty %>">
            <%= Html.Encode(day.Day) %>
        </div>
        
        <% if (day.IsActive)
           { %>
           
        <a href="javascript:;" id="addEntry<%= dayId %>" class="AddCalendarEntry">
            Add
        </a>
        <% } %>
        <table>
            <tbody id='entriesFor<%= dayId %>'>
                <%
                    foreach (var entry in day.Entries)
                    {%>
                <tr id='Entry<%= entry.Id %>'>
                    <td>
                        <span class="editEntry">
                            <a id='editEntry<%= entry.Id %>' href="javascript:;" class="EditCalendarEntry">
                                <%= Html.Encode(string.Format("{0} HRS", entry.Hours)) %>
                            </a>
                        </span>
                        <span class="deleteEntry">
                            <a href="javascript:;" id='deleteEntry<%= entry.Id %>' class="DeleteCalendarEntry">X</a>
                        </span>
                        <input type="hidden" id="entryHours<%= entry.Id %>" value="<%= entry.Hours %>" />
                    </td>
                </tr>
                <%
                    } %>
            </tbody>
        </table>
    </div>
    <% } %>
</div>
