$(document).ready(function () {
    $("#CityId").change(function () {
        $("#StreetId").empty();
        $.ajax({
            type: 'POST',
            url: addressGetStreetPath,
            dataType: 'json',
            data: { cityId: $("#CityId").val() },
            success: function (streets) {
                $.each(streets, function (i, street) {
                    $("#StreetId").append('<option value="' + street.Value + '">' +
                     street.Text + '</option>');
                });
            },
            error: function (ex) {
                alert('Failed to retrieve states.' + ex);
            }
        });
        return false;
    })
});