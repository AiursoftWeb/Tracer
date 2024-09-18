'use strict';

var trig = function (trigger, value) {
    var newTr = logTable.insertRow(-1);
    var newTd0 = newTr.insertCell();
    var newTd1 = newTr.insertCell();
    var newTd2 = newTr.insertCell();
    newTd0.innerText = new Date();
    newTd1.innerText = trigger;
    newTd2.innerText = value;
};

var ctx = document.getElementById('httpChart').getContext('2d');
var downloadChartCtx = document.getElementById('downloadChart').getContext('2d');
var wsChartCtx = document.getElementById('wsChart').getContext('2d');

var chartData = {
    labels: [],
    datasets: [{
        label: "HTTP Lag",
        backgroundColor: 'rgb(255, 99, 132)',
        borderColor: 'rgb(255, 99, 132)',
        fill: false,
        data: []
    }]
};

var downloadchartData = {
    labels: [],
    datasets: [{
        label: "Download Speed",
        backgroundColor: 'rgb(70, 170, 252)',
        borderColor: 'rgb(70, 170, 252)',
        fill: true,
        data: []
    }]
};

var wschartData = {
    labels: [],
    datasets: [{
        label: "WebSocket Connection",
        backgroundColor: 'rgb(2, 232, 99)',
        borderColor: 'rgb(2, 232, 99)',
        fill: false,
        data: []
    }]
};

var chartOption = {
    responsive: true,
    tooltips: {
        mode: 'index',
        intersect: false
    },
    hover: {
        mode: 'nearest',
        intersect: true
    },
    scales: {
        xAxes: [{
            display: true,
            scaleLabel: {
                display: true,
                labelString: 'Time'
            }
        }],
        yAxes: [{
            display: true,
            scaleLabel: {
                display: true,
                labelString: 'Value'
            },
            ticks: {
                beginAtZero: true
            }
        }]
    }
};

chartOption.scales.yAxes[0].ticks.suggestedMax = 120;
window.myLine = new Chart(ctx, {
    type: 'line',
    data: chartData,
    options: chartOption
});

chartOption.scales.yAxes[0].ticks.suggestedMax = 20;
window.myDownloadLine = new Chart(downloadChartCtx, {
    type: 'line',
    data: downloadchartData,
    options: chartOption
});

chartOption.scales.yAxes[0].ticks.beginAtZero = false;
chartOption.scales.yAxes[0].ticks.suggestedMin = 80;
chartOption.scales.yAxes[0].ticks.suggestedMax = 120;
window.myWSLine = new Chart(wsChartCtx, {
    type: 'line',
    data: wschartData,
    options: chartOption
});