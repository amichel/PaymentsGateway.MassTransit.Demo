
//var accountNumber = 123;
var hub = $.connection.paymentsHub;
//hub.qs = `AccountNumber${accountNumber}`;


export const hubClient = hub.client;
export function startHub (){return $.connection.hub.start();}
export const ccDepositServer = hub.server.ccDeposit;