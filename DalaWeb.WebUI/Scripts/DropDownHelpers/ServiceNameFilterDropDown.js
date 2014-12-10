$(function () {
    $("#ServiceName").change(function () {
        var selected = $(this).val();
        var tableSelector = "table";
        var $table = $(tableSelector);
        if (selected != "Все") {
            $table.find("tbody").find("tr").hide().filter(function () { return $.trim($(this).find("td:nth-child(4)").text()) == selected; }).show();
        }
        else {
            $table.find("tbody").find("tr").show();
        }
    })
});
