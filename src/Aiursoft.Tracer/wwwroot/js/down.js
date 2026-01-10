'use strict';

var testInProgress = false;
var threads = 4;
var refreshPeriod = 0.8;
var loadedBytes = new Array(threads).fill(0);
var lastLoadedBytes = new Array(threads).fill(0);
var baseBytes = new Array(threads).fill(0);
var downloadUrl = '/download.dat';
var uploadUrl = '/upload';
var xhrs = [];
var progressUpdateInterval;
var uploadData = new Uint8Array(10 * 1024 * 1024); // 10MB

const createDownload = (index) => {
    xhrs[index] = new XMLHttpRequest();

    xhrs[index].addEventListener('progress', (event) => {
        if (!testInProgress) return;
        loadedBytes[index] = event.loaded;
    });

    xhrs[index].open('GET', downloadUrl);
    
    // Set the responseType to blob to allow reading the response as a binary string
    xhrs[index].responseType = 'blob';
    xhrs[index].send();
};

const createUpload = (index) => {
    xhrs[index] = new XMLHttpRequest();

    xhrs[index].upload.addEventListener('progress', (event) => {
        if (!testInProgress) return;
        loadedBytes[index] = baseBytes[index] + event.loaded;
    });

    xhrs[index].addEventListener('load', () => {
        if (!testInProgress) return;
        baseBytes[index] += uploadData.length;
        createUpload(index);
    });

    xhrs[index].open('POST', uploadUrl);
    xhrs[index].send(uploadData);
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
    baseBytes.fill(0);

    // Clear the progress update interval
    clearInterval(progressUpdateInterval);

    // Reset the UI elements
    document.getElementById('downloadbutton').removeAttribute('disabled');
};

const startDownload = () => {
    if (testInProgress) return;
    testInProgress = true;
    const direction = document.getElementById('transferDirection').value;
    document.getElementById('downloadbutton').setAttribute('disabled', 'disabled');
    document.getElementById('downStatus').classList.remove('d-none');
    document.getElementById('downStatusMbps').classList.remove('d-none');

    if (direction === 'download') {
        downloadchartData.datasets[0].label = 'Download Speed';
        document.getElementById('direction-icon').setAttribute('data-lucide', 'download-cloud');
    } else {
        downloadchartData.datasets[0].label = 'Upload Speed';
        document.getElementById('direction-icon').setAttribute('data-lucide', 'upload-cloud');
    }
    if (window.lucide) {
        window.lucide.createIcons();
    }

    for (let i = 0; i < threads; i++) {
        if (direction === 'download') {
            createDownload(i);
        } else {
            createUpload(i);
        }
    }

    progressUpdateInterval = setInterval(updateStats, refreshPeriod * 1000);
};

document.getElementById('transferDirection').addEventListener('change', function () {
    const direction = this.value;
    if (direction === 'download') {
        document.getElementById('direction-icon').setAttribute('data-lucide', 'download-cloud');
    } else {
        document.getElementById('direction-icon').setAttribute('data-lucide', 'upload-cloud');
    }
    if (window.lucide) {
        window.lucide.createIcons();
    }
});

document.getElementById('downloadbutton').addEventListener('click', function () {
    startDownload();
});

const updateStats = () => {
    let totalSpeed = 0;

    for (let i = 0; i < threads; i++) {
        let speed = (loadedBytes[i] - lastLoadedBytes[i]) / refreshPeriod / (1024 * 1024);
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
