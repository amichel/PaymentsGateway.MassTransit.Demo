/*
 * Webpack distribution configuration
 *
 * This file is set up for serving the distribution version. It will be compiled to dist/ by default
 */

'use strict';

var webpack = require('webpack');
var path = require('path');
var nodeModulesPath = path.resolve(__dirname, 'node_modules');
var srcPath = path.resolve(__dirname, 'scripts/components');
var appPath = path.resolve(__dirname, 'scripts/components');

var buildPath = path.resolve(__dirname, 'public', 'build');
var mainPath = path.resolve(appPath, 'components/payments/creditCardDeposit.js');

module.exports = {

  output: {
    publicPath: '/scripts/components/',
    path: buildPath,
    filename: 'index.js'
  },

  debug: false,
  devtool: false,
  entry: mainPath,

  stats: {
    colors: true,
    reasons: false
  },

  plugins: [
    new webpack.optimize.DedupePlugin(),
    new webpack.optimize.UglifyJsPlugin(),
    new webpack.optimize.OccurenceOrderPlugin(),
    new webpack.optimize.AggressiveMergingPlugin()
  ],

  resolve: {
	root: srcPath,
    extensions: ['', '.js', '.jsx'],
    alias: {
            'styles': 'styles/',
            'components': 'components/',
            'stores': 'stores/',
            'actions': 'actions/'
    },
	modulesDirectories: ['node_modules', 'src']
  },

  module: {

    loaders: [{
      test: /\.(js|jsx)$/,
      exclude: /node_modules/,
      loader: 'babel-loader'
    }]
  }
};
