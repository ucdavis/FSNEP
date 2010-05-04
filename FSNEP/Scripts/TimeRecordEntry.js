///<reference path="jquery-1.3.2-vsdoc.js">
$(function() {
    DisplayMessage("Message OnLoad");

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
});

function AddEntry(id) {
    var addEntryDiv = $("#dialogTimeRecordEntry");

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