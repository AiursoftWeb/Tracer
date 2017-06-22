'use strict'
var downMaxTime = 0;
var download = function () {
    var st = new Date();
    $.get('/home/download?t=' + st.getMilliseconds(), function (data) {
        var et = new Date();
        var downloadTime = et - st;

        if (downloadTime > downMaxTime) {
            downMaxTime = downloadTime;
        }

        var speed = 3.0 / downloadTime * 1000;
        var minspeed = 3.0 / downMaxTime * 1000;

        if (speed < $('#speedlagfilter').val()) {
            trig('Downloader', speed + 'MB/s');
        }
        $('#downStatus').html('Speed: ' + speed.toFixed(2) + 'MB/s');
        $('#downMax').html('Min: ' + minspeed.toFixed(2) + 'MB/s');
        setTimeout(download, 0);
    });
};
download();