<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<FSNEP.Controllers.ProjectsAccountsViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Projects
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript">
        $(document).ready(function() {
            $("#Projects").change(function() {
                var id = $(this).val();
                window.location = '<%= Url.Action("Projects", new {id=""}) %>/' + id;
            });
        });
    </script>

    <h2>Projects</h2>

    <%= this.Select("Projects").Options(Model.Projects, p=>p.ID, p=>p.Name) %>

    <%= Html.Encode(Model.Project == null) %>
    
</asp:Content>
