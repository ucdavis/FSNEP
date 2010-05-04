<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CostShareReviewViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Review
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2><%= Html.Encode(string.Format("Cost Share for {0:MMMM yyyy}", Model.CostShare.Date)) %></h2>
    
    <%= Html.Grid(Model.CostShareEntries)
            .DisplayAlternateMessageWhen(Model.CostShareEntries.Count() == 0, "No Cost Share Entries Found")
            .Transactional()
            .Name("CostShareEntries")
            .Columns(col=>
                         {
                             col.Add(x => x.Amount);
                             col.Add(x => x.ExpenseType.Name).Title("Expense Type");
                             col.Add(x => x.Project.Name).Title("Project");
                             col.Add(x => x.FundType.Name).Title("Fund Type");
                             col.Add(x => x.Account.Name).Title("Account");
                             col.Add(x => x.Description);
                             col.Add(x => x.Comment);
                         })
        %>
        
    <br />
    <div>
        <%= Html.ActionLink<CostShareController>(x=>x.History(), "Back to Cost Share History") %>
    </div>    

</asp:Content>