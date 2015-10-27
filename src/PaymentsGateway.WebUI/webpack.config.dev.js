'use strict';
var webpack = require('webpack');
var path = require('path');
var nodeModulesPath = path.resolve(__dirname, 'node_modules');
var srcPath = path.resolve(__dirname, 'scripts/components');
var appPath = path.resolve(__dirname, 'scripts/components');

var buildPath = path.resolve(__dirname, 'public', 'build');
var mainPath = path.resolve(appPath, 'creditCardDeposit.index.js');

module.exports = {
	target: 'web',
	context: __dirname,
	devtool: 'eval-cheap-module-source-map',
	output: {
		path: buildPath,
		filename: 'bundle.js',
		publicPath: 'http://localhost:8080/build/'
	},

	debug: true,
	entry: [
		'webpack-dev-server/client?http://localhost:8080',
		'webpack/hot/only-dev-server',
		mainPath
	],
	resolve: {
		root: srcPath,
		extensions: ['', '.js'],
		alias: {
			'styles': 'styles/',
			'components': 'components/',
			'stores': 'stores/',
			'actions': 'actions/'
		},
		modulesDirectories: ['node_modules', 'src']
	},
	module: {
		loaders: [
			{
				test: /\.(js)$/,
				exclude: /node_modules/,
				loaders: ['react-hot', 'babel']
			}, {
				test: /\.css$/,
				loader: 'style!css'
			}
		]
	},

	plugins: [
		new webpack.HotModuleReplacementPlugin(),
		new webpack.NoErrorsPlugin()
	]

};