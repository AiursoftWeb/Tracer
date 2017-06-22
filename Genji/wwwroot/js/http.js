'use strict'
var pingMaxlag = 0;

var ping = function () {
    var startTime = new Date();
    $.get('/Home/Ping', function (data) {
        var endtime = new Date();
        var lag = endtime - startTime;
        if (lag > pingMaxlag) {
            pingMaxlag = lag;
        }
        if (lag > $('#pinglagfilter').val()) {
            trig('HTTP Get', lag + 'ms');
        }
        $('#httpStatus').html('Current: ' + lag + 'ms');
        $('#httpMax').html('Max lag: ' + pingMaxlag + 'ms');
        setTimeout(ping, 30);
    });
};
ping();