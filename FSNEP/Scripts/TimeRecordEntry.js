///<reference path="jquery-1.3.2-vsdoc.js">
$(function() {
    DisplayMessage("Message OnLoad");
    PopulateAccounts($("#Project"));

    $(".AddCalendarEntry").live('click', function() {
        var clicked = $(this);

        var id = GetIdFromElement(clicked, ElementType.Add);

        DisplayMessage("You clicked on the add entry for day " + id);

        AddEntry(id);

    });

    $(".EditCalendarEntry").live('click', function() {
        var clicked = $(this);

        DisplayMessage("You clicked on the edit entry for id " + GetIdFromElement(clicked, ElementType.Edit));
    });

    $(".DeleteCalendarEntry").live('click', function() {
        var clicked = $(this);

        var id = GetIdFromElement(clicked, ElementType.Delete);

        RemoveEntry(id);

        DisplayMessage("You clicked on the delete entry for id " + GetIdFromElement(clicked, ElementType.Delete));
    });

    $("#formAddEntry").submit(function() {
        //hijax the form post and do it ajax style
        var valid = $(this).valid();

        if ($(this).valid()) {  //validate the form

            var data = GatherAddEntryData();
            var serviceUrl = Services.AddEntry;

            $.post(
                Services.AddEntry,
                data,
                function(result) {
                    LogMessage("Add Entry Result", result);
                    DisplayMessage("New Entry Added with id = " + result.id);
                },
                'json'
            );
        }

        return false;
    });

    $("#Project").change(function() { PopulateAccounts(this); });
});

function RemoveEntry(entryId) {
    var data = { entryId: entryId };

    $.post(
        Services.RemoveEntry,
        data,
        function() {
            DisplayMessage("Entry with Id " + entryId + " removed");
        }
    );
}

function GatherAddEntryData() {
    var date = $("#addRecordDay").val();
    
    var hours = $("#Hours").val();
    var activityType = $("#ActivityType").val();

    var fundType = $("#FundType").val();
    var project = $("#Project").val();
    var account = $("#Account").val();
    var comment = $("#Comment").val();

    var data = { Date : date, Hours: hours, ActivityType: activityType, FundType: fundType, Project: project, Account: account, Comment: comment };

    return data;
}

function PopulateAccounts(el) {
    var projectId = $(el).val();

    //Get the new accounts for this project
    $.getJSON(
        Services.GetAccountsForProject + "/" + projectId,
        null,
        OnPopulateAccountsComplete);       
}

function OnPopulateAccountsComplete(result) {
    LogMessage("Result", result);
    
    var accountSelect = $("#Account");
    accountSelect.empty(); //remove the current options

    $(result).each(function() { //append in the new elements
        var acct = this;
        var newOption = $("<option>" + acct.Name + "</option>").val(acct.Id);

        accountSelect.append(newOption);
    });
}

function AddEntry(id) {
    var addEntryDiv = $("#dialogTimeRecordEntry");

    var addRecordDay = $("#addRecordDay");
    addRecordDay.val(id);
    
    OpenDialog(addEntryDiv, null, "Add Entry", null);
}

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

function DisplayMessage(message){
    $().message(message);
}

function LogMessage(message, data) {

    var canDebug = typeof (window.console) != "undefined";

    if (canDebug) {
        console.log(message, data);
    }
}

function GetIdFromElement(el, elementType) {
    var str = el.attr('id');
    var prefix = elementType;
    var id = str.toString().substring(prefix.length, str.length);

    return id;    
}

var ElementType = {
    Add: "addEntry",
    Edit: "editEntry",
    Delete: "deleteEntry"
}