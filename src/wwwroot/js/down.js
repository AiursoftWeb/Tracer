'use strict';

var testInProgress = false;
var loadedBytes = 0;
var lastLoadedBytes = 0;
var downloadUrl = '/download.dat';
var xhr;
var progressUpdateInterval;

const download = () => {
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
    document.getElementById('downloadbutton').setAttribute('disabled', 'disabled');
    document.getElementById('downStatus').classList.remove('d-none');
    document.getElementById('downStatusMbps').classList.remove('d-none');

    download();
    progressUpdateInterval = setInterval(updateStats, 800);
};

const stopDownload = () => {
    testInProgress = false;
    loadedBytes = 0;
    lastLoadedBytes = 0;
    xhr.abort();
    clearInterval(progressUpdateInterval);
    document.getElementById('downloadbutton').removeAttribute('disabled');
};

const updateStats = () => {
    let currentTime = new Date();
    let speed = (loadedBytes - lastLoadedBytes) / (0.8) / (1024 * 1024);

    // Update last loaded bytes
    lastLoadedBytes = loadedBytes;

    // Update view
    document.getElementById('downStatus').innerHTML = 'Speed: ' + speed.toFixed(2) + 'MB/s';
    document.getElementById('downStatusMbps').innerHTML = 'Speed: ' + (speed * 8).toFixed(2) + 'Mbps';

    if (downloadchartData.labels.length > 25) {
        downloadchartData.labels.shift();
        downloadchartData.datasets[0].data.shift();
    }
    downloadchartData.labels.push('');
    downloadchartData.datasets[0].data.push(speed.toFixed(2));
    window.myDownloadLine.update();
};

document.getElementById('downloadbutton').addEventListener('click', function () {
    if (testInProgress) {
        stopDownload();
    } else {
        startDownload();
    }
});
