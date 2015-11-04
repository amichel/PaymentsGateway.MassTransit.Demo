 var PaymentsDal = function (callback,monitoringCb) {
        var connection = $.connection;
        var hub = connection.paymentsHub;
        var monitoringCallback = monitoringCb||function() {};
        connection.hub.disconnected(function() {
            setTimeout(function() {
                connection.hub.start();
            }, 1); // Restart connection after 5 seconds.
        });

        function switchAccount(accountNumber) {
            connection.hub.stop();
            connection.hub.qs = { AccountNumber: accountNumber };
            connection.hub.start();
        }

        // Create a callback function that the hub can call to send response from bus consumer
        hub.client.onDepositResponse = function (response) {
            console.trace("push notif response", response);
            callback(response);
        };
        hub.client.onMonitoringEvent = monitoringCallback;

        // Start the connection.
        var starting = connection.hub.start();
        return {
            starting: starting,
            payWithCreditCard: function (ccInfo) {
                var result = hub.server.ccDeposit({ AccountNumber: ccInfo.accountNumber, CardId: ccInfo.cardId, Amount: ccInfo.amount, Currency: ccInfo.currency });
                return result;
            },
            switchAccount: switchAccount
        };
    };