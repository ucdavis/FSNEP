<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<FSNEP.Core.Domain.CostShareEntry>>" %>

<%
    Html.Grid(Model)
        .DisplayAlternateMessageWhen(Model.Count() == 0, "No Local Support Entries Found")
        .Name("CostShareEntries")
        .Columns(col =>
                     {
                         col.Add(x => x.Amount);
                         col.Add(x => x.ExpenseType.Name).Title("Expense Type");
                         col.Add(x =>
                                     {%>
                                <%=Html.ActionLink<FileController>(a => a.ViewEntryFile(x.Id),
                                                                                           "View File")%>
                                <%
                                     }).Title("Receipt");
                         col.Add(x => x.Project.Name).Title("Project");
                         col.Add(x => x.FundType.Name).Title("Fund Type");
                         col.Add(x => x.Account.Name).Title("Account");
                         col.Add(x => x.Description);
                         col.Add(x => x.Comment);
                     })
        .Render();
%>

<p class="record-totals">
    Total Amount: $<%= Html.Encode(Model.Sum(x=>x.Amount)) %>
</p>