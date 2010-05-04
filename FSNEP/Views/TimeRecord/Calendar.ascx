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
            <div class="calendarDate">
                <%= Html.Encode(day.Day) %>
            </div>
            Add 
            <table>
                <tbody id='<%= dayId %>'>
                    <%
                        foreach (var entry in day.Entries)
                        {%>
                    <tr id='Entry<%= entry.Id %>'>
                        <td>
                            <span class="editEntry">
                                <%= Html.Encode(string.Format("{0} HRS", entry.Hours)) %>
                            </span>
                        </td>
                    </tr>
                    <%
        
                        } %>
                </tbody>
            </table>
        </div>
        <% } %>
    </div>
