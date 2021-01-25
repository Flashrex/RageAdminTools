/*
*   Code von Flashrex
*   Discord: Flashrex#6783
*/

const player = mp.players.local;
let specCam = null;
let target = null;

//Distanz zwischen Kamera und Target
const distance = 2.0;

//Kamerahöhe relativ zum Target
const height = 1.0;

/*
*   Call dieses Event um Specate zu aktivieren
*   Damit es funktioniert muss der Spieler in die Nähe des Targets teleportiert werden.
*   Serverside Beispiel: player.Position = target.Position.Add(new Vector3(0, 0, 50));
*   Damit Spieler auch für andere nicht sichtbar ist muss SharedData gesetzt werden.
*   Dazu siehe DataHandler weiter unten.
*   targetId : number (RemoteId (player.Id) vom Target)
*/
mp.events.add('AdminTools:Spectate', (targetId) => {
    //Deaktiviert aktuelles Spectate falls vorhanden
    if(specCam) stopSpectate();

    //Position freezen, Unsichtbar machen, Unsterblich machen
    player.freezePosition(true);
    player.setAlpha(0);
    player.setInvincible(true);

    //Radar deaktivieren
    mp.game.ui.displayRadar(false);

    //Checkt ob der Target für den Client geladen wurde
    var interval = setInterval(() => {
        mp.players.forEach(p => { if(p.remoteId === targetId) target = p; });
        if(target) {
            //Falls target in Range ist
            const pos = target.position;
            const forward = target.getForwardVector();

            //Kamera erstellen
            specCam = mp.cameras.new("admintools_spectate", new mp.Vector3(pos.x - forward.x * distance,pos.y - forward.y * distance, pos.z +height), new mp.Vector3(0, 0, 0), 90);

            //Kamera auf Target pointen
            specCam.pointAt(target.handle, 0, 0, 0, false);

            //Kamera aktivieren
            specCam.setActive(true);
            mp.game.cam.renderScriptCams(true, false, 0, true, false);

            clearInterval(interval);
        }
    }, 10);

    //Deaktiviert nach 5 Sekunden den Interval, wenn kein Target gefunden wurde
    setTimeout(() => {
        if(interval) clearInterval(interval);
    }, 5000);
})

/*
*   Deaktiviert den Spectate Modus.
*/
function stopSpectate() {
    if(specCam) {

        //Spieler unfreezen, Sichtbar machen, Sterblich machen
        player.freezePosition(false);
        player.resetAlpha();
        player.setInvincible(false);

        //Kamera deaktivieren, löschen und Radar anzeigen
        mp.game.cam.renderScriptCams(false, false, 0, true, true);
        specCam.setActive(false);
        specCam.destroy();
        mp.game.cam.destroyAllCams(true);
        mp.game.ui.displayRadar(true);
        
        specCam = null;
        return;
    }
}

/*
*   Updated die Kamera Position
*/
function updateCamPosition() {
    if(!specCam || target === null) return;

    const pos = target.position;
    const forward = target.getForwardVector();

    specCam.setCoord(pos.x - forward.x * distance, pos.y - forward.y * distance, pos.z + height);
}

/*
*   Kümmert sich um alles im Zusammenhang mit der Kamera
*/
mp.events.add('render', () => {
    if(specCam) {
        //Wenn auf Desktop oder im Escape Menu passiert nichts
        if(mp.game.ui.isPauseMenuActive() || !mp.system.isFocused) return;

        //Keycode 8 -> Backspace
        //Deaktiviert Spectate Modus
        if(mp.keys.isDown(8) && !mp.gui.cursor.visible) {

            //Hier sollte ShareData für Unsichtbarkeit gelöscht und Position resettet werden
            mp.events.callRemote("AdminTools:StopSpectate");

            //Stoppt Spectate
            stopSpectate();
            return;
        }

        //Zeigt eine Hilfenachricht oben links
        ShowHelpMessage("~INPUT_CELLPHONE_CANCEL~ Abbrechen");

        //Updated Cam Position
        updateCamPosition();
        
        //Setzt Playser Position über den Target
        if(target) player.position = new mp.Vector3(target.position.x, target.position.y, target.position.z + 50);
    }
})

/*
*   Zeigt eine Hilfenachricht oben links
*/
function ShowHelpMessage(message) {
    mp.game.ui.setTextComponentFormat('STRING');
    mp.game.ui.addTextComponentSubstringPlayerName(message);
    mp.game.ui.displayHelpTextFromStringLabel(0, false, true, -1);
}

/*
*   Sorgt dafür, dass Spieler unsichtbar sind.
*   Dazu einfach dem Spieler der unsichtbar sein soll serverseitig SharedData setzen:
*       player.SetSharedData("AdminTools:IsInvisible", true);
*   Zum deaktivieren einfach SharedData resetten.
*/
mp.events.addDataHandler("AdminTools:IsInvisible", (entity, value, oldvalue) => {
    if(entity.id === mp.players.local.id) return;

    if(value) entity.setAlpha(0);
    else entity.resetAlpha();
})

/*
*   Same wie DataHandler
*/
mp.events.add('entityStreamIn', (entity) => {
    if(entity.getVariable('AdminTools:IsInvisible') === true) entity.setAlpha(0);
    else entity.resetAlpha();
});