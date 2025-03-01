document.addEventListener("DOMContentLoaded", function () {
    let table = document.getElementById("recordsTable").getElementsByTagName("tbody")[0];
    let searchInput = document.getElementById("searchInput");
    let pagination = document.getElementById("pagination");
    let rows = Array.from(table.rows);
    let rowsPerPage = 10;
    let currentPage = 1;

    // SignalR connection setup
    const connection = new signalR.HubConnectionBuilder()
    .withUrl("/medicalRecordHub")  // The URL to your SignalR hub
    .configureLogging(signalR.LogLevel.Information)
    .build();

    // Start the connection
    connection.start().then(() => {
        console.log("✅ Connected to SignalR hub");
    }).catch(err => console.error("❌ SignalR Connection Error:", err));

    // Listen for updates from the server
    connection.on("ReceiveMedicalRecordUpdate", () => {
        console.log("🔄 New medical record detected! Refreshing page...");
        location.reload(); // Refresh the page when an update is detected
    });

    function renderTable() {
        let filteredRows = rows.filter(row => {
            let searchText = searchInput.value.toLowerCase();
            return Array.from(row.cells).some(cell => cell.textContent.toLowerCase().includes(searchText));
        });

        let totalPages = Math.ceil(filteredRows.length / rowsPerPage);
        let start = (currentPage - 1) * rowsPerPage;
        let end = start + rowsPerPage;

        table.innerHTML = "";
        filteredRows.slice(start, end).forEach(row => table.appendChild(row));

        renderPagination(totalPages);
    }

    function renderPagination(totalPages) {
        pagination.innerHTML = "";
        for (let i = 1; i <= totalPages; i++) {
            let li = document.createElement("li");
            li.className = "page-item " + (i === currentPage ? "active" : "");
            let a = document.createElement("a");
            a.className = "page-link";
            a.textContent = i;
            a.href = "#";
            a.addEventListener("click", function (e) {
                e.preventDefault();
                currentPage = i;
                renderTable();
            });
            li.appendChild(a);
            pagination.appendChild(li);
        }
    }

    searchInput.addEventListener("input", function () {
        currentPage = 1;
        renderTable();
    });

    function checkForUpdates() {
        fetch('/ViewRecord/CheckForMedicalRecordUpdates')
            .then(response => response.json())
            .then(data => {
                if (data.update) {
                    console.log("🔄 New medical record detected! Refreshing page...");
                    location.reload();
                }
            })
            .catch(error => console.error("❌ Error checking for updates:", error));
    }
    
    // Run check every 5 seconds
    setInterval(checkForUpdates, 5000);    

    renderTable();
});
