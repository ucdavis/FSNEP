///<reference path="jquery-1.3.2-vsdoc.js">
$(function() {
    DisplayMessage("Message OnLoad");
    PopulateAccounts($("#Project"), $("#Account"));
    PopulateAccounts($("#Adjust_Project"), $("#Adjust_Account"));

    $(".AddCalendarEntry").live('click', function() {
        var clicked = $(this);

        var id = GetIdFromElement(clicked, ElementType.Add);

        DisplayMessage("You clicked on the add entry for day " + id);

        AddEntry(id);

    });

    $(".EditCalendarEntry").live('click', function() {
        var clicked = $(this);

        var id = GetIdFromElement(clicked, ElementType.Edit);

        DisplayMessage("You clicked on the edit entry for id " + id);

        EditEntry(id);
    });

    $(".DeleteCalendarEntry").live('click', function() {
        var clicked = $(this);

        var id = GetIdFromElement(clicked, ElementType.Delete);

        DisplayMessage("You clicked on the delete entry for id " + id);

        RemoveEntry(id);
    });

    $(".DeleteAdjustmentEntry").live('click', function() {
        var clicked = $(this);

        var id = GetIdFromElement(clicked, ElementType.DeleteAdjustment);

        DisplayMessage("You clicked on the delete adjustment entry for id " + id);

        RemoveAdjustmentEntry(id);
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
                    DisplayMessage("New Entry Added with id = " + result.Id);

                    $("#dialogTimeRecordEntry").dialog("close");

                    UpdateAddEntryUI(result.Id, $("#addRecordDay").val(), data.Hours);
                    UpdateTimeRecordTotalHours(result.HoursDelta);
                },
                'json'
            );
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

            //update UI
            $("#editEntry" + data.EntryId).html(data.Hours + " HRS ");
            $("#entryHours" + data.EntryId).val(data.Hours);

            $.post(
                serviceUrl,
                data,
                function(result) {
                    LogMessage("Edit Entry Result", result);
                    DisplayMessage("Entry Edited with id = " + result.Id);

                    $("#dialogTimeRecordEdit").dialog("close");

                    UpdateTimeRecordTotalHours(result.HoursDelta);
                },
                'json'
            );
        }
        else {
            alert("Entry not valid -- check your values");
        }

        return false;
    });

    $("#Adjust_Project").change(function() { PopulateAccounts(this, $("#Adjust_Account")); });
    $("#Project").change(function() { PopulateAccounts(this, $("#Account")); });

    //alert(new Date().getDate());

    $("#Adjust_AdjustmentDate").datepicker({
        numberOfMonths: 3,
        maxDate: '-' + new Date().getDate() + 'D'
    });

});

function UpdateAddEntryUI(id, date, hours) {
    var newEntry = $('<tr>')
            .attr('id', 'Entry' + id)
            .append(
                $('<td>')
                .append(
                    $('<span>')
                    .addClass('editEntry')
                    .append(
                        $('<a>')
                        .addClass('EditCalendarEntry')
                        .attr('href', 'javascript:;')
                        .attr('id', 'editEntry' + id)
                        .html(hours + ' HRS ')
                    )
                )
                .append(
                    $('<span>')
                    .addClass('deleteEntry')
                    .append(
                        $('<a>')
                        .addClass('DeleteCalendarEntry')
                        .attr('href', 'javascript:;')
                        .attr('id', 'deleteEntry' + id)
                        .html('X')
                    )
                )
                .append(
                    $('<input type="hidden" />')
                    .attr('id', 'entryHours' + id)
                    .val(hours)
                )
            );

    //Append to the tbody
    $("#entriesFor" + date).append(newEntry);
}

function RemoveEntry(entryId) {

    var data = { entryId: entryId, __RequestVerificationToken: __RequestVerificationToken };

    var entryRow = $("#Entry" + entryId);

    entryRow.fadeOut();

    $.post(
        Services.RemoveEntry,
        data,
        function(result) {
            DisplayMessage("Entry with Id " + result.Id + " removed");
            UpdateTimeRecordTotalHours(result.HoursDelta);
        },
        'json'
    );
}

function RemoveAdjustmentEntry(entryId) {

    var data = { entryId: entryId, __RequestVerificationToken: __RequestVerificationToken };

    var entryRow = $("#AdjustmentEntry" + entryId);

    entryRow.fadeOut();

    $.post(
        Services.RemoveEntry,
        data,
        function(result) {
            DisplayMessage("Adjustment Entry with Id " + result.Id + " removed");
            UpdateTimeRecordTotalHours(result.HoursDelta);
        },
        'json'
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

function PopulateAccounts(el, acctEl) {
    var projectId = $(el).val();

    //Get the new accounts for this project
    $.getJSON(
        Services.GetAccountsForProject + "/" + projectId,
        null,
        function(result) {
            OnPopulateAccountsComplete(result, acctEl);
        }

    );
}

function OnPopulateAccountsComplete(result, acctEl) {
    LogMessage("Result", result);
    
    acctEl.empty(); //remove the current options

    $(result).each(function() { //append in the new elements
        var acct = this;
        var newOption = $("<option>" + acct.Name + "</option>").val(acct.Id);

        acctEl.append(newOption);
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

    $("#Edit_ActivityType").val(result.ActivityType);

    $("#Edit_FundType_Readonly").html(result.FundType);
    $("#Edit_Project_Readonly").html(result.Project);
    $("#Edit_Account_Readonly").html(result.Account);
    
    $("#Edit_Hours").val(result.Hours);
    $("#Edit_Comment").val(result.Comment);

    ShowEditEntryLoadingMessage(false);
    
    DisplayMessage("Edit Entry Loaded");
}

function UpdateTimeRecordTotalHours(hoursDelta) {
    //Updated the time record totals
    var timeSheetHours = $("#timeSheetHours");

    var totalHours = parseFloat(timeSheetHours.html()) + parseFloat(hoursDelta);

    timeSheetHours.html(totalHours);
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
    Delete: "deleteEntry",
    DeleteAdjustment: "deleteAdjustmentEntry"
}