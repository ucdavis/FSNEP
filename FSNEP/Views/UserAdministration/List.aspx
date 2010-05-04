<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<UserListViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	User Admin: User List
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript">
        $(function() {
            $("#dlUserJump").change(function() {
                //Jump to the user chosen
                var userId = $(this).val();

                window.location = '<%= Url.Action("ModifyById") %>/' + userId;
            });

            $("#project").change(function() {

                var listUrlBase = '<%= Url.Action("List") %>';

                var projectId = $(this).val();

                if (projectId == "") window.location = listUrlBase;
                else window.location = listUrlBase + '/?projectId=' + projectId;
            });
        });
    </script>

    <h2>User Admin: User List</h2>

    <h3><%= Html.Encode(TempData["Message"]) %></h3>
    
    <p>
        <%= Html.ActionLink("Create New", "Create") %>
    </p>

    <p>
            <%= this.Select("dlUserJump").Options(Model.Users, a=>a.Id, a=>a.FullNameLastFirst).FirstOption("--Search--").Label("Jump to User: ") %>
    </p>
    
    <p>
            <%= this.Select("project").Options(Model.Projects, x=>x.Id, x=>x.Name).Selected(Model.SelectedFilterProjectId ?? 0).FirstOption("--All Projects--").Label("Filter by Project: ") %>
    </p>
    
    <%
        Html.Grid(Model.Users)
            .Name("Users")
            .PrefixUrlParameters(false)
            .Columns(col =>
                         {
                             col.Add(u =>
                                         {%>
                                            <%=Html.ActionLink("Edit", "Modify", new {id = u.UserName})%>
                                            <%
                                         });
                             col.Add(x => x.UserName);
                             col.Add(x => x.FirstName);
                             col.Add(x => x.LastName);
                             col.Add(x => x.Salary);
                             col.Add(x => x.BenefitRate);
                             col.Add(x => x.FTE);
                             //col.Add(x => x.Token);
                         })
            .Pageable(x => x.PageSize(20))
            .Sortable()
            .Render();
           
            %>

    

</asp:Content>

