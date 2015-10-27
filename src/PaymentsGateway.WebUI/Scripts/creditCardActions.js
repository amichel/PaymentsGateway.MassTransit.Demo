import {startHub, hubClient, ccDepositServer} from './paymentHub';
import {PAYMENT_STATUS} from './constants';
export const CHANGE_PAYMENT_STATUS = 'CHANGE_PAYMENT_STATUS';
export const PAYMENTS_HUB_READY = 'PAYMENTS_HUB_READY';


export function startPaymentsHub() {
	return function(dispatch) {
		hubClient.onDepositResponse = function(response) {
			console.log("received hub response:", response);
			dispatch(changePaymentStatus(response));
		};

		return startHub().done(() => {
			dispatch(paymentsHubReady());
		});
	}
}

export function startPaymentProcessing(paymentData) {
	return function(dispatch) {
		dispatch(PAYMENT_STATUS.started);

		return ccDepositServer(paymentData);
	}
}

export function changePaymentStatus(status) {
	return {type:CHANGE_PAYMENT_STATUS,status}
}

export function paymentsHubReady() {
	return {
		type: PAYMENTS_HUB_READY
	};
}
