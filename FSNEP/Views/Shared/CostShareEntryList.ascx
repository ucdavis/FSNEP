<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<FSNEP.Core.Domain.CostShareEntry>>" %>

<%
    Html.Grid(Model)
        .DisplayAlternateMessageWhen(Model.Count() == 0, "No Cost Share Entries Found")
        .Name("CostShareEntries")
        .Columns(col =>
                     {
                         col.Add(x => x.Amount);
                         col.Add(x => x.ExpenseType.Name).Title("Expense Type");
                         col.Add(x =>
                                     {%>

                                        asdsad

                                        <%
                                     }).Title("Receipt");
                         col.Add(x => x.Project.Name).Title("Project");
                         col.Add(x => x.FundType.Name).Title("Fund Type");
                         col.Add(x => x.Account.Name).Title("Account");
                         col.Add(x => x.Description);
                         col.Add(x => x.Comment);
                     })
        .Render;
        %>
