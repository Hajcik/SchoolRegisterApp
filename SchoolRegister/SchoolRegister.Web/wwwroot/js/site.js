// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$("#subjects-filter-input").change(function () {
    const filterValue = $("#subjects-filter-input").val();
    $.get("/Subject/Index", $.param({
        filterValue: filterValue
    }), function (resultData) {
        $(".subjects-table-data").html(resultData);
    });
});

$("#students-filter-input").change(function () {
    var filterValue = $("#students-filter-input").val();
    $.get("/Student/Index", $.param({
        filterValue: filterValue
    }), function (resultData) {
        $(".students-table-data").html(resultData);
    });
});
//$("#students-parent-filter-input").change(function () {
//    var filterValue = $("#students-parent-filter-input").val();
//    $.get("/Student/Index", $.param({
//        filterValue: filterValue
//    }), function (resultData) {
//        $(".students-parent-table-data").html(resultData);
//    });
//})