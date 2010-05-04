<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<TimeRecordEntryViewModel>" %>

<script type="text/javascript">
    var Services = {
        AddEntry: '<%= Url.Action("AddEntry", "TimeRecord", new {recordId=Model.TimeRecord.Id}) %>',
        EditEntry: '<%= Url.Action("EditEntry", "TimeRecord") %>',
        RemoveEntry: '<%= Url.Action("RemoveEntry", "TimeRecord") %>',
        GetEntry: '<%= Url.Action("GetEntry", "TimeRecord") %>',
        GetAccountsForProject: '<%= Url.Action("GetAccountsForProject", "Association") %>'
    }

    var __RequestVerificationToken = null;

    $(function() {
        __RequestVerificationToken = $("input:hidden[name=__RequestVerificationToken]").val();
    });
    
</script>

<%= Html.AntiForgeryToken() %>
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

<div class="TimeRecordOptionsBox">
    <div class="TimeRecordTotals">
        TimeRecord Totals: <span id="timeSheetHours"><%= Html.Encode(Model.TotalHours) %></span> Hour(s)
    </div>
    <div class="TimeRecordTarget">
        Target Hours For Month: <%= Html.Encode(Model.TimeRecord.TargetHours) %> Hour(s)
    </div>
    <div class="TimeRecordAdjustment">
        <a href="javascript:;" id="addAdjustment">Add Adjustment [TODO]</a>
        
        <ul id="AdjustmentEntries">
            <% foreach (var adjustmentEntry in Model.AdjustmentEntries) { %>
            <li id="AdjustmentEntry<%= adjustmentEntry.Id %>">
                <%= Html.Encode(
                        string.Format("{0} HRS, {1}, {2}, {3}, {4}", 
                            adjustmentEntry.Hours, 
                            adjustmentEntry.Project.Name, 
                            adjustmentEntry.FundType.Name, 
                            adjustmentEntry.ActivityType.Name, 
                            adjustmentEntry.Account.Name)) 
                %>
                <span class="deleteAdjustmentEntry">
                    <a href="javascript:;" id='deleteAdjustmentEntry<%= adjustmentEntry.Id %>' class="DeleteAdjustmentEntry">X</a>
                </span>
            </li>
            <% } %>
        </ul>
    </div>
</div>

<div id="dialogTimeRecordEntry" class="TimeRecordEntryDialog" title="Add Entry" style="display:none">
    <%= Html.ClientSideValidation<FSNEP.Core.Domain.TimeRecordEntry>() %>
    <%= Html.ClientSideValidation<FSNEP.Core.Domain.Entry>() %>
    <form id="formAddEntry" method="post" action="<%= Url.Action("AddEntry", "TimeRecord") %>">
    <input id="addRecordDay" type="hidden" />
    <fieldset>
        <legend>Entry Information</legend>
        <p>
            <%= this.TextBox("Hours").Class("required").Label("Hours:") %>
        </p>
        <p>
            <label for="activityType">Activity Type:</label>
            <select id="ActivityType" name="ActivityType" class="required">
                <% Html.RenderPartial("ActivityTypeSelectOptions", Model.ActivityCategories); %>
            </select>
        </p>
        <p>
            <%= this.Select("FundType").Options(Model.FundTypes, f=>f.Id, f=>f.Name).Label("Fund Type:").Class("required").Disabled(Model.FundTypes.Count() == 1) %>
        </p>
        <p>
            <%= this.Select("Project").Options(Model.Projects, p=>p.Id, p=>p.Name).Label("Project:").Class("required").Disabled(Model.Projects.Count() == 1) %>
        </p>
        <p>
            <label for="Account" id="Account_Label">Account:</label>
            <select class="required" id="Account" name="Account">
                <option value="">Project Has No Accounts</option>
            </select>
        </p>
        <p>
            <%= this.TextArea("Comment").Columns(25).Label("Comments:")%>
        </p>
        <p>
            <input type="submit" value="Save!" />
        </p>
        
    </fieldset>
    </form>
</div>

<div id="dialogTimeRecordAdjustment" class="TimeRecordAdjustmentDialog" title="Adjust Entry" style="display:block">
    <%= Html.ClientSideValidation<FSNEP.Core.Domain.TimeRecordEntry>("Adjust")
            .AddRule("AdjustmentDate", new xVal.Rules.DataTypeRule(xVal.Rules.DataTypeRule.DataType.Date))
    %>
    <%= Html.ClientSideValidation<FSNEP.Core.Domain.Entry>("Adjust") %>
    <form id="formAdjustEntry" method="post" action="<%= Url.Action("AddEntry", "TimeRecord") %>">
    <fieldset>
        <legend>Entry Information</legend>
        <p>
            <%= this.TextBox("Adjust_AdjustmentDate").Class("required").Label("Adjusted Date: ") %>
        </p>
        <p>
            <%= this.TextBox("Adjust_Hours").Class("required").Label("Hours:")%>
        </p>
        <p>
            <label for="Adjust_ActivityType">Activity Type:</label>
            <select id="Adjust_ActivityType" name="Adjust_ActivityType" class="required">
                <% Html.RenderPartial("ActivityTypeSelectOptions", Model.ActivityCategories); %>
            </select>
        </p>
        <p>
            <%= this.Select("Adjust_FundType").Options(Model.FundTypes, f => f.Id, f => f.Name).Label("Fund Type:").Class("required").Disabled(Model.FundTypes.Count() == 1)%>
        </p>
        <p>
            <%= this.Select("Adjust_Project").Options(Model.Projects, p => p.Id, p => p.Name).Label("Project:").Class("required").Disabled(Model.Projects.Count() == 1)%>
        </p>
        <p>
            <label for="Adjust_Account">Account:</label>
            <select class="required" id="Adjust_Account" name="Adjust.Account">
                <option value="">Project Has No Accounts</option>
            </select>
        </p>
        <p>
            <%= this.TextArea("Adjust_Comment").Columns(25).Class("required").Label("Comments:")%>
        </p>
        <p>
            <input type="submit" value="Save!" />
        </p>
        
    </fieldset>
    </form>
</div>

<div id="dialogTimeRecordEdit" class="TimeRecordEditDialog" title="Edit Entry" style="display:none">
    <span style="display: none;" id="spanLoadingEntryDetails">
        <br/>
        <img alt="" src="../Images/mozilla_blu.gif"/> 
        Loading ...
    </span>
    <div id="divEntryDetails">
        <%= Html.ClientSideValidation<FSNEP.Core.Domain.TimeRecordEntry>("Edit") %>
        <%= Html.ClientSideValidation<FSNEP.Core.Domain.Entry>("Edit") %>
        <form id="formEditEntry" method="post" action="<%= Url.Action("EditEntry", "TimeRecord") %>">
        <input id="Edit_EntryId" type="hidden" />
        <fieldset>
            <legend>Entry Information</legend>
            <p>
                <label for="Edit_ActivityType">Activity Type:</label>
                <select id="Edit_ActivityType" name="Edit.ActivityType" class="required">
                    <% Html.RenderPartial("ActivityTypeSelectOptions", Model.ActivityCategories); %>
                </select>
            </p>
            <p>
                <label for="Edit_FundType_Readonly">Fund Type:</label>
                <span id="Edit_FundType_Readonly"></span>
            </p>
            <p>
                <label for="Edit_Project">Project:</label>
                <span id="Edit_Project_Readonly"></span>
            </p>
            <p>
                <label for="Edit_Account">Account:</label>
                <span id="Edit_Account_Readonly"></span>
            </p>
            <p>
                <%= this.TextBox("Edit_Hours").Class("required").Label("Hours")  %>
            </p>
            <p>
                <label for="Edit_Comment"></label>
                <%= this.TextArea("Edit_Comment").Columns(25).Label("Comments:")%>
            </p>
            <p>
                <input type="submit" value="Update!" />
            </p>
            
        </fieldset>
        </form>
    </div>
</div>