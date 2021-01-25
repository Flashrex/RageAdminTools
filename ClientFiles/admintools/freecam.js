const player = mp.players.local;
let cam = null;

mp.events.add('AdminTools:Freecam', () => {
    if(!cam) {
        player.freezePosition(true);
        player.setAlpha(0);
        player.setInvincible(true);
    
        let pos = player.position;
        cam = mp.cameras.new("admintools_freecam", pos, new mp.Vector3(0, 0, 0), 90);
        cam.setActive(true);
        mp.game.ui.displayRadar(false);
        mp.game.cam.renderScriptCams(true, false, 0, true, false);
        cam.setAffectsAiming(false);
    }
})

function stopFreeCam() {
    if(cam) {
        player.position = new mp.Vector3(player.position.x, player.position.y, player.position.z - 50);
        player.freezePosition(false);
        player.resetAlpha();
        player.setInvincible(false);
        
        mp.game.cam.renderScriptCams(false, false, 0, true, true);
        cam.setActive(false);
        cam.destroy();
        mp.game.cam.destroyAllCams(true);
        mp.game.ui.displayRadar(true);
        cam = null;
        return;
    }
}

mp.events.add('render', () => {
    if(cam) {
        if(mp.game.ui.isPauseMenuActive() || !mp.system.isFocused) return;

        if(mp.keys.isDown(8) && !mp.gui.cursor.visible) {
            mp.events.callRemote("AdminTools:StopSpectate");
            stopFreeCam();
            return;
        }
        ShowHelpMessage("~INPUT_MOVE_UP_ONLY~ Vorwärts \n~INPUT_SPRINT~ Speedboost \n~INPUT_CELLPHONE_CANCEL~ Abbrechen");
        
        var speed = 0.1;
        //Get Mouse Movement
        var rightAxisX = mp.game.controls.getDisabledControlNormal(0, 220);
        var rightAxisY = mp.game.controls.getDisabledControlNormal(0, 221);

        //Rotate Cam
        const rot = cam.getRot(2);
        cam.setRot(rot.x + rightAxisY *-5.0, 0, rot.z + rightAxisX *-5.0, 2);

        //Shift-Key
        if(mp.keys.isDown(16) && !mp.gui.cursor.visible) speed *= 10;

        if(mp.keys.isDown(87) && !mp.gui.cursor.visible)  {
            const pos = cam.getCoord();
            const dir = cam.getDirection();
            const forward = new mp.Vector3(pos.x + dir.x * speed, pos.y + dir.y * speed, pos.z + dir.z * speed);
            cam.setCoord(forward.x, forward.y, forward.z);
            player.position = new mp.Vector3(forward.x, forward.y, forward.z + 50);
        }
    }
})

function ShowHelpMessage(message) {
    mp.game.ui.setTextComponentFormat('STRING');
    mp.game.ui.addTextComponentSubstringPlayerName(message);
    mp.game.ui.displayHelpTextFromStringLabel(0, false, true, -1);
}