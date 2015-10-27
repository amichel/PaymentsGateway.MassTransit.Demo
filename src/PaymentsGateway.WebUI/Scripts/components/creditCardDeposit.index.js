import ReactDOM from 'react-dom';
import React from 'react';
import CCDeposit from './payments/creditCardDeposit';
import thunkMiddleware from 'redux-thunk';
import createLogger from 'redux-logger';
import { createStore, applyMiddleware } from 'redux';
import { startPaymentsHub, changePaymentStatus } from '../creditCardActions';
import rootReducer from '../reducers';

const loggerMiddleware = createLogger();

const createStoreWithMiddleware = applyMiddleware(
  thunkMiddleware, // lets us dispatch() functions
  loggerMiddleware // neat middleware that logs actions
)(createStore);

const store = createStoreWithMiddleware(rootReducer);


store.dispatch(startPaymentsHub());

ReactDOM.render(<CCDeposit store={store}/>, document.getElementById('reactRoot'));