﻿'use strict';

var pingMaxlag = 0;
var pingStop = false;
var ping = function () {
    //thread safe
    if ($('#pingbutton').attr('disabled') === 'disabled') {
        return;
    }
    pingMaxlag = 0;
    $('#pingbutton').attr('disabled', 'disabled');
    $('#httpStatus').removeClass('d-none');
    $('#httpMax').removeClass('d-none');
    pingStop = false;
    startping();
};
var startping = function () {
    if (pingStop) {
        return;
    }
    //prepare
    var startTime = new Date();
    $.get('/Ping', function () {
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

var stopPing = function () {
    pingStop = true;
    $('#pingbutton').removeAttr('disabled', 'disabled');
};