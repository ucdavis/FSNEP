///<reference path="jquery-1.3.2-vsdoc.js">
$(function() {
    DisplayMessage("Message OnLoad");

    $(".AddCalendarEntry").live('click', function() {
        var clicked = $(this);
        
        DisplayMessage("You clicked on the add entry for day " + GetIdFromElement(clicked, "addEntry"));
    });

    $(".EditCalendarEntry").live('click', function() {
        var clicked = $(this);

        DisplayMessage("You clicked on the edit entry for id " + GetIdFromElement(clicked, "editEntry"));
    });
});

function DisplayMessage(message){
    $().message(message);
}

function GetIdFromElement(el, prefix) {
    var str = el.attr('id');
    var id = str.toString().substring(prefix.length, str.length);

    return id;
}