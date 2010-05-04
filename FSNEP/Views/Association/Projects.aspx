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
    
    <%= this.Select("Projects").FirstOption("", "--Select A Project--").Options(Model.Projects, p=>p.ID, p=>p.Name).Selected(Model.Project != null ? Model.Project.ID : 0) %>

    <% if (Model.Project != null) { %>
    
    <% using(Html.BeginForm("Associate", "Association", new {id = Model.Project.ID})) {%>
    <br /><br />
    <fieldset>
        <legend>Accounts for <%=Html.Encode(Model.Project.Name)%></legend>
        <p>
            <%=
                   this.MultiSelect("AccountIds").Options(Model.Accounts, "ID", "Name").Selected(
                       Model.Project.Accounts.Select(a => a.ID))%>
        </p>
        <p>
            <input type="submit" id="submitAssociation" value="Associate" />
        </p>
    </fieldset>
    
    <% }
     } %>
</asp:Content>
