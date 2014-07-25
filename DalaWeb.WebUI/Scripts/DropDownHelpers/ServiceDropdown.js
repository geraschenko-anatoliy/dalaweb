$(document).ready(function () {
    $("#CompanyId").change(function () {
        $("#ServiceId").empty();
        $.ajax({
            type: 'POST',
            url: addressGetServicesPath,
            dataType: 'json',
            data: { companyId: $("#CompanyId").val() },
            success: function (services) {
                $.each(services, function (i, company) {
                    $("#ServiceId").append('<option value="' + company.Value + '">' +
                     company.Text + '</option>');
                });
            },
            error: function (ex) {
                alert('Failed to retrieve states.' + ex);
            }
        });
        return false;
    })
});