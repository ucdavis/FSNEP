<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<FSNEP.Core.Domain.User>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	ListUsers
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript">
        $(function() {
            $("#dlUserJump").change(function() {
                //Jump to the user chosen
                var userId = $(this).val();

                window.location = '<%= Url.Action("ModifyById") %>/' + userId;
            });
        });
    </script>

    <h2>ListUsers</h2>

    <h3><%= Html.Encode(TempData["Message"]) %></h3>

    <p>
        Jump to User: 
            <%= this.Select("dlUserJump").Options(Model, a=>a.Id, a=>a.FullNameLastFirst).FirstOption("--Search--") %>
    </p>
    
    <%
        Html.Grid(Model)
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
                             col.Add(x => x.Token);
                         })
            .Pageable(x => x.PageSize(20))
            .Sortable()
            .Render();
           
            %>

    <p>
        <%= Html.ActionLink("Create New", "Create") %>
    </p>

</asp:Content>

