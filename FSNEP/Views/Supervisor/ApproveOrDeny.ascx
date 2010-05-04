<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Record>" %>
<%@ Import Namespace="FSNEP.Core.Domain"%>

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

<% if (Model.CanBeApprovedOrDenied) { %>
<div>
    <% using (Html.BeginForm("ApproveOrDenyRecord", "Supervisor", new {id=Model.Id}, FormMethod.Post, new {id="ApproveOrDeny"})) { %>
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