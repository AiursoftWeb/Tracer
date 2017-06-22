var maxlag = 0;
var ping = function () {
    var startTime = new Date();
    $.get('/Home/Ping', function (data) {
        var endtime = new Date();
        var lag = endtime - startTime;
        if (lag > maxlag) {
            maxlag = lag;
        }
        $('#httpStatus').html('<p>Current: ' + lag + 'ms</p>');
        $('#httpStatus').append('<p>Max lag: ' + maxlag + 'ms</p>');
        setTimeout(ping, 30);
    });
};
ping();