var maxTime = 0;
var download = function () {
    var st = new Date();
    $.get('/home/download?t=' + st.getMilliseconds(), function (data) {
        var et = new Date();
        var downloadTime = et - st;
        if (downloadTime > maxTime) {
            maxTime = downloadTime;
        }
        $('#downStatus').html('<p>Speed: ' + (10.0 / downloadTime * 1000).toFixed(2) + 'MB/s</p>');
        $('#downStatus').append('<p>Min speed: ' + (10.0 / maxTime * 1000).toFixed(2) + 'MB/s</p>');
        setTimeout(download, 1000);
    });
};
download();