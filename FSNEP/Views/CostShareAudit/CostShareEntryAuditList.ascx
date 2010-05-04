<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<IEnumerable<FSNEP.Core.Domain.CostShareEntry>>" %>

<script type="text/javascript">
    $(function() {
        $('.Exclude').click(function() {
            var dialog = $('#dialogExcludeEntry');
            

            OpenDialog(dialog, null, "Exclude Entry", null);
        });
    });

    function OpenDialog(dialog /*The dialog DIV JQuery object*/, buttons /*Button collection */, title, onClose) {
        dialog.dialog("destroy"); //Reset the dialog to its initial state
        dialog.dialog({
            autoOpen: true,
            closeOnEscape: false,
            width: 400,
            //height: 600,
            modal: false,
            title: title,
            buttons: buttons,
            //show: 'fold',
            close: onClose
        });
    }
</script>

<%
    Html.Grid(Model)
        .DisplayAlternateMessageWhen(Model.Count() == 0, "No Cost Share Entries Found")
        .Name("CostShareEntries")
        .Columns(col =>
                     {
                         col.Add(x =>
                                     { %>
                                        <a class="Exclude" id="Exclude<%= x.Id %>" href="#">Exclude</a>
                                     <%});
                         col.Add(x => x.Amount);
                         col.Add(x => x.ExpenseType.Name).Title("Expense Type");
                         col.Add(x => { %>
                                <%= Html.ActionLink<FileController>(a=>a.ViewEntryFile(x.Id), "View File") %>
                                <%}).Title("Receipt");
                         col.Add(x => x.Project.Name).Title("Project");
                         col.Add(x => x.FundType.Name).Title("Fund Type");
                         col.Add(x => x.Account.Name).Title("Account");
                         col.Add(x => x.Description);
                         col.Add(x => x.Comment);
                     })
        .Render();
        %>

<div id="dialogExcludeEntry" class="EntryExcludeDialog" title="Exclude Entry" style="display:none">
    <span style="display: none;" id="spanLoadingEntryDetails">
        <br/>
        <img alt="" src="../Images/mozilla_blu.gif"/> 
        Loading ...
    </span>
    <div id="divEntryDetails">
        Lorem ipsum
    </div>
</div>