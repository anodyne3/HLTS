"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const functions = require("firebase-functions");
const admin = require("firebase-admin");
exports.default = functions.https.onCall(async (data, context) => {
    var _a;
    const uid = (_a = context.auth) === null || _a === void 0 ? void 0 : _a.uid;
    const currentCoinsAmountRef = admin.database().ref('/users/' + uid + '/userData/coinsAmount');
    const currentCoinsAmount = await currentCoinsAmountRef.once('value').then(snap => {
        return snap.val();
    });
    await currentCoinsAmountRef.set(currentCoinsAmount - 1);
    const nextResultRef = admin.database().ref('/users/' + uid + '/userData/nextResult');
    const lastResultRef = admin.database().ref('/users/' + uid + '/userData/lastResult');
    await nextResultRef.once('value').then(async (snap) => {
        await lastResultRef.set({
            0: snap.child('0').val(),
            1: snap.child('1').val(),
            2: snap.child('2').val()
        });
    });
    await nextResultRef.set({
        0: generateRandomResult(),
        1: generateRandomResult(),
        2: generateRandomResult()
    });
    function generateRandomResult() {
        return Math.floor(Math.random() * (24 - 12 + 1)) - 12;
    }
});
//# sourceMappingURL=onCall.f.js.map