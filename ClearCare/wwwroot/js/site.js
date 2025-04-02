// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function initializeTableSearch(searchInputId, searchButtonId, tableId) {
    $(document).ready(function() {
        const $searchInput = $(`#${searchInputId}`);
        const $searchButton = $(`#${searchButtonId}`);
        const $table = $(`#${tableId}`);

        function applySearch() {
            const searchText = $searchInput.val().toLowerCase();
            $table.find('tbody tr').each(function() {
                const rowText = $(this).text().toLowerCase();
                $(this).toggle(rowText.includes(searchText));
            });
        }

        $searchButton.click(applySearch);
        $searchInput.keypress(function(e) {
            if (e.which === 13) applySearch();
        });
    });
}
