<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<FSNEP.Core.Domain.CostShare>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	History
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>History</h2>

    <%
        Html.Grid(Model)
            .Transactional()
            .Name("CostShares")
            .PrefixUrlParameters(false)
            .Columns(col =>
                         {
                             col.Add(cs =>
                                         {%>
                                            <%=Html.ActionLink("Select", "Entry", new {id = cs.Id})%>
                                            <%
                                         });
                             col.Add(x => x.MonthName).Title("Month");
                             col.Add(x => x.Year);
                             col.Add(x => x.Status.Name);
                         })
            .Render();
        
            %>
    
    <p>
        <%= Html.ActionLink("Create New", "Create") %>
    </p>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="AdditionalScripts" runat="server">
</asp:Content>

