<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<FSNEP.Core.Domain.TimeRecord>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Time Record History
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Time Record History</h2>
    
    <%
        Html.Grid(Model)
            .Transactional()
            .Name("TimeRecords")
            .PrefixUrlParameters(false)
            .Columns(col =>
                         {
                             col.Add(x =>
                                         {
                                            %>
                                             <%=Html.ActionLink("Select", "Entry", new {id = x.Id})%>
                                             <%
                                         });
                             col.Add(x => x.MonthName).Title("Month");
                             col.Add(x => x.Year);
                             col.Add(x => x.Status.Name);
                             col.Add(x =>
                                         {
                                    %>
                                    <%= Html.ActionLink<ReportController>(a=>a.PrintOwnTimeRecord(x.Id), "Print PDF") %>
                                 <%
                                         });
                         })
            .Render();
           
            %>

</asp:Content>