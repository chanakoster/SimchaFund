$(function () {
    $("#new-contributor").on("click", function () {
        new bootstrap.Modal($(".new-contrib")[0]).show();

    });

    $(".deposit-button").on("click", function () {
        const contribId = $(this).data("contribid");
        const tr = $(this).closest("tr");
        const name = tr.find("td:eq(1)").text();

        $('.deposit [name="contributorId"]').val(contribId);
        $("#deposit-name").text(name);

        new bootstrap.Modal($(".deposit")[0]).show();
    });

    $("#search").on("keyup", function () {
        const text = $(this).val().toLowerCase();
        $("table tbody tr").each(function () {
            const row = $(this);
            const name = row.find("td:eq(1)").text().toLowerCase();
            row.toggle(name.indexOf(text) >= 0);
        });
    });

    $("#clear").on("click", function () {
        $("#search").val("");
        $("table tbody tr").show();
    });

    $(".edit-contrib").on("click", function () {
        const id = $(this).data("id");
        const firstName = $(this).data("firstName");
        const lastName = $(this).data("lastName");
        const cell = $(this).data("cell");
        const alwaysInclude = $(this).data("alwaysInclude");
        const date = $(this).data("date");


        $("#edit_contributor-id").val(id);
        $("#edit_contributor_first_name").val(firstName);
        $("#edit_contributor_last_name").val(lastName);
        $("#edit_contributor_cell_number").val(cell);
        $("#edit_contributor_created_at").val(date);
        $("#edit_contributor_always_include").prop("checked", alwaysInclude === 'True');

        new bootstrap.Modal($(".contrib-edit")[0]).show();
    });
});
