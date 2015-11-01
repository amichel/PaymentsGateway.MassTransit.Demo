import React from 'react';
import { connect } from 'react-redux';
import {Button, Input, DropdownButton, MenuItem, Glyphicon} from 'react-bootstrap';
import {startPaymentProcessing} from '../../creditCardActions';

const CreditCardDeposit = React.createClass({
	startPayment: function() {
		const paymentData = {
			ccNumber: this.refs.ccNumber.value,
			cvv: this.refs.cvv.value,
			cardHolderName: this.refs.cardHolderName.value,
			expirationDate: this.refs.expirationDate.value
		}
		this.props.dispatch(startPaymentProcessing(paymentData));
	},
	render: function() {
				console.log("rendering for state", this.props.store.getState());

		return (
			<div>
				<h1>Have some fun by paying us with your: </h1>
					<div className="container-fluid row bg-orange bg-trasparent">
					  <h2 className="col-md-3">Credit Card</h2>

					  <div className="col-md-9">
						<div className="row">
						  <div className="col-md-9 form-group">
							  <Input type="text" ref="ccNumber" className="form-control" placeholder="Credit Card Number"/>
						  </div>
						  <div className="col-md-3 form-group">
							  <Input type="text" ref="cvv" className="form-control" placeholder="CVV"/>
						  </div>
						  <div className="col-md-9 form-group">
							<Input type="text" ref="cardHolderName" className="form-control" placeholder="Name On The Card"/>
						  </div>
						  <div className="col-md-3 form-group">
							<Input type="email" ref="expirationDate" className="form-control" placeholder="Expiration Date"/>
						  </div>
						</div>

						<div className="row">
						  <div className="col-md-5 form-group">
								<Input type="number" ref="amount" className="form-control" placeholder="Amount"/>
							</div>

						  <div className="col-md-5 form-group">
						  <DropdownButton ref="currency" className="col-md-5 form-control">
			  	  				<MenuItem eventKey="1">EUR</MenuItem>
								<MenuItem eventKey="2">USD</MenuItem>
			  					<MenuItem eventKey="3" active>GBP</MenuItem>
			  					<MenuItem eventKey="4">JPY</MenuItem>
						</DropdownButton>
						 </div>

						  <div className="col-md-12 form-group">
							<Button bsStyle={this.props.paymentsHubReady ? "danger" : "default"} className="pull-right" onClick={this.startPayment}>Pay</Button>
						  </div>
						</div>
					  </div>
					</div>
			  </div>
		);
	}
});

function mapStateToProps(state) {
	return {paymentStatus: state.paymentStatus, paymentsHubReady:state.paymentsHubReady } ;;
}

export default connect(mapStateToProps)(CreditCardDeposit);
