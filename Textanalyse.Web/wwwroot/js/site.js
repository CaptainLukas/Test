// Write your JavaScript code.
$(document).ready(function () {
    $('#DivSearch').submit(function (event) {
        // prevent form from submitting
        event.preventDefault();

        // do ajax call on submit-btn click
        $('#ButtonInput').click(function () {
            $.ajax({
                type: 'GET',
                url: '/search', // url that is defined within the controller 
                data: {},
                dataType: 'json',
                success: function (result) {
                    alert("success");
                },
                error: function () {
                    alert('error');
                }
            });
        });
    });
});