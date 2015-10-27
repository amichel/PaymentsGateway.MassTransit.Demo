import {startPayment, PAYMENTS_HUB_READY, CHANGE_PAYMENT_STATUS} from './creditCardActions';
import {PAYMENT_STATUS} from './constants';

var initialState = {
	paymentStatus: PAYMENT_STATUS.none,
	paymentsHubReady: false
};

export default function rootReducer(state=initialState, action) {
	switch(action.type) {
		case CHANGE_PAYMENT_STATUS:
			return Object.assign({}, state, {
				paymentStatus: action.status
			});
		case PAYMENTS_HUB_READY:
			return Object.assign({}, state, {
				paymentsHubReady: true
			});
		default :
			return state;
	}
}
