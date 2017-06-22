'use strict';
var downMaxTime = 0;
var download = function () {
    //thread safe
    if ($('#downloadbutton').attr('disabled') == 'disabled') {
        return;
    }
    $('#downloadbutton').attr('disabled', 'disabled');
    startdownload();
};

var startdownload = function () {
    //prepare
    var st = new Date();
    $.get('/home/download?t=' + st.getMilliseconds(), function (data) {
        //get time
        var et = new Date();
        var downloadTime = et - st;
        //update max value
        if (downloadTime > downMaxTime) {
            downMaxTime = downloadTime;
        }
        //get speed
        var speed = 3.0 / downloadTime * 1000;
        var minspeed = 3.0 / downMaxTime * 1000;
        //log
        if (speed < $('#speedlagfilter').val()) {
            trig('Downloader', speed + 'MB/s');
        }
        //update view
        $('#downStatus').html('Speed: ' + speed.toFixed(2) + 'MB/s');
        $('#downMax').html('Min: ' + minspeed.toFixed(2) + 'MB/s');
        setTimeout(startdownload, 0);
    });
}