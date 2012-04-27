<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<FSNEP.Core.Domain.Project>>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Local Support Report
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript">
        $(function() {

            $("form").validate();

        });
    </script>

    <h2>Local Support Report</h2>

    <% using(Html.BeginForm()) { %>
        <%= Html.AntiForgeryToken() %> 
    <fieldset>
        <p>
            <%= this.Select("projectId")
                .Options(Model, x=>x.Id, x=>x.Name)
                .FirstOption("-- Select A Project --")
                .Class("required")
                .Label("Project: ")
            %>
        </p>    
        <p>
            <%= this.TextBox("year").Class("required digits").Label("Year*: ") %>
        </p>
        <p>
            <input type="submit" value="Generate!" />
        </p>
        <p>
            *Year Represents The Larger Year During A Fiscal Cycle (i.e. Oct 2008 � Sept 2009 would be 2009)  
        </p>
    </fieldset>
    
    <% } %>

</asp:Content>