<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<TimeRecordReviewViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Review
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

<h2><%= Html.Encode(string.Format("Time Record for {0:MMMM yyyy}", Model.TimeRecord.Date)) %></h2>
    
    <%= Html.Grid(Model.TimeRecordEntries)
            .DisplayAlternateMessageWhen(Model.TimeRecordEntries.Count() == 0, "No Time Record Entries Found")
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
        
    <br />
    <div>
        <%= Html.ActionLink<TimeRecordController>(x=>x.History(), "Back to Time Record History") %>
    </div>    

</asp:Content>
