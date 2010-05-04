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

        DisplayMessage("You clicked on the delete entry for id " + GetIdFromElement(clicked, ElementType.Delete));
    });

    $("#formAddEntry").submit(function() {
        //hijax the form post and do it ajax style
        return false;
    });

    $("#Project").change(function() { PopulateAccounts(this); });
});

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