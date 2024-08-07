'use strict';

var testInProgress = false;
var threads = 4;
var loadedBytes = new Array(threads).fill(0);
var lastLoadedBytes = new Array(threads).fill(0);
var downloadUrl = '/download.dat';
var xhrs = [];
var progressUpdateInterval;
var chunkSize = (1024 / threads) * 1024 * 1024; // 256MB per thread

const createDownload = (index, start, end) => {
    xhrs[index] = new XMLHttpRequest();

    xhrs[index].addEventListener('progress', (event) => {
        if (!testInProgress) return;
        loadedBytes[index] = event.loaded;
    });

    xhrs[index].open('GET', downloadUrl);
    xhrs[index].setRequestHeader('Range', `bytes=${start}-${end}`);
    xhrs[index].send();
};

const stopDownload = () => {
    if (!testInProgress) return;

    // Abort all ongoing XMLHttpRequests
    for (let i = 0; i < threads; i++) {
        if (xhrs[i]) {
            xhrs[i].abort();
        }
    }

    // Reset variables
    testInProgress = false;
    loadedBytes.fill(0);
    lastLoadedBytes.fill(0);

    // Clear the progress update interval
    clearInterval(progressUpdateInterval);

    // Reset the UI elements
    document.getElementById('downloadbutton').removeAttribute('disabled');
};

const startDownload = () => {
    if (testInProgress) return;
    testInProgress = true;
    document.getElementById('downloadbutton').setAttribute('disabled', 'disabled');
    document.getElementById('downStatus').classList.remove('d-none');
    document.getElementById('downStatusMbps').classList.remove('d-none');

    for (let i = 0; i < threads; i++) {
        let start = i * chunkSize;
        let end = start + chunkSize - 1;
        createDownload(i, start, end);
    }

    progressUpdateInterval = setInterval(updateStats, 800);
};

const updateStats = () => {
    let totalSpeed = 0;

    for (let i = 0; i < threads; i++) {
        let speed = (loadedBytes[i] - lastLoadedBytes[i]) / 0.8 / (1024 * 1024);
        totalSpeed += speed;
        lastLoadedBytes[i] = loadedBytes[i];
    }

    // Update view
    document.getElementById('downStatus').innerHTML = 'Speed: ' + totalSpeed.toFixed(2) + 'MB/s';
    document.getElementById('downStatusMbps').innerHTML = 'Speed: ' + (totalSpeed * 8).toFixed(2) + 'Mbps';

    if (downloadchartData.labels.length > 25) {
        downloadchartData.labels.shift();
        downloadchartData.datasets[0].data.shift();
    }
    downloadchartData.labels.push('');
    downloadchartData.datasets[0].data.push(totalSpeed.toFixed(2));
    window.myDownloadLine.update();
};

document.getElementById('downloadbutton').addEventListener('click', function () {
    startDownload();
});
