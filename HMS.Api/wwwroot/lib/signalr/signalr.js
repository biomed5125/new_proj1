// /wwwroot/js/lab-live.js
// Requires: <script src="/lib/signalr/signalr.js"></script> already on the page.

(async () => {
    // ensure ULs have the console style
    function ensureConsole(ulId) {
        const el = document.getElementById(ulId);
        if (!el) return null;
        el.classList.add('console');
        return el;
    }

    const resultsUl = ensureConsole('liveResultsLog'); // newest-first list in your page
    const traceUl = ensureConsole('traceLog');       // newest-first list in your page

    // add a line (newest first), keep list snappy
    function prependLine(ul, html) {
        if (!ul) return;
        const nearTop = ul.scrollTop <= 5;
        const li = document.createElement('li');
        li.innerHTML = html;
        ul.prepend(li);
        // trim to last 250 lines for speed
        while (ul.children.length > 250) ul.removeChild(ul.lastChild);
        if (nearTop) ul.scrollTop = 0;
    }

    // small helpers
    const fmtT = (isoOrTicks) => {
        try { return new Date(isoOrTicks).toLocaleTimeString(); }
        catch { return ''; }
    };
    const flagClass = (f) => {
        if (!f) return '';
        const up = ('' + f).toUpperCase();
        if (up === 'H' || up === 'HIGH' || up === 'CRIT') return 'flag H';
        if (up === 'L' || up === 'LOW') return 'flag L';
        return 'flag';
    };

    // Build pretty line html for ResultPosted (no IN/OUT in this DTO; neutral line)
    function lineForResult(dto) {
        const ts = `<span class="ts">[${fmtT(dto.at)}]</span>`;
        const acc = dto.accession ? `<span class="acc">${dto.accession}</span>` : '';
        const test = dto.instrumentCode ? `<span class="test">${dto.instrumentCode}</span>` : '';
        const val = `<span class="val">${dto.value ?? ''}</span>`;
        const unit = dto.units ? `<span class="unit">${dto.units}</span>` : '';
        const flag = dto.flag ? `<span class="${flagClass(dto.flag)}">${dto.flag}</span>` : '';
        const dev = dto.deviceId ? `<span class="dev">dev:${dto.deviceId}</span>` : '';
        return `${ts}${dev}${acc}${test}<span class="payload">${val}${unit}</span>${flag}`;
    }

    // Build pretty line html for Trace-kind events (if you forward them via LabHub)
    // NOTE: Lab Hub's HubEventDto doesn't carry direction; we show as neutral.
    function lineForTrace(dto) {
        const ts = `<span class="ts">[${fmtT(dto.at)}]</span>`;
        const dev = dto.deviceId ? `<span class="dev">dev:${dto.deviceId}</span>` : '';
        const acc = dto.accession ? `<span class="acc">${dto.accession}</span>` : '';
        const test = dto.instrumentCode ? `<span class="test">${dto.instrumentCode}</span>` : '';
        const val = dto.value ? `<span class="val">${dto.value}</span>` : '';
        const unit = dto.units ? `<span class="unit">${dto.units}</span>` : '';
        return `${ts}${dev}${acc}${test}${val}${unit}`;
    }

    // connect to Lab Hub
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/hubs/lab")
        .withAutomaticReconnect()
        .build();

    // Single handler – your HubEventDto has: at, deviceId, kind, accession, instrumentCode, value, units, flag
    connection.on("TraceEvent", dto => {
        // Normalize property casing (just in case)
        const normalized = {
            at: dto.at ?? dto.At ?? dto.atUtc ?? dto.AtUtc,
            deviceId: dto.deviceId ?? dto.DeviceId,
            kind: (dto.kind ?? dto.Kind ?? '').toString(),
            accession: dto.accession ?? dto.Accession,
            instrumentCode: dto.instrumentCode ?? dto.InstrumentCode,
            value: dto.value ?? dto.Value,
            units: dto.units ?? dto.Units,
            flag: dto.flag ?? dto.Flag
        };

        if (normalized.kind === 'ResultPosted') {
            prependLine(resultsUl, lineForResult(normalized));
        } else if (normalized.kind === 'Trace') {
            // Only if you also push trace lines via LabHub; otherwise nothing to do
            prependLine(traceUl, lineForTrace(normalized));
        } else {
            // Other kinds can be shown in trace area
            prependLine(traceUl, lineForTrace(normalized));
        }
    });

    try {
        await connection.start();
    } catch (e) {
        console.error("SignalR connect failed:", e);
        prependLine(resultsUl, `<span class="ts">[—]</span><span class="payload">SignalR connect failed</span>`);
    }
})();
