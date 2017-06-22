var dfmaxlag = 0;
var download = function () {
    var st = new Date();
    $.get('/home/download', function (data) {
        var et = new Date();
        var downloadcost = et - st;
        if (downloadcost > dfmaxlag) {
            dfmaxlag = downloadcost;
        }
        $('#downStatus').html('<p>Speed: ' + (10.0 / downloadcost * 1000).toFixed(2) + 'MB/s</p>');
        setTimeout(download, 30);
    });
};
download();