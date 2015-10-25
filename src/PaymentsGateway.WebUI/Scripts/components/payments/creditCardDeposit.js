import React from 'react';

export default React.createClass({
	render:function() {
		return (
			<div className="container-fluid bg-grey">
				<h1>Have some fun by paying us with your: </h1>
				<h2>Credit Card </h2>
			  <form className="form">

				<div className="col-sm-7">
				  <div className="row">
					<div className="col-sm-9 form-group">
						<input type="text" className="form-control" placeholder="Credit Card Number"/>
					</div>
					<div className="col-sm-3 form-group">
						<input type="text" className="form-control" placeholder="CVV"/>
					</div>
					<div className="col-sm-9 form-group">
					  <input type="text" className="form-control" placeholder="Name On The Card"/>
					</div>
					<div className="col-sm-3 form-group">
					  <input type="email" className="form-control" placeholder="Expiration Date"/>
					</div>
				  </div>
				 
				  <div className="row">
					<div className="col-sm-12 form-group">
					  <button className="btn btn-default pull-right" type="submit">Pay</button>
					</div>
				  </div> 
				</div>
			  </form>
		</div>
		);
	}
})