"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const functions = require("firebase-functions");
const admin = require("firebase-admin");
exports.default = functions.auth.user().onCreate(async (user) => {
    const uid = user.uid;
    const startingCoins = await admin.database().ref('/constants/users/startingCoins')
        .once('value')
        .then(snap => {
        return snap.val();
    });
    await admin.database().ref('/users/' + uid + '/userData')
        .set({
        coinsAmount: startingCoins,
    })
        .then(log => {
        console.log(log + startingCoins);
    });
});
//# sourceMappingURL=onCreate.f.js.map