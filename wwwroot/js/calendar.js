$( function() {
    $("#datepicker").datepicker({
        onSelect: function(dateText) {
            fetchAppointments(dateText);
        }
    });
});

function fetchAppointments(date) {
    var selectedDate = moment(date, "MM/DD/YYYY"); // Parse the selected date with moment.js

    // Create a table
    var html = "<table><thead><tr><th>Time</th><th>Client</th></tr></thead><tbody>";

    // Loop through each 30-minute period from 9 AM to 9 PM
    for (var i = 9; i <= 21; i += 0.5) {
        var time = moment({hour: Math.floor(i), minute: (i % 1) * 60}).format("HH:mm");

        // Find the client for the current time period
        var client = clients.find(function(client) {
            var appointmentDate = moment(client.appointmentDate); // Parse the appointment date with moment.js
            var appointmentTime = moment(client.appointmentTime, "HH:mm").format("HH:mm"); // Parse the appointment time with moment.js

            // Check if the appointment date is the same as the selected date and the appointment time is the same as the current time
            return appointmentDate.isSame(selectedDate, 'day') && appointmentTime === time;
        });

        // Add a row to the table for the current time period
        html += "<tr><td>" + time + "</td><td>" + (client ? client.firstName + " " + client.lastName : "") + "</td></tr>";
    }

    html += "</tbody></table>"; // Close the table

    $("#matchingClients").html(html);
}