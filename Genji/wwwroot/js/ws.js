'use strict'
function getWSAddress() {
    var ishttps = 'https:' == document.location.protocol ? true : false;
    var host = window.location.host;
    var head = ishttps ? "wss://" : "ws://"
    return head + host;
}
var webSocket;
var wsStartTime = new Date();
var wsMaxLag = 0;
$().ready(function () {
    webSocket = new WebSocket(getWSAddress() + "/Home/Pushing");
    webSocket.onopen = function () {
        $("#spanStatus").text("connected");
    };
    webSocket.onmessage = function (evt) {
        $("#spanStatus").html(evt.data);
        var wslag = new Date() - wsStartTime;
        $('#wsStatus').html('Current: ' + wslag + 'ms');
        if (wslag > wsMaxLag) {
            wsMaxLag = wslag;
        }
        if (wslag > $('#wslagfilter').val()) {
            trig('WebSockeet', wslag + 'ms');
        }
        $("#wsmax").html('Max: ' + wsMaxLag + 'ms');
        wsStartTime = new Date();
    };
    webSocket.onerror = function (evt) {
        alert(evt.message);
    };
    webSocket.onclose = function () {
        $("#spanStatus").text("disconnected");
    };
});