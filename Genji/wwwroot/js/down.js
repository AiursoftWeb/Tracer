var maxTime = 0;
var download = function () {
    var st = new Date();
    $.get('/home/download', function (data) {
        var et = new Date();
        var downloadTime = et - st;
        if (downloadTime > maxTime) {
            maxTime = downloadTime;
        }
        $('#downStatus').html('<p>Speed: ' + (1.0 / downloadTime * 1000).toFixed(2) + 'MB/s</p>');
        $('#downStatus').append('<p>Min speed: ' + (1.0 / maxTime * 1000).toFixed(2) + 'MB/s</p>');
        setTimeout(download, 0);
    });
};
download();