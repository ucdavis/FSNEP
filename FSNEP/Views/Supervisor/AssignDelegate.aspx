<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<FSNEP.Core.Domain.User>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Assign Delegate
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript">
        $(function() {
            $('form').validate();
        });
    </script>

    <h2>Assign Delegate</h2>

    <p>You can assign a supervisor delegate from the list below</p>
    
    <% using(Html.BeginForm()) { %>
        <%= Html.AntiForgeryToken() %>
        
    <p>
        <%= this.Select("UserId")
                .Class("required")
                .Options(Model, x=>x.Id, x=>x.FullNameLastFirst)
                .FirstOption("--Select A Delegate--")
                .Label("Delegates: ")
            %>
    </p>
    <p>
        <input type="submit" value="Assign Delegate" />
    </p>
    
    <% } %>

</asp:Content>
