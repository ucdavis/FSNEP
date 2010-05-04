<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<CostShareAuditHistoryViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	History
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript">
        $(function() {
            $("#project").change(function() {

                var listUrlBase = '<%= Url.Action("History") %>';

                var projectId = $(this).val();

                if (projectId == "") window.location = listUrlBase;
                else window.location = listUrlBase + '/?projectId=' + projectId;
            });
        });
    </script>

    <h2>History</h2>

    <%= this.Select("project")
            .Options(Model.Projects, x=>x.Id, x=>x.Name)
            .Selected(Model.Project != null ? Model.Project.Id : 0)
            .FirstOption("--Select A Project--")
        %>

</asp:Content>