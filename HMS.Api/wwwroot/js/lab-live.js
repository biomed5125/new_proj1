(async () => {
    // include vendor script: <script src="/lib/signalr/signalr.js"></script>
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/hubs/lab")
        .withAutomaticReconnect()
        .build();

    // optional: subscribe only to one device (uncomment & set an ID)
    // connection.start().then(() => connection.invoke("JoinDeviceGroup", "dev:2025900000001"));

    function paintFlag(flag) {
        if (!flag) return "";
        const f = flag.toUpperCase();
        const span = document.createElement("span");
        span.textContent = ` ${f}`;
        span.style.fontWeight = "600";
        if (f === "H" || f === "HIGH" || f === "CRIT") span.style.color = "#c62828";
        else if (f === "L" || f === "LOW") span.style.color = "#1565c0";
        else span.style.color = "#2e7d32";
        return span.outerHTML;
    }

    function prependLine(ulId, text, htmlSuffix = "") {
        const ul = document.getElementById(ulId);
        if (!ul) return;
        const nearTop = ul.scrollTop <= 5;
        const li = document.createElement("li");
        li.innerHTML = text + htmlSuffix;
        ul.prepend(li);
        if (nearTop) ul.scrollTop = 0; // autoscroll if user is at top (newest-first list)
    }

    connection.on("TraceEvent", dto => {
        const ts = new Date(dto.at).toLocaleTimeString();
        if (dto.kind === "Trace") {
            // raw ASTM lines summary
            const text = `[${ts}] dev=${dto.deviceId} ${dto.accession ?? ""} ${dto.instrumentCode ?? ""} = ${dto.value ?? ""} ${dto.units ?? ""}`;
            prependLine("traceLog", text);
        } else if (dto.kind === "ResultPosted") {
            const text = `[${ts}] ${dto.accession ?? ""} ${dto.instrumentCode ?? ""} = ${dto.value ?? ""} ${dto.units ?? ""}`;
            prependLine("liveResultsLog", text, paintFlag(dto.flag));
        }
    });

    try { await connection.start(); }
    catch (e) { console.error("SignalR connect failed:", e); }
})();
