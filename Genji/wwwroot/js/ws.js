function getWSAddress() {
    var ishttps = 'https:' == document.location.protocol ? true : false;
    var host = window.location.host;
    var head = ishttps ? "wss://" : "ws://"
    return head + host;
}
var webSocket;
var startTime = new Date();
var maxLag = 0;
$().ready(function () {
    webSocket = new WebSocket(getWSAddress() + "/Home/Pushing");
    webSocket.onopen = function () {
        $("#spanStatus").text("connected");
    };
    webSocket.onmessage = function (evt) {
        $("#spanStatus").html(evt.data);
        var wslag = new Date() - startTime;
        $("#wsStatus").html('<p>Current: ' + wslag + 'ms</p>');
        if (wslag > maxLag) {
            maxLag = wslag;
        }
        $("#wsStatus").append('<p>Max lag: ' + maxLag + "ms</p>");
        startTime = new Date();
    };
    webSocket.onerror = function (evt) {
        alert(evt.message);
    };
    webSocket.onclose = function () {
        $("#spanStatus").text("disconnected");
    };
});