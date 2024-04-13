var controller = $(".controller-marker").data("controller");

//function getHallId(selectedHallName) {
//    $.ajax({
//        url: '/'+controller+'/GetHallId',
//        type: 'GET',
//        data: { hallName: selectedHallName },
//        success: function (data) {
//            $('#HallId').val(data.hallId);
//        },
//        error: function () {
//            console.log('Error occurred during AJAX request');
//        }
//    });
//}

// Function to get Guest Limit
function getGuestLimit(hallId) {
    $.ajax({
        url: '/' + controller + '/GetGuestLimit',
        type: 'GET',
        data: { hallId: hallId },
        success: function (data) {
            $('#GuestLimit').val(data.guestLimit);
        },
        error: function () {
            console.log('Error occurred during AJAX request');
        }
    });
}

$('#hallNameDropdown').change(function () {
    var hallId = $(this).val();

   // getHallId(selectedHallName);
    getGuestLimit(hallId);
    $('#NumberOfGuests').val('');
});

$('#NumberOfGuests').on("input", function () {
    var numberOfGuests = parseInt($(this).val());
    var guestLimit = parseInt($('#GuestLimit').val());

    if (numberOfGuests > guestLimit) {
        alert('Number of guests exceeds the hall limit of '+ guestLimit);
        $('#NumberOfGuests').val('');
    }
});
function getAvailableHalls(selectedDate) {
    $.ajax({
        url: '/'+controller+'/GetAvailableHalls',
        type: 'GET',
        data: { date: selectedDate },
        success: function (data) {
            $('#hallNameDropdown').empty();
            $("#hallNameDropdown").append($('<option>', {
                value: '',
                text: 'Select Hall'
            }));
            $.each(data.availableHalls, function (index, hall) {
                var option = new Option(hall.hallName + ' - ' + hall.location + ', ' + hall.venue, hall.hallId);
                $("#hallNameDropdown").append(option);
            });
            $.each(data.unavailableHalls, function (index, hall) {
                var optionText = hall.hallName + ' - ' + hall.location + ', ' + hall.venue + ' (Booked)';
                var option = new Option(optionText, hall.hallId);
                option.disabled = true;
                option.style.color = 'red';
                $("#hallNameDropdown").append(option);
            });


        },
        error: function () {
            console.log('Error occurred during AJAX request');
        }
    });
}
$('#Date').change(function () {
    console.log('Date Changed');
    var selectedDate = $(this).val();
    $('#hallNameDropdown').val('');
    getAvailableHalls(selectedDate);
});
