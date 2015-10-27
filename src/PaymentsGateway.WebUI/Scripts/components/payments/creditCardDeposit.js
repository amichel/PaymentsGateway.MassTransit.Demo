import React from 'react';
import {Button, Input} from 'react-bootstrap';
import {startPaymentProcessing} from '../../creditCardActions';

export default React.createClass({
	startPayment:function() {
		const paymentData = {
			ccNumber: this.refs.ccNumber,
			cvv:this.refs.cvv,
			cardHolderName:this.refs.cardHolderName,
			expirationDate:this.refs.expirationDate
		}
		this.props.store.dispatch(startPaymentProcessing(paymentData));
	},
	render:function() {
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
						  <div className="col-md-12 form-group">
							<Button bsStyle="danger" className="pull-right" onClick={this.startPayment}>Pay</Button>
						  </div>
						</div> 
					  </div>
					</div>
			  </div>
		);
	}
})