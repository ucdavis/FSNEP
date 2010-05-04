<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<AuditHistoryViewModel<CostShare>>" %>
<%@ Import Namespace="FSNEP.Core.Domain"%>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Cost Share Audit
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript">
        $(function() {
            $("#project").change(function() {

                var listUrlBase = '<%= Url.Action("CostShareHistory") %>';

                var projectId = $(this).val();

                if (projectId == "") window.location = listUrlBase;
                else window.location = listUrlBase + '/?projectId=' + projectId;
            });
        });
    </script>

    <h2>Cost Share Audit</h2>

    Project: <%= this.Select("project")
            .Options(Model.Projects, x=>x.Id, x=>x.Name)
            .Selected(Model.Project != null ? Model.Project.Id : 0)
            .FirstOption("--Select A Project--")
        %>

    <% if (Model.Project != null)
       { %>

        <br /><br />
        <div>

        <%
        Html.Grid(Model.Records)
            .DisplayAlternateMessageWhen(Model.Records.Count() == 0, "No Cost Share Records Found")
            .Name("CostShares")
            .PrefixUrlParameters(false)
            .Columns(col =>
                         {
                             col.Add(cs =>
                                         {%>
                                            <%= Html.ActionLink<AuditController>(x=>x.CostShareReview(cs.Id), "Review") %>
                                            <%
                                         });
                             col.Add(x => x.User.FullName).Title("User");
                             col.Add(x => x.MonthName).Title("Month");
                             col.Add(x => x.Year);
                             col.Add(x => x.Status.Name).Title("Status");
                         })
            .Render();
        
            %>

        </div>

    <% } %>
</asp:Content>