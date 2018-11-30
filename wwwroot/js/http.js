'use strict';
var pingMaxlag = 0;
var ping = function () {
    //thread safe
    if ($('#pingbutton').attr('disabled') === 'disabled') {
        return;
    }
    $('#pingbutton').attr('disabled', 'disabled');
    startping();
};
var startping = function () {
    //prepare
    var startTime = new Date();
    $.get('/Ping', function (data) {
        //get time
        var endtime = new Date();
        var lag = endtime - startTime - 7;
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
        if (chartData.labels.length > 25) {
            chartData.labels.shift();
            chartData.datasets[0].data.shift();
        }
        chartData.labels.push('');
        chartData.datasets[0].data.push(lag);
        window.myLine.update();

        setTimeout(startping, 1000);
    });
};
