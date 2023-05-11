'use strict';

var testInProgress = false;
var testStartTime;
var loadedBytes = 0;
var lastLoadedBytes = 0;
var downloadUrl = '/home/download';
var xhr;
var progressUpdateInterval;

const download = () => {
    testStartTime = new Date();
    xhr = new XMLHttpRequest();

    xhr.addEventListener('progress', (event) => {
        if (!testInProgress) return;
        loadedBytes = event.loaded;
    });

    xhr.open('GET', downloadUrl);
    xhr.send();
};

const startDownload = () => {
    if (testInProgress) return;
    testInProgress = true;
    $('#downloadbutton').attr('disabled', 'disabled');
    $('#downStatus').removeClass('d-none');
    $('#downStatusMbps').removeClass('d-none');

    download();
    progressUpdateInterval = setInterval(updateStats, 800);
};

const stopDownload = () => {
    testInProgress = false;
    xhr.abort();
    clearInterval(progressUpdateInterval);
    $('#downloadbutton').removeAttr('disabled');
};

const updateStats = () => {
    let currentTime = new Date();
    let speed = (loadedBytes - lastLoadedBytes) / (0.8) / (1024 * 1024);

    // Update last loaded bytes
    lastLoadedBytes = loadedBytes;

    // Update view
    $('#downStatus').html('Speed: ' + speed.toFixed(2) + 'MB/s');
    $('#downStatusMbps').html('Speed: ' + (speed * 8).toFixed(2) + 'Mbps');

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