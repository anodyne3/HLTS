const loadFunctions = require('firebase-function-tools');
const admin = require('firebase-admin');
const functions = require('firebase-functions');
const config = functions.config().firebase;

admin.initializeApp(config);

loadFunctions(__dirname, exports);