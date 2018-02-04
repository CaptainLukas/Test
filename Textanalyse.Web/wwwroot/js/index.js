$(document).ready(function () {

    $('#searchForm').submit(function (event) {

        // prevent form from submitting
        event.preventDefault();
    });

    // do ajax call on submit-btn click
    $('#searchButton').click(function () {

        //var data = `words=${$('#inputSearch').text()}`;
        var data = $('#searchText').val();

        $.ajax({
            type: 'GET',
            url: '/search', // url that is defined within the controller 
            data: {
                words: data
            },
            contentType: "application/json; charset=utf-8",
            dataType: 'json',
            success: function (result) {

                // if there is a message, display it
                if (result.message !== null && result.message !== undefined && result.message !== "") {
                    $('#searchMsgBox').text(result.message.toString());
                    $('#searchMsgBox').show();
                }
                else {
                    // temporarily show the result in a box
                    $('#jsonBox').text(result.json);
                    $('#jsonBox').show();
                }
            },
            error: function () {
                console.log("Error while sending AJAX request to path \"search\".");
            }
        });
    });
});