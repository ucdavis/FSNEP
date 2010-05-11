<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<FSNEP.Core.Domain.TimeRecordEntry>>" %>

<%= Html.Grid(Model.OrderBy(x=>x.Date))
            .DisplayAlternateMessageWhen(Model.Count() == 0, "No Time Record Entries Found")
            .Name("TimeRecordEntries")
            .Columns(col=>
                         {
                             col.Add(x => x.Date);
                             col.Add(x => x.Hours);
                             col.Add(x => x.ActivityType.Name).Title("Activity");
                             col.Add(x => x.Project.Name).Title("Project");
                             col.Add(x => x.FundType.Name).Title("Fund Type");
                             col.Add(x => x.Account.Name).Title("Account");
                             col.Add(x => x.Comment);
                         })
        %>
        
<p class="record-totals">
    Time Record Total Hours: <%= Html.Encode(Model.Sum(x=>x.Hours)) %>
</p>