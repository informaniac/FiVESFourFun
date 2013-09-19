/**
 * Created with JetBrains WebStorm.
 * Author: Torsten Spieldenner
 * Date: 9/18/13
 * Time: 12:40 PM
 * (c) DFKI 2013
 * http://www.dfki.de
 */

var FIVES = FIVES || {};
FIVES.Creation = FIVES.Creation || {};

(function () {
    "use strict";

    var EntityCreator = function () {
    };

    var ec = EntityCreator.prototype;

    ec.createEntityFromForm = function() {
        var position = _retrievePosition();
        FIVES.Communication.FivesCommunicator.createEntityAt(position.x, position.y, position.z);
    };

    var _retrievePosition = function () {
        var posX = _getValidFloatFieldValue("input-position-x");
        var posY = _getValidFloatFieldValue("input-position-y");
        var posZ = _getValidFloatFieldValue("input-position-z");

        return {x: posX, y: posY, z: posZ};
    }

    var _getValidFloatFieldValue = function (fieldName) {
        var field = $("#" + fieldName);
        if (!field) {
            console.error("[ERROR] EntityCreator._getValidFloatFieldValue: Could not access field with fieldname " + fieldName);
            return;
        }
        var value = field.val();
        if (!value) {
            console.warn("[WARNING] EntityCreator._getValidFloatFieldValue: No value specified for " + fieldName + ", will use defaultvalue instead");
            value = 0;
        }
        return value;
    };

    FIVES.Creation.EntityCreator = new EntityCreator();
}());