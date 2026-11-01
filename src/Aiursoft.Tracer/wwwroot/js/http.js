(() => {
    const start = document.getElementById('pingbutton');
    const stop = document.getElementById('stop-pingbutton');
    const status = document.getElementById('httpStatus');
    const maximum = document.getElementById('httpMax');
    const label = document.getElementById('ping-label');
    const text = (key, fallback) => document.querySelector(`#http-loc [data-key="${key}"]`)?.textContent || fallback;
    let run = 0;
    let request = null;
    let timer;
    let maxLag = 0;

    function showStatus(value, failed = false) {
        status.textContent = value;
        status.classList.remove('d-none');
        status.classList.toggle('bg-info', !failed);
        status.classList.toggle('bg-warning', failed);
        status.classList.toggle('text-dark', failed);
    }

    function measure(currentRun) {
        if (currentRun !== run) return;
        const started = new Date();
        request = $.ajax({ url: '/Ping', timeout: 10000 });
        request.done(() => {
            if (currentRun !== run) return;
            request = null;
            const lag = new Date() - started;
            maxLag = Math.max(maxLag, lag);
            if (lag > $('#pinglagfilter').val()) trig('HTTP Get', lag + 'ms');
            showStatus(lag + 'ms');
            maximum.textContent = maxLag + 'ms';
            if (chartData.labels.length > 25) {
                chartData.labels.shift();
                chartData.datasets[0].data.shift();
            }
            chartData.labels.push('');
            chartData.datasets[0].data.push(lag);
            window.myLine.update();
            timer = setTimeout(() => measure(currentRun), 1000);
        });
        request.fail(() => {
            if (currentRun !== run) return;
            request = null;
            start.disabled = false;
            stop.disabled = true;
            label.textContent = text('retry', 'Retry');
            showStatus(text('failed', 'Test failed. Try again.'), true);
        });
    }

    start.addEventListener('click', () => {
        if (start.disabled) return;
        start.disabled = true;
        stop.disabled = false;
        label.textContent = text('start', 'Start Test');
        maxLag = 0;
        maximum.textContent = '—';
        maximum.classList.remove('d-none');
        showStatus(text('waiting', 'Waiting...'));
        measure(++run);
    });

    stop.addEventListener('click', () => {
        ++run;
        clearTimeout(timer);
        request?.abort();
        request = null;
        start.disabled = false;
        stop.disabled = true;
        label.textContent = text('start', 'Start Test');
        showStatus(text('stopped', 'Stopped'));
    });
})();
