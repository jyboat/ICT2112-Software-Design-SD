document.addEventListener("DOMContentLoaded", function () {
    let table = document.getElementById("auditTable").getElementsByTagName("tbody")[0];
    let searchInput = document.getElementById("searchInput");
    let pagination = document.getElementById("pagination");
    let rows = Array.from(table.rows);
    let rowsPerPage = 10;
    let currentPage = 1;

    // ✅ SignalR connection setup for Audit Logs
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/auditLogHub")  // The URL of the SignalR hub
        .configureLogging(signalR.LogLevel.Information)
        .build();

    // ✅ Start the SignalR connection
    connection.start().then(() => {
        console.log("✅ Connected to SignalR hub");
    }).catch(err => console.error("❌ SignalR Connection Error:", err));

    // ✅ Listen for updates from the server
    connection.on("ReceiveAuditLogUpdate", () => {
        console.log("🔄 New audit log detected! Refreshing table...");
        location.reload(); 
    });

    // ✅ Function to render the table
    function renderTable() {
        let filteredRows = rows.filter(row => {
            let searchText = searchInput.value.toLowerCase();
            return Array.from(row.cells).some(cell => cell.textContent.toLowerCase().includes(searchText));
        });

        let totalPages = Math.ceil(filteredRows.length / rowsPerPage);
        let start = (currentPage - 1) * rowsPerPage;
        let end = start + rowsPerPage;

        table.innerHTML = ""; // Clear current rows
        filteredRows.slice(start, end).forEach(row => table.appendChild(row)); // Append filtered rows

        renderPagination(totalPages); // Render pagination controls
    }

    // ✅ Function to render pagination
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
                renderTable(); // Re-render table when page is clicked
            });
            li.appendChild(a);
            pagination.appendChild(li);
        }
    }

    // ✅ Search input event listener
    searchInput.addEventListener("input", function () {
        currentPage = 1; // Reset to first page on search
        renderTable();
    });

    // ✅ Initial render of the table
    renderTable();
});
