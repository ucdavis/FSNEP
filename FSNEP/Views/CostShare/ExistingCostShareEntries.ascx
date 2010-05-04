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
                        col.Add(x => x.ExpenseType.Name);
                        col.Add(x => x.Project.Name);
                        col.Add(x => x.FundType.Name);
                        col.Add(x => x.Account.Name);
                        col.Add(x => x.Description);
                        col.Add(x => x.Comment);
                    })
        .Render();
    
%>