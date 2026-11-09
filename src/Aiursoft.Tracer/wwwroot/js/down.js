'use strict';

var testInProgress = false;
var threads = 4;
var refreshPeriod = 0.8;
var loadedBytes = [];
var lastLoadedBytes = [];
var baseBytes = [];
var downloadUrl = '/download.dat';
var uploadUrl = '/upload';
var xhrs = [];
var progressUpdateInterval;
var uploadData = new Uint8Array(100 * 1024 * 1024); // 100MB

let transferRun = 0;
let completedDownloads = 0;
const transferStart = document.getElementById('downloadbutton');
const transferStop = document.getElementById('stopdownloadbutton');
const transferDirection = document.getElementById('transferDirection');
const transferThreads = document.getElementById('threads');
const transferStatus = document.getElementById('transfer-status');
const transferLabel = document.getElementById('transfer-label');
const transferText = (key, fallback) => document.querySelector(`#bandwidth-loc [data-key="${key}"]`)?.textContent || fallback;

const showTransferStatus = (message, failed = false) => {
    transferStatus.textContent = message;
    transferStatus.classList.remove('d-none');
    transferStatus.classList.toggle('text-body', failed);
    transferStatus.classList.toggle('text-muted', !failed);
};

const stopDownload = (state = 'stopped') => {
    if (!testInProgress) return;
    // Invalidate callbacks before aborting: abort events may fire synchronously.
    testInProgress = false;
    ++transferRun;
    clearInterval(progressUpdateInterval);
    xhrs.forEach(request => request?.abort());
    transferStart.disabled = false;
    transferStop.disabled = true;
    transferDirection.disabled = false;
    transferThreads.disabled = false;
    transferLabel.textContent = state === 'failed' ? transferText('retry', 'Retry') : transferText('start', 'Start Test');
    const messages = {
        stopped: transferText('stopped', 'Stopped'),
        completed: transferText('completed', 'Completed'),
        failed: transferText('failed', 'Test failed. Try again.')
    };
    showTransferStatus(messages[state], state === 'failed');
};

const createTransfer = (index, direction, currentRun) => {
    if (!testInProgress || currentRun !== transferRun) return;
    const request = new XMLHttpRequest();
    xhrs[index] = request;
    const isCurrent = () => testInProgress && currentRun === transferRun && xhrs[index] === request;
    const fail = () => {
        if (isCurrent()) stopDownload('failed');
    };
    const progressTarget = direction === 'download' ? request : request.upload;
    progressTarget.addEventListener('progress', event => {
        if (!isCurrent()) return;
        loadedBytes[index] = baseBytes[index] + event.loaded;
    });
    request.addEventListener('load', () => {
        if (!isCurrent()) return;
        if (request.status < 200 || request.status >= 300) {
            fail();
            return;
        }
        if (direction === 'upload') {
            baseBytes[index] += uploadData.length;
            createTransfer(index, direction, currentRun);
        } else if (++completedDownloads === threads) {
            stopDownload('completed');
        }
    });
    ['error', 'timeout', 'abort'].forEach(event => request.addEventListener(event, fail));
    try {
        request.open(direction === 'download' ? 'GET' : 'POST', direction === 'download' ? downloadUrl : uploadUrl);
        if (direction === 'download') request.responseType = 'blob';
        request.send(direction === 'download' ? null : uploadData);
    } catch {
        fail();
    }
};

const startDownload = () => {
    if (testInProgress) return;
    testInProgress = true;
    const currentRun = ++transferRun;
    completedDownloads = 0;
    threads = parseInt(document.getElementById('threads').value);
    if (isNaN(threads) || threads < 1) threads = 1;
    if (threads > 16) threads = 16;

    loadedBytes = new Array(threads).fill(0);
    lastLoadedBytes = new Array(threads).fill(0);
    baseBytes = new Array(threads).fill(0);
    xhrs = new Array(threads);

    const direction = document.getElementById('transferDirection').value;
    transferStart.disabled = true;
    transferStop.disabled = false;
    transferDirection.disabled = true;
    transferThreads.disabled = true;
    transferLabel.textContent = transferText('start', 'Start Test');
    showTransferStatus(transferText('running', 'Testing...'));
    document.getElementById('downStatus').textContent = '—';
    document.getElementById('downStatusMbps').textContent = '—';
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

    progressUpdateInterval = setInterval(() => {
        if (testInProgress && currentRun === transferRun) updateStats();
    }, refreshPeriod * 1000);
    for (let i = 0; i < threads; i++) {
        createTransfer(i, direction, currentRun);
    }
};

transferDirection.addEventListener('change', function () {
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

transferStop.addEventListener('click', () => stopDownload());

transferStart.addEventListener('click', function () {
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
