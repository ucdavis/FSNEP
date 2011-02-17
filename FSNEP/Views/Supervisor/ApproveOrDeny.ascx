<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Record>" %>
<%@ Import Namespace="FSNEP.Core.Domain"%>

 <script type="text/javascript">
    var approved;

    $(function () {
        approved = $("#Approved");

        $("#Approve").click(function () {
            approved.val(true);
        });

        $("#Disapprove").click(function () {
            if ($("#ReviewComment").val()) {
                approved.val(false);
            }
            else {
                alert("Comment Required for Disapproval");
                return false;
            }
        });
    });
</script>

<% if (Model.CanBeApprovedOrDenied) { %>
<div>
    <% using (Html.BeginForm("ApproveOrDenyRecord", "Supervisor", new {id=Model.Id}, FormMethod.Post, new {id="ApproveOrDeny"})) { %>
        <%= Html.AntiForgeryToken() %>
        <%= Html.Hidden("Approved") %>
        
        <%= Html.ClientSideValidation<Record>() %>
        
        <label for="ReviewComment">Comment:</label>
        <p class="labelnote">[Note: Comment Required For Disapproval]</p>
        <%= Html.TextArea("ReviewComment", string.Empty, 5, 60, null) %>
        <br /><br />
        <input id="Approve" type="submit" value="Approve" />
        <input id="Disapprove" type="submit" value="Disapprove" />
        
    <% } %>
</div>
<% } %>