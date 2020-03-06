import * as functions from 'firebase-functions';
import * as admin from "firebase-admin";

export default functions.https.onCall(async (data, context) => {
    const uid = context.auth?.uid;

    const currentCoinsAmountRef = admin.database().ref('/users/' + uid + '/userData/coinsAmount');
    const currentCoinsAmount = await currentCoinsAmountRef.once('value').then(snap => {
        return snap.val()
    });

    await currentCoinsAmountRef.set(currentCoinsAmount - 1);

    const nextResultRef = admin.database().ref('/users/' + uid + '/userData/nextResult');
    const lastResultRef = admin.database().ref('/users/' + uid + '/userData/lastResult');

    await nextResultRef.once('value').then(async snap => {
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