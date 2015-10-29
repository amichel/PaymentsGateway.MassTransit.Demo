import ReactDOM from 'react-dom';
import React from 'react';
import CreditCardDeposit from './payments/creditCardDeposit';
import thunkMiddleware from 'redux-thunk';
import createLogger from 'redux-logger';
import { createStore, applyMiddleware } from 'redux';
import { Provider } from 'react-redux';
import { startPaymentsHub, changePaymentStatus } from '../creditCardActions';
import rootReducer from '../reducers';

const loggerMiddleware = createLogger();

const createStoreWithMiddleware = applyMiddleware(
  thunkMiddleware, // lets us dispatch() functions
  loggerMiddleware // neat middleware that logs actions
)(createStore);

const store = createStoreWithMiddleware(rootReducer);

store.subscribe(() => {
	console.trace("store changed for state:", store.getState());
});

ReactDOM.render(
		<Provider store={store}>
			<CreditCardDeposit store={store}/>
		</Provider>, document.getElementById('reactRoot')
);
store.dispatch(startPaymentsHub());
