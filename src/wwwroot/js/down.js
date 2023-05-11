'use strict';

var testInProgress = false;
var completedRequests = 0;
var testInterval;
var requestInterval;
var downloadUrl = '/home/download';

const download = () => {
    let st = new Date();

    $.get(downloadUrl + '?t=' + st.getMilliseconds())
        .done(() => {
            if (!testInProgress) return;
            completedRequests++;
        });
}

const startDownload = () => {
    if (testInProgress) return;
    testInProgress = true;
    $('#downloadbutton').attr('disabled', 'disabled');
    $('#downStatus').removeClass('d-none');
    $('#downMax').removeClass('d-none');

    requestInterval = setInterval(download, 0);
    testInterval = setInterval(updateStats, 1000);
};

const stopDownload = () => {
    testInProgress = false;
    clearInterval(requestInterval);
    clearInterval(testInterval);
    $('#downloadbutton').removeAttr('disabled');
};

const updateStats = () => {
    let speed = completedRequests;
    completedRequests = 0;

    // Update view
    $('#downStatus').html('Speed: ' + speed.toFixed(2) + 'MB/s');

    if (downloadchartData.labels.length > 25) {
        downloadchartData.labels.shift();
        downloadchartData.datasets[0].data.shift();
    }
    downloadchartData.labels.push('');
    downloadchartData.datasets[0].data.push(speed.toFixed(2));
    window.myDownloadLine.update();
};

$('#downloadbutton').on('click', function () {
    if (testInProgress) {
        stopDownload();
    } else {
        startDownload();
    }
});
