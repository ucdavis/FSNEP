<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<User>>" %>
<%@ Import Namespace="FSNEP.Core.Domain"%>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	TimeRecord
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript">
        $(function() {
            $('form').validate();

            $('#recordId').cascade('#userId', {
                ajax: { url: '/Report/GetRecordForUser/' },
                template: function(item) {
                    return "<option value='" + item.value + "'>" + item.text + "</option>";
                }
                //,match: function(selectedValue) { return this.when == selectedValue; }
            });
        });
    </script>

    <h2>TimeRecord</h2>
    
    <% using (Html.BeginForm()) { %>
        <%= Html.AntiForgeryToken() %>
        
    <fieldset>
        <legend>Select A Time Record</legend>
        <p>
            <%= this.Select("userId")
                .Options(Model, x=>x.Id, x=>x.FullNameLastFirst)
                .FirstOption("Select A User")
                .Class("required")
                .Label("User: ")
            %>
        </p>
        <p>
            <label for="recordId">Record: </label>
            <select id="recordId" name="recordId" class="required">
            </select>
        </p>
        <p>
            <input type="submit" value="Generate!" />
        </p>
    </fieldset>
    
    <% } %>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="AdditionalScripts" runat="server">
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/jquery.cascade.js") %>"></script>
</asp:Content>
