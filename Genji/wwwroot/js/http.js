'use strict';
var pingMaxlag = 0;
var ping = function () {
    //prepare
    $('#pingbutton').attr("disabled", true);
    var startTime = new Date();
    $.get('/Home/Ping', function (data) {
        //get time
        var endtime = new Date();
        var lag = endtime - startTime;
        //update max value
        if (lag > pingMaxlag) {
            pingMaxlag = lag;
        }
        //log
        if (lag > $('#pinglagfilter').val()) {
            trig('HTTP Get', lag + 'ms');
        }
        //update view
        $('#httpStatus').html('Current: ' + lag + 'ms');
        $('#httpMax').html('Max lag: ' + pingMaxlag + 'ms');
        setTimeout(ping, 30);
    });
};
