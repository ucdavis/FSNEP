<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ReviewViewModel<TimeRecord, TimeRecordEntry>>" %>
<%@ Import Namespace="FSNEP.Core.Domain"%>

<asp:Content ID="Content1" ContentPlaceHolderID="titleContent" runat="server">
	TimeRecordReview
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <script type="text/javascript">
        var approved;

        $(function() {
            approved = $("#Approved");

            $("#Approve").click(function() {
                approved.val(true);
            });

            $("#Disapprove").click(function() {
                if ($("#ReviewComment").val().length == 0) {
                    alert("Comment Required for Disapproval");
                    return false;
                }
                
                approved.val(false);
            });
        });
    </script>

    <h2><%= Html.Encode(string.Format("Reviewing {0} for {1:MMMM yyyy}", Model.Record.User.FullName, Model.Record.Date)) %></h2>

    <% Html.RenderPartial("TimeRecordEntryList", Model.Entries); %>

    <% if (Model.CanBeApprovedOrDenied) { %>
    <div>
        <% using (Html.BeginForm("ApproveOrDenyRecord", "Supervisor", new {id=Model.Record.Id}, FormMethod.Post, new {id="ApproveOrDeny"})) { %>
            <%= Html.AntiForgeryToken() %>
            <%= Html.Hidden("Approved") %>
            
            <%= Html.ClientSideValidation<Record>() %>
            
            <input id="Approve" type="submit" value="Approve" />
            <input id="Disapprove" type="submit" value="Disapprove" />
            <span>[Note: Comment Required For Disapproval]</span>
            <br />
            <label for="ReviewComment">Comment:</label>
            <%= Html.TextArea("ReviewComment", string.Empty, 5, 60, null) %>
            
        <% } %>
    </div>
    <% } %>
    
    <div>
        <%= Html.ActionLink<SupervisorController>(x=>x.TimeRecordList(), "Back to Time Record Review List") %>
    </div>

</asp:Content>