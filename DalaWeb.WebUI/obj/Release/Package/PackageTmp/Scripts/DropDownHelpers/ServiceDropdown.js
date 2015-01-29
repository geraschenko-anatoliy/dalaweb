$(document).ready(function () {
    $("#ServiceCompanyId").change(function () {
        $("#Service_ServiceId").empty();
        $.ajax({
            type: 'POST',
            url: addressGetServicesPath,
            dataType: 'json',
            data: { companyId: $("#ServiceCompanyId").val(), abonentId: $("#Abonent_AbonentId").val() },
            success: function (services) {
                $.each(services, function (i, company) {
                    $("#Service_ServiceId").append('<option value="' + company.Value + '">' +
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