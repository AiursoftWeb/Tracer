'use strict';
var trig = function (trigger, value) {
    var newTr = logTable.insertRow(-1);
    var newTd0 = newTr.insertCell();
    var newTd1 = newTr.insertCell();
    var newTd2 = newTr.insertCell();
    newTd0.innerText = new Date();
    newTd1.innerText = trigger;
    newTd2.innerText = value;
}

var startAll = function () {
    $('#startAllButton').attr('disabled', 'disabled');
    WsTest();
    ping();
    download();
};