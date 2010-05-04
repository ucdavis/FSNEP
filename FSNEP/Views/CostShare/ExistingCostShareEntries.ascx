<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<FSNEP.Core.Domain.CostShareEntry>>" %>

<% Html.Grid(Model)
       .DisplayAlternateMessageWhen(Model.Count() == 0, "No Records Found")
       .Name("CostShareEntries")
       .PrefixUrlParameters(false)
       .Columns(col =>
                    {
                        col.Add(entry =>
                                    {%>
                                        <form method="post" action="<%=Url.Action("RemoveEntry", new {entryId = entry.Id})%>">
                                            <%=Html.AntiForgeryToken()%>
                                            <input type="submit" value="Remove" />
                                        </form>
                                        <%
                                    });
                        col.Add(x => x.Amount);
                        col.Add(x => x.ExpenseType.Name).Title("Expense Type");
                        col.Add(x => x.Project.Name).Title("Project");
                        col.Add(x => x.FundType.Name).Title("Fund Type");
                        col.Add(x => x.Account.Name).Title("Account");
                        col.Add(x => x.Description);
                        col.Add(x => x.Comment);
                    })
        .Render();
    
%>