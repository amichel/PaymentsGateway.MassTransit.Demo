import React from 'react';
import {Button, Input} from 'react-bootstrap';

export default React.createClass({
	render:function() {
		return (
			<div className="">
				<h1>Have some fun by paying us with your: </h1>
			  <div className="container-fluid row bg-orange bg-trasparent">
				<h2 className="col-md-3">Credit Card</h2>

				<div className="col-md-9">
				  <div className="row">
					<div className="col-md-9 form-group">
						<Input type="text" className="form-control" placeholder="Credit Card Number"/>
					</div>
					<div className="col-md-3 form-group">
						<Input type="text" className="form-control" placeholder="CVV"/>
					</div>
					<div className="col-md-9 form-group">
					  <Input type="text" className="form-control" placeholder="Name On The Card"/>
					</div>
					<div className="col-md-3 form-group">
					  <Input type="email" className="form-control" placeholder="Expiration Date"/>
					</div>
				  </div>
				 
				  <div className="row">
					<div className="col-md-12 form-group">
					  <Button bsStyle="danger" className="pull-right" type="submit">Pay</Button>
					</div>
				  </div> 
				</div>
			  </div>
		</div>
		);
	}
})