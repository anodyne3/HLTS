import * as functions from "firebase-functions";
import * as admin from "firebase-admin";
import {ref} from "firebase-functions/lib/providers/database";

export const createNewUserDatabaseNode = functions.auth.user().onCreate(async (user) => {
    const uid = user.uid;
    const startingCoins = await admin.database().ref('/constants/constants/startingCoins')
        .once('value')
        .then(snap => {
            console.log(snap.ref, snap.val());
            return snap.val();
        });

    await admin.database().ref('/users/' + uid + '/userData')
        .set({
            coinsAmount: startingCoins,
        })
        .then(log => {
            console.log(startingCoins, ref)
        })
});