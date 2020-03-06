var loadFunctions = require('firebase-function-tools');
var admin = require('firebase-admin');
var functions = require('firebase-functions');
var config = functions.config().firebase;
admin.initializeApp(config);
loadFunctions(__dirname, exports);