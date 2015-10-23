﻿'use strict';
var webpack = require('webpack');
var path = require('path');
var nodeModulesPath = path.resolve(__dirname, 'node_modules');
var srcPath = path.resolve(__dirname, 'scripts/components');
var appPath = path.resolve(__dirname, 'scripts/components');

var buildPath = path.resolve(__dirname, 'public', 'build');
var mainPath = path.resolve(appPath, 'components/payments/creditCardDeposit.js');

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
         'webpack-hot-middleware/client',
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
       
        loaders: [{
            test: /\.(js)$/,
            exclude: /node_modules/,
            loader: 'babel'
        },  {
            test: /\.css$/,
            loader: 'style!css'
        }]
    },

    plugins: [
        new webpack.ProvidePlugin({
            'fetch': 'exports?global.fetch!whatwg-fetch'
        }),
        new webpack.HotModuleReplacementPlugin(),
        new webpack.NoErrorsPlugin()
    ]

};