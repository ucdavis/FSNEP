<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<User>>" %>
<%@ Import Namespace="FSNEP.Core.Domain"%>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	Time Record Report
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript">
        $(function() {
            $('#recordId').cascade('#userId', {
                ajax: { url: '<%= Url.Action("GetRecordForUser", "Report") %>' },
                template: function(item) {
                    return "<option value='" + item.value + "'>" + item.text + "</option>";
                }
                //,match: function(selectedValue) { return this.when == selectedValue; }
            });

            $('#generate').click(function(e) {
                //First make sure that a recordId was chosen
                var recordSelect = $('#recordId');
                var recordId = recordSelect.val();

                if ($('form').valid()) {
                    //Ok, no errors so open a new time record report in a new window
                    window.open('<%= Url.Action("DisplayViewableTimeRecord", "Report") %>/' + recordId, "_blank");
                }

                e.preventDefault();
            });
        });
    </script>

    <h2>TimeRecord</h2>
    
    <% using (Html.BeginForm()) { %>
    
    <fieldset>
        <legend>Select A Time Record</legend>
        <p>
            <%= this.Select("userId")
                .Options(Model, x => x.Id, x => x.FullNameLastFirst)
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
            <a href="#" id="generate">Generate!</a>
        </p>
    </fieldset>

    <% } %>
    
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="AdditionalScripts" runat="server">
    <script type="text/javascript" src="<%= Url.Content("~/Scripts/jquery.cascade.js") %>"></script>
</asp:Content>
