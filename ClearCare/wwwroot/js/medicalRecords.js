document.addEventListener("DOMContentLoaded", function () {
    let table = document.getElementById("recordsTable").getElementsByTagName("tbody")[0];
    let searchInput = document.getElementById("searchInput");
    let pagination = document.getElementById("pagination");
    let rows = Array.from(table.rows);
    let rowsPerPage = 10;
    let currentPage = 1;

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

    renderTable();
});
