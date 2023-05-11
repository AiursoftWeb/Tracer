'use strict';

var testInProgress = false;
var testStartTime;
var loadedBytes = 0;
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
    $('#downMax').removeClass('d-none');

    download();
    progressUpdateInterval = setInterval(updateStats, 750);
};

const stopDownload = () => {
    testInProgress = false;
    xhr.abort();
    clearInterval(progressUpdateInterval);
    $('#downloadbutton').removeAttr('disabled');
};

const updateStats = () => {
    let currentTime = new Date();
    let elapsedTime = (currentTime - testStartTime) / 1000;
    let speed = loadedBytes / elapsedTime / (1024 * 1024);

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

