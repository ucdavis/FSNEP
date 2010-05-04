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

        var id = GetIdFromElement(clicked, ElementType.Edit);

        DisplayMessage("You clicked on the edit entry for id " + GetIdFromElement(clicked, ElementType.Edit));

        EditEntry(id);
    });

    $(".DeleteCalendarEntry").live('click', function() {
        var clicked = $(this);

        var id = GetIdFromElement(clicked, ElementType.Delete);

        DisplayMessage("You clicked on the delete entry for id " + GetIdFromElement(clicked, ElementType.Delete));

        RemoveEntry(id);
    });

    $("#formAddEntry").submit(function() {
        //hijax the form post and do it ajax style
        if ($(this).valid()) {  //validate the form

            var data = GatherAddEntryData();
            var serviceUrl = Services.AddEntry;

            $.post(
                serviceUrl,
                data,
                function(result) {
                    LogMessage("Add Entry Result", result);
                    DisplayMessage("New Entry Added with id = " + result.id);
                },
                'json'
            );
            
            //updated UI
        }
        else {
            alert("Entry not valid -- check your values");
        }

        return false;
    });

    $("#formEditEntry").submit(function() {
        if ($(this).valid()) {
            var data = GatherEditEntryData();
            var serviceUrl = Services.EditEntry;

            $.post(
                serviceUrl,
                data,
                function(result) {
                    LogMessage("Edit Entry Result", result);
                    DisplayMessage("Entry Edited with id = " + result.id);
                },
                'json'
            );
            
            //update UI
            debugger;
        }
        else {
            alert("Entry not valid -- check your values");
        }

        return false;
    });

    $("#Project").change(function() { PopulateAccounts(this); });
});

function RemoveEntry(entryId) {

    var data = { entryId: entryId, __RequestVerificationToken: __RequestVerificationToken };

    var entryRow = $("#Entry" + entryId);

    entryRow.fadeOut();

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

    var data = { Date: date, Hours: hours, ActivityType: activityType, FundType: fundType,
                    Project: project, Account: account, Comment: comment, 
                    __RequestVerificationToken: __RequestVerificationToken };

    return data;
}

function GatherEditEntryData() {
    var entryId = $("#Edit_EntryId").val();

    var hours = $("#Edit_Hours").val();
    var activityType = $("#Edit_ActivityType").val();

    var fundType = $("#Edit_FundType").val();
    var project = $("#Edit_Project").val();
    var account = $("#Edit_Account").val();
    var comment = $("#Edit_Comment").val();

    var data = { EntryId: entryId, Hours: hours, ActivityType: activityType, FundType: fundType,
        Project: project, Account: account, Comment: comment,
        __RequestVerificationToken: __RequestVerificationToken
    };

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

function EditEntry(entryId) {
    var editEntryDiv = $("#dialogTimeRecordEdit");

    ShowEditEntryLoadingMessage(true);

    $.getJSON(
        Services.GetEntry,
        { entryId: entryId },
        EditEntryLoaded,
        'json'
    );

    OpenDialog(editEntryDiv, null, "Edit Entry", null);
}

function EditEntryLoaded(result) {
    LogMessage("Edit Entry Loaded", result);

    $("#Edit_EntryId").val(result.Id);
    
    $("#Edit_ActivityType").html(result.ActivityType);
    $("#Edit_FundType").html(result.FundType);
    $("#Edit_Project").html(result.Project);
    $("#Edit_Account").html(result.Account);
    
    $("#Edit_Hours").val(result.Hours);
    $("#Edit_Comment").val(result.Comment);

    ShowEditEntryLoadingMessage(false);
    
    DisplayMessage("Edit Entry Loaded");
}

function ShowEditEntryLoadingMessage(loading) {
    var spanLoadingEntryDetails = $("#spanLoadingEntryDetails");
    var divEntryDetails = $("#divEntryDetails");

    if (loading) {
        spanLoadingEntryDetails.show();
        divEntryDetails.css('visibility', 'hidden');
    }
    else {
        spanLoadingEntryDetails.hide();
        divEntryDetails.css('visibility', 'visible');
    }
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