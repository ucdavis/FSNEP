///<reference path="jquery-1.3.2-vsdoc.js">
$(function() {
    DisplayMessage("Message OnLoad");

    $(".AddCalendarEntry").live('click', function() {
        var clicked = $(this);
        
        DisplayMessage("You clicked on the add entry for day " + GetIdFromElement(clicked, ElementType.Add));
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