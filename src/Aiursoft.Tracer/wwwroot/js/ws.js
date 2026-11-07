(() => {
    const start = document.getElementById('wsbutton');
    const stop = document.getElementById('stopwsbutton');
    const status = document.getElementById('spanStatus');
    const feedback = document.getElementById('ws-feedback');
    const current = document.getElementById('wsStatus');
    const maximum = document.getElementById('wsmax');
    const label = document.getElementById('ws-label');
    const text = (key, fallback) => document.querySelector(`#ws-loc [data-key="${key}"]`)?.textContent || fallback;
    let webSocket = null;
    let maxLag = 0;

    function showStatus(value, failed = false) {
        status.textContent = value;
        feedback.textContent = value;
        status.classList.toggle('bg-secondary', !failed);
        status.classList.toggle('bg-warning', failed);
        status.classList.toggle('text-dark', failed);
    }

    function finish(key, fallback, failed = false) {
        const socket = webSocket;
        webSocket = null;
        start.disabled = false;
        stop.disabled = true;
        label.textContent = failed ? text('retry', 'Retry') : text('start', 'Start Test');
        showStatus(text(key, fallback), failed);
        socket?.close();
    }

    start.addEventListener('click', () => {
        if (start.disabled) return;
        start.disabled = true;
        stop.disabled = false;
        label.textContent = text('start', 'Start Test');
        maxLag = 0;
        current.textContent = '—';
        maximum.textContent = '—';
        current.classList.remove('d-none');
        maximum.classList.remove('d-none');
        showStatus(text('connecting', 'Connecting...'));

        let socket;
        try {
            const protocol = location.protocol === 'https:' ? 'wss:' : 'ws:';
            socket = new WebSocket(protocol + '//' + location.host + '/Home/Pushing');
        } catch {
            finish('failed', 'Test failed. Try again.', true);
            return;
        }
        webSocket = socket;
        let started = new Date();
        socket.onopen = () => {
            if (webSocket !== socket) return;
            showStatus(text('connected', 'Connected'));
        };
        socket.onmessage = event => {
            if (webSocket !== socket) return;
            status.textContent = text('server-time', 'Server Time:') + ' ' + event.data;
            const lag = new Date() - started;
            started = new Date();
            maxLag = Math.max(maxLag, lag);
            if (lag > $('#wslagfilter').val()) trig('WebSocket', lag + 'ms');
            current.textContent = lag + 'ms';
            maximum.textContent = maxLag + 'ms';
            if (wschartData.labels.length > 80) {
                wschartData.labels.shift();
                wschartData.datasets[0].data.shift();
            }
            wschartData.labels.push('');
            wschartData.datasets[0].data.push(lag);
            window.myWSLine.update();
        };
        socket.onerror = () => {
            if (webSocket !== socket) return;
            finish('failed', 'Test failed. Try again.', true);
        };
        socket.onclose = () => {
            if (webSocket !== socket) return;
            finish('disconnected', 'Disconnected. Try again.', true);
        };
    });

    stop.addEventListener('click', () => {
        if (stop.disabled) return;
        finish('stopped', 'Stopped');
    });
})();
