var gnX = 0;
var gnY = 0;
var giUnitSelected = -1;
var gnMoveOffset = null;
var giUnitStartX = -1;
var giUnitStartY = -1;
var giTurnPhase = 0; // 0=allied movement, 1=allied attack, 2=enemy movement, 3=enemy attack
var GermanEventsTimer = null;
var GermanEvents = Array();
var giGermanUnitUnderAttack = -1;
var ViewMap = Array();
var TestMode = false; // Set this to true to show all pieces for testing
var MaxX = -1;
var MaxY = -1;

$(document).ready(function () {
	MaxX = $('#MaxX').val();
	MaxY = $('#MaxY').val();
	TestMode = ($('#TestMode').val() == '1');

	// initialize the ViewMap array
	for (var y = 0; y < MaxY; y++) {
		for (var x = 0; x < MaxX; x++) {
			ViewMap[x] = Array();
			ViewMap[x][y] = false;
		}
	}

	RecomputeMapView();

	var IE = document.all ? true : false;
	if (!IE) {
		document.captureEvents(Event.MOUSEMOVE);
	}

});

function Point(x, y)
{
	this.x = x || 0;
	this.y = y || 0;
};
Point.prototype.x = null;
Point.prototype.y = null;

function SetupEventHandlers()
{
	document.onmouseup = MouseUp;
	document.ontouchmove = PreventDefault;

	var loSVGObject = document.getElementById('SVGObject');

	if (loSVGObject)
	{
		loSVGObject.onmousedown = MouseDown;
		loSVGObject.onmousemove = MouseMove;
		
		loSVGObject.onmouseover = MouseOver;
		loSVGObject.onmouseout = MouseOut;

		loSVGObject.ontouchmove = TouchMove;

		loSVGObject.onkeyup = KeyUpEvents;
		loSVGObject.onkeydown = KeyDownEvents;
	}
}
addLoadEvent(SetupEventHandlers);

function NextTurn()
{
	giTurnPhase++;
	if (giTurnPhase > 3)
		giTurnPhase = 0;

	UpdateTurnPhaseText();

	if (giTurnPhase >= 2)
	{
		// disable the turn phase button
		var loTurnPhaseButton = document.getElementById('btnNext');
		if (loTurnPhaseButton)
		{
			loTurnPhaseButton.disabled = true;
		}
	}

	var url = /*CURRENT_URL +*/ "?plNextTurn=" + giTurnPhase;
	SjaxCall('GET', url, NextTurnReturn);
}

function NextTurnReturn(psAJAXResponseText)
{
	var lsReturnStatus = psAJAXResponseText;

	if (lsReturnStatus.substring(0, 1) == 'E')
	{
		var laData = lsReturnStatus.split('|');

		// end of game condition
		alert('game over! ' + laData[1]);
	}
	else if (giTurnPhase == 2)
	{
		// german movement phase
		var laData = lsReturnStatus.split('|');

		if (lsReturnStatus != "")
		{
			// need to stack these backwards
			for (var i = 0; i < laData.length; i++)
			{
				GermanEvents[i] = laData[laData.length - i - 1];
			}
		}

		if (lsReturnStatus.length > 0)
		{
			var intervalTime = 300;

			if (TestMode) {
				intervalTime = 150;
			}

			GermanEventsTimer = setInterval(DoGermanEvents, intervalTime);
		}
		else
		{
			// just increment the game phase
			NextTurn();
		}
	}
	else if (giTurnPhase == 3)
	{
		// german attack phase
		var laData = lsReturnStatus.split('|');

		if (lsReturnStatus != "")
		{
			// need to stack these backwards
			for (var i = 0; i < laData.length; i++)
			{
				GermanEvents[i] = "A," + laData[laData.length - i - 1];
			}
		}

		if (lsReturnStatus.length > 0)
		{
			var intervalTime = 500;

			if (TestMode) {
				intervalTime = 250;
			}

			GermanEventsTimer = setInterval(DoGermanEvents, intervalTime);
		}
		else
		{
			giTurnPhase++;

			if (giTurnPhase > 3)
				giTurnPhase = 0;

			UpdateTurnPhaseText();
		}
	}

	if (giTurnPhase == 0)
	{
		// enable the phase button
		var loTurnPhaseButton = document.getElementById('btnNext');
		if (loTurnPhaseButton)
		{
			loTurnPhaseButton.disabled = false;
		}

		// reset all the movement and attacking factors of the allied pieces
		for (var i = 0; i < 1000; i++)
		{
			loMovement = document.getElementById('Movement' + i);
			if (loMovement)
			{
				loMovement.textContent = '1';
			}
			loAttackingUnit = document.getElementById('UnitAttackedThisTurn' + i);
			if (loAttackingUnit)
			{
				loAttackingUnit.textContent = '0';
			}
		}
	}
}

function DoGermanEvents()
{
	if (GermanEvents.length > 0)
	{
		var laEvent = GermanEvents[GermanEvents.length - 1].split(',');

		if (laEvent[0] == 'T') {
			var liUnitNumber = laEvent[1];
			var lnDestX = laEvent[2];
			var lnDestY = laEvent[3];
			var lnDestX = lnDestX * 54.75 + 35.25;
			var lnDestY = lnDestY * 62.5 + 31.25 * (lnDestY % 2) + 31.25;

			var lObjectLayer = document.getElementById('Unit' + liUnitNumber);
			if (lObjectLayer) {
				var lnUnitX = parseInt(lObjectLayer.getAttribute('canvasx'));
				var lnUnitY = parseInt(lObjectLayer.getAttribute('canvasy'));

				var loLine = document.getElementById('UnitDest' + liUnitNumber);
				if (loLine) {
					loLine.setAttribute('x1', lnDestX);
					loLine.setAttribute('y1', lnDestY);
					loLine.setAttribute('x2', lnUnitX);
					loLine.setAttribute('y2', lnUnitY);
				}
			}

			GermanEvents.pop();
		}
		else if (laEvent[0] == 'M')
		{
			var liUnitNumber = laEvent[1];
			var lnX = laEvent[2];
			var lnY = laEvent[3];

			if (!MapOccupied(lnX, lnY, liUnitNumber))
			{
				// turn unit red
				var loUnitRect = document.getElementById('UnitRect' + liUnitNumber);
				if (loUnitRect)
				{
					loUnitRect.style.fill = '#ffaaaa';
				}

				// destination
				GermanEvents[GermanEvents.length - 1] = 'D,' + liUnitNumber + ',' + lnX + ',' + lnY;
			}
			else
			{
				if (GermanEvents.length == 1)
				{
					// this is to prevent that we don't get into an endless loop
					GermanEvents.pop();
				}
				else
				{
					// re-stack the current item at the beginning of the list
					var lsItem = GermanEvents[GermanEvents.length - 1];
					GermanEvents.splice(0, 0, lsItem);
					GermanEvents.pop();
				}

				//TODO: need an endless loop counter, just in case
			}
		}
		else if (laEvent[0] == 'D')
		{
			var liUnitNumber = laEvent[1];
			var liX = laEvent[2];
			var liY = laEvent[3];

			// move the unit
			var loUnit = document.getElementById('Unit' + liUnitNumber);
			if (loUnit)
			{
				loUnit.setAttribute('gridx', liX);
				loUnit.setAttribute('gridy', liY);

				// convert back into screen coordinates
				var lnX = liX * 54.75 + 35.25;
				var lnY = liY * 62.5 + 31.25 * (liX % 2) + 31.25;

				loUnit.setAttribute('canvasx', lnX);
				loUnit.setAttribute('canvasy', lnY);

				loUnit.setAttribute("transform", "translate(" + lnX + " " + lnY + ")");

				// check to see if this unit moved into a viewable cell
				if (ViewMap[liX][liY])
				{
					loUnit.style.display = '';
				}
				else
				{
					loUnit.style.display = 'none';
				}

				if (TestMode) {
					loUnit.style.display = '';
				}
			}

			// decrement the movement number
			var loMovement = document.getElementById('Movement' + liUnitNumber);
			if (loMovement)
			{
				loMovement.textContent = '0';
			}

			// re-paint the unit gray
			var loUnitRect = document.getElementById('UnitRect' + liUnitNumber);
			if (loUnitRect)
			{
				loUnitRect.style.fill = '#ddd';
			}

			GermanEvents.pop();
		}
		else if (laEvent[0] == 'A')
		{
			// turn allied and german units red
			var liGermanUnit = laEvent[1];
			var liAlliedUnit = laEvent[2];
			var liResult = laEvent[3];

			var loUnitRect = document.getElementById('UnitRect' + liGermanUnit);
			if (loUnitRect)
			{
				loUnitRect.style.fill = '#ffaaaa';
			}
			loUnitRect = document.getElementById('UnitRect' + liAlliedUnit);
			if (loUnitRect)
			{
				loUnitRect.style.fill = '#ffaaaa';
			}
			else
			{
				alert('Bug: Allied unit number ' + liAlliedUnit + ' has already been destroyed!');
			}

			// destination
			GermanEvents[GermanEvents.length - 1] = 'B,' + liGermanUnit + ',' + liAlliedUnit + ',' + liResult;
		}
		else if (laEvent[0] == 'B')
		{
			// remove destroyed unit
			var liGermanUnit = laEvent[1];
			var liAlliedUnit = laEvent[2];
			var liResult = laEvent[3];

			var loUnitRect = document.getElementById('UnitRect' + liGermanUnit);
			if (loUnitRect)
			{
				loUnitRect.style.fill = '#ddd';
			}
			loUnitRect = document.getElementById('UnitRect' + liAlliedUnit);
			if (loUnitRect)
			{
				loUnitRect.style.fill = '#d0fed0';
			}

			if (liResult == 1)
			{
				// allied unit destroyed
				var loUnit = document.getElementById('Unit' + liAlliedUnit);
				if (loUnit)
					loUnit.parentNode.removeChild(loUnit);
				else
					alert('illegal attempt to remove allied unit # ' + liAlliedUnit);

				RecomputeMapView();
			}
			else if (liResult == 2)
			{
				// german unit destroyed
				var loUnit = document.getElementById('Unit' + liGermanUnit);
				if (loUnit)
					loUnit.parentNode.removeChild(loUnit);
				else
					alert('illegal attempt to remove german unit # ' + liGermanUnit);
			}
			else if (liResult == 3) {
				// allied unit damaged (subtract from defense)
				var loUnit = document.getElementById('Defense' + liAlliedUnit);
				if (loUnit) {
					var defenseNumber = parseInt(loUnit.textContent);
					loUnit.textContent = (defenseNumber - 1);
					loUnit.style.fill = 'red';
				}
				else {
					alert('illegal attempt to remove german unit # ' + liAlliedUnit);
				}
			}
			else if (liResult == 4) {
				// allied unit damaged (subtract 2 from defense)
				var loUnit = document.getElementById('Defense' + liAlliedUnit);
				if (loUnit) {
					var defenseNumber = parseInt(loUnit.textContent);
					loUnit.textContent = (defenseNumber - 2);
					loUnit.style.fill = 'red';
				}
				else {
					alert('illegal attempt to remove german unit # ' + liAlliedUnit);
				}
			}

			GermanEvents.pop();
		}
	}
	else
	{
		window.clearInterval(GermanEventsTimer)
		GermanEventsTimer = null;

		NextTurn();
	}
}

function UpdateTurnPhaseText()
{
	var loTurnphaseText = document.getElementById('TurnPhaseText');
	if (loTurnphaseText)
	{
		switch (giTurnPhase)
		{
			case 0:
				loTurnphaseText.innerHTML = "Allied Movement";
				break;
			case 1:
				loTurnphaseText.innerHTML = "Allied Attack";
				break;
			case 2:
				loTurnphaseText.innerHTML = "Axis Movement";
				break;
			case 3:
				loTurnphaseText.innerHTML = "Axis Attack";
				break;
		}

		if (giTurnPhase < 2)
			loTurnphaseText.style.color = 'blue';
		else
			loTurnphaseText.style.color = 'red';
	}
}

function PreventDefault(event)
{
	event.preventDefault();
}

function MouseDown(event)
{
	event.preventDefault();
}

function MouseMove(event)
{
	var IE = document.all ? true : false;
	if (IE)
	{ // grab the x-y pos.s if browser is IE
		if (document.body)
		{
			gnX = event.clientX + document.body.scrollLeft;
			gnY = event.clientY + document.body.scrollTop;
		}
	}
	else
	{  // grab the x-y pos.s if browser is NS
		gnX = event.pageX;
		gnY = event.pageY;
	}

	var lPoint = TopLeftOffset(gnX, gnY);
	gnX -= lPoint.x;
	gnY -= lPoint.y;

	if (gnX < 0)
	{
		gnX = 0;
	}
	if (gnY < 0)
	{
		gnY = 0;
	}

	if (giUnitSelected > -1)
	{
		MoveUnit(giUnitSelected);
	}

	event.preventDefault();
}

function MouseUp(event)
{
	if (giUnitSelected > -1)
	{
		MouseUpUnit(giUnitSelected);
	}

	event.preventDefault();
}

function MouseOver(event)
{
}

function MouseOut(event)
{
}

function TouchStart(event)
{
	event.preventDefault();
}

function TouchMove(event)
{
	var targetEvent = event.touches.item(0);

	if (event.touches.length == 1)
	{
		gnX = parseInt(targetEvent.clientX);
		gnY = parseInt(targetEvent.clientY);

		var lPoint = TopLeftOffset(gnX, gnY);
		gnX -= lPoint.x;
		gnY -= lPoint.y;

		if (giUnitSelected > -1)
		{
			MoveUnit(giUnitSelected);
		}
	}

	event.preventDefault();
}

function TouchCancel(event)
{
	event.preventDefault();
}

function TouchEnd(event)
{
	event.preventDefault();
}

function KeyUpEvents(event)
{
	var lsKeyCode;
	if (!event)
		var event = window.event;
	if (!event)
		return;

	if (event.keyCode)
		lsKeyCode = event.keyCode;
	else if (event.which)
		lsKeyCode = event.which;
}

function KeyDownEvents(event)
{
	var lsKeyCode;
	if (!event)
		var event = window.event;
	if (!event)
		return;

	if (event.keyCode)
		lsKeyCode = event.keyCode;
	else if (event.which)
		lsKeyCode = event.which;
}

function MouseOverUnit(piUnitNumber)
{
	if (giTurnPhase > 0) {
		return;
	}

	var loObj = document.getElementById('UnitRect' + piUnitNumber);
	if (loObj) {
		if (!HasUnitBeenMoved(piUnitNumber)) {
			loObj.style.stroke = 'red';
		}
	}
}

function MouseOutUnit(piUnitNumber)
{
	if (giTurnPhase > 0) {
		return;
	}

	var loObj = document.getElementById('UnitRect' + piUnitNumber);
	if (loObj) {
		loObj.style.stroke = 'black';
	}
}

function MouseDownUnit(piUnitNumber)
{
	if (giTurnPhase == 1)
	{
		giUnitSelected = piUnitNumber;
		
		var lObjectLayer = document.getElementById('Unit' + piUnitNumber);
		if (lObjectLayer)
		{
			var lnCanvasX = parseInt(lObjectLayer.getAttribute('canvasx'));
			var lnCanvasY = parseInt(lObjectLayer.getAttribute('canvasy'));

			var lPoint = ComputeMapCoords(new Point(lnCanvasX, lnCanvasY));

			giUnitStartX = lPoint.x;
			giUnitStartY = lPoint.y;
		}

		// allied attack phase
		if (!HasUnitAttackedThisTurn(piUnitNumber)) {
			var loObj = document.getElementById('UnitRect' + piUnitNumber);
			if (loObj) {
				loObj.style.fill = '#ffaaaa';
			}
		}
	}

	if (giTurnPhase == 0)
	{
		// allied move phase
		var loObj = document.getElementById('UnitRect' + piUnitNumber);
		if (loObj)
		{
			if (HasUnitBeenMoved(piUnitNumber))
				return;

			giUnitSelected = piUnitNumber;

			var lObjectLayer = document.getElementById('Unit' + piUnitNumber);
			if (lObjectLayer)
			{
				var lnCanvasX = parseInt(lObjectLayer.getAttribute('canvasx'));
				var lnCanvasY = parseInt(lObjectLayer.getAttribute('canvasy'));

				var lPoint = ComputeMapCoords(new Point(lnCanvasX, lnCanvasY));

				giUnitStartX = lPoint.x;
				giUnitStartY = lPoint.y;
			}
		}
	}
}

function MouseUpUnit()
{
	if (giTurnPhase == 1) {
		// allied attack phase
		if (!HasUnitAttackedThisTurn(giUnitSelected)) {
			var loObj = document.getElementById('UnitRect' + giUnitSelected);
			if (loObj) {
				loObj.style.fill = '#d0fed0';
			}

			var loObj = document.getElementById('UnitRect' + giGermanUnitUnderAttack);
			if (loObj) {
				loObj.style.fill = '#ddd';
			}

			// ajax the attack request

			if (giGermanUnitUnderAttack > -1) {
				var url = /*CURRENT_URL +*/ "?piAttackGermanUnit=" + giGermanUnitUnderAttack + "&piAlliedUnit=" + giUnitSelected;
				SjaxCall('GET', url, AttackGermanUnitReturn);
			}
		}

		// reset all the global variables
		giUnitSelected = -1;
		giUnitStartX = -1;
		giUnitStartY = -1;
		giGermanUnitUnderAttack = -1;
		gnMoveOffset = null;
	}

	if (giTurnPhase == 0)
	{
		// allied move phase
		var loObj = document.getElementById('UnitRect' + giUnitSelected);
		if (loObj)
		{
			// decrement movement number
			var lObjectLayer = document.getElementById('Unit' + giUnitSelected);
			if (lObjectLayer)
			{
				var lnCanvasX = parseInt(lObjectLayer.getAttribute('canvasx'));
				var lnCanvasY = parseInt(lObjectLayer.getAttribute('canvasy'));

				var lPoint = ComputeMapCoords(new Point(lnCanvasX, lnCanvasY));
				lObjectLayer.setAttribute('gridx', lPoint.x);
				lObjectLayer.setAttribute('gridy', lPoint.y);

				if (lPoint.x != giUnitStartX || lPoint.y != giUnitStartY)
				{
					loMovement = document.getElementById('Movement' + giUnitSelected);
					if (loMovement)
					{
						loMovement.textContent = '0';

						var url = /*CURRENT_URL +*/ "?piMoveUnit=" + giUnitSelected + "&pnX=" + lPoint.x + "&pnY=" + lPoint.y;
						SjaxCall('GET', url, null);
					}

					// need to handle the map mask and visibility
					UnMaskRegion(lPoint.x, lPoint.y);
					RecomputeMapView();
				}
			}

			giUnitSelected = -1;
			giUnitStartX = -1;
			giUnitStartY = -1;
			gnMoveOffset = null;
			loObj.style.stroke = 'black';
		}
	}
}

function UnMaskRegion(piX, piY)
{
	var loMask = document.getElementById('Mask' + piX + "," + piY);
	if (loMask || TestMode) {
		loMask.style.display = 'none';
	}

	// unmask cell above and below
	if (piY > 0)
	{
		var loMask = document.getElementById('Mask' + piX + "," + (piY - 1));
		if (loMask || TestMode) {
			loMask.style.display = 'none';
		}
	}

	if (piY < MaxY-1)
	{
		var loMask = document.getElementById('Mask' + piX + "," + (piY + 1));
		if (loMask || TestMode) {
			loMask.style.display = 'none';
		}
	}

	if (piX % 2 == 1)
	{
		// y and y+1
		if (piX > 0)
		{
			var loMask = document.getElementById('Mask' + (piX - 1) + "," + piY);
			if (loMask || TestMode) {
				loMask.style.display = 'none';
			}

			if (piY < MaxY-1)
			{
				var loMask = document.getElementById('Mask' + (piX - 1) + "," + (piY + 1));
				if (loMask || TestMode) {
					loMask.style.display = 'none';
				}
			}
		}

		if (piX < MaxX-1)
		{
			var loMask = document.getElementById('Mask' + (piX + 1) + "," + piY);
			if (loMask || TestMode) {
				loMask.style.display = 'none';
			}

			if (piY < MaxY-1)
			{
				var loMask = document.getElementById('Mask' + (piX + 1) + "," + (piY + 1));
				if (loMask || TestMode) {
					loMask.style.display = 'none';
				}
			}
		}
	}
	else
	{
		// y and y-1
		if (piX > 0)
		{
			var loMask = document.getElementById('Mask' + (piX - 1) + "," + piY);
			if (loMask || TestMode) {
				loMask.style.display = 'none';
			}

			if (piY > 0)
			{
				var loMask = document.getElementById('Mask' + (piX - 1) + "," + (piY - 1));
				if (loMask || TestMode) {
					loMask.style.display = 'none';
				}
			}
		}

		if (piX < MaxX-1)
		{
			var loMask = document.getElementById('Mask' + (piX + 1) + "," + piY);
			if (loMask || TestMode) {
				loMask.style.display = 'none';
			}

			if (piY > 0)
			{
				var loMask = document.getElementById('Mask' + (piX + 1) + "," + (piY - 1));
				if (loMask || TestMode) {
					loMask.style.display = 'none';
				}
			}
		}
	}
}

function AttackGermanUnitReturn(psAJAXResponseText)
{
	// German Unit #, allied unit #, result
	var laData = psAJAXResponseText.split(',');

	var liGermanUnit = laData[0];
	var liAlliedUnit = laData[1];
	var liResult = laData[2];

	document.getElementById('UnitAttackedThisTurn' + liAlliedUnit).textContent = '1';

	// result (0=none,1=german destroyed,2=allied destroyed)
	if (liResult == 2)
	{
		// allied unit destroyed
		var loUnit = document.getElementById('Unit' + liAlliedUnit);
		if (loUnit) {
			loUnit.parentNode.removeChild(loUnit);
		}
		else {
			alert('illegal attempt to remove allied unit # ' + liAlliedUnit);
		}

		RecomputeMapView();
	}
	else if (liResult == 1)
	{
		// german unit destroyed
		var loUnit = document.getElementById('Unit' + liGermanUnit);
		if (loUnit) {
			loUnit.parentNode.removeChild(loUnit);
		}
		else {
			alert('illegal attempt to remove german unit # ' + liGermanUnit);
		}
	}
	else if (liResult == 3) {
		// german unit damaged (subtract from defense)
		var loUnit = document.getElementById('Defense' + liGermanUnit);
		if (loUnit) {
			var defenseNumber = parseInt(loUnit.textContent);
			loUnit.textContent = (defenseNumber - 1);
			loUnit.style.fill = 'red';
		}
		else {
			alert('illegal attempt to remove german unit # ' + liGermanUnit);
		}
	}
	else if (liResult == 4) {
		// german unit damaged (subtract 2 from defense)
		var loUnit = document.getElementById('Defense' + liGermanUnit);
		if (loUnit) {
			var defenseNumber = parseInt(loUnit.textContent);
			loUnit.textContent = (defenseNumber - 2);
			loUnit.style.fill = 'red';
		}
		else {
			alert('illegal attempt to remove german unit # ' + liGermanUnit);
		}
	}
}

function TopLeftOffset()
{
	var x = 0;
	var y = 0;
	var xRight = 0;
	var a = document.getElementById('SVGObjectBox');
	while (a)
	{
		var tn = a.tagName.toUpperCase();
		x += a.offsetLeft - (tn == "DIV" && a.scrollLeft ? a.scrollLeft : 0);
		y += a.offsetTop - (tn == "DIV" && a.scrollTop ? a.scrollTop : 0);

		if (tn == "BODY")
			break;

		a = a.offsetParent;
	}

	var lPoint = new Point(x, y);

	return lPoint;
}

function MoveUnit(piUnitNumber)
{
	if (giTurnPhase == 0)
	{
		// allied move phase
		var lObjectLayer = document.getElementById('Unit' + piUnitNumber);

		var lMouse = new Point(gnX, gnY);

		// the user might not click directly on the center point, so we need to move relative to the center vs. starting mouse coords.
		if (gnMoveOffset == null)
		{
			var lnCanvasX = parseInt(lObjectLayer.getAttribute('canvasx'));
			var lnCanvasY = parseInt(lObjectLayer.getAttribute('canvasy'));
			gnMoveOffset = new Point(lMouse.x - lnCanvasX, lMouse.y - lnCanvasY);
		}

		lMouse.x = lMouse.x - gnMoveOffset.x;
		lMouse.y = lMouse.y - gnMoveOffset.y;

		lMouse = SnapTo(lMouse);

		lObjectLayer.setAttribute('canvasx', lMouse.x);
		lObjectLayer.setAttribute('canvasy', lMouse.y);

		lObjectLayer.setAttribute("transform", "translate(" + lMouse.x + " " + lMouse.y + ")");
	}
	else if (giTurnPhase == 1)
	{
		// allied attack phase
		var lObjectLayer = document.getElementById('Unit' + piUnitNumber);

		if (!HasUnitAttackedThisTurn(piUnitNumber)) {
			var lMouse = new Point(gnX, gnY);

			// the user might not click directly on the center point, so we need to move relative to the center vs. starting mouse coords.
			if (gnMoveOffset == null) {
				var lnCanvasX = parseInt(lObjectLayer.getAttribute('canvasx'));
				var lnCanvasY = parseInt(lObjectLayer.getAttribute('canvasy'));
				gnMoveOffset = new Point(lMouse.x - lnCanvasX, lMouse.y - lnCanvasY);
			}

			lMouse.x = lMouse.x - gnMoveOffset.x;
			lMouse.y = lMouse.y - gnMoveOffset.y;

			lMouse = SnapTo(lMouse);

			// find unit at lMouse location
			var lCoord = ComputeMapCoords(lMouse);

			var liGermanUnitNumber = FindGermanUnitAtCoords(lCoord.x, lCoord.y);
			if (liGermanUnitNumber > -1) {
				var loObj = document.getElementById('UnitRect' + liGermanUnitNumber);
				if (loObj) {
					loObj.style.fill = '#ffaaaa';
				}

				giGermanUnitUnderAttack = liGermanUnitNumber;
			}
		}
	}
}

function SnapTo(pMouse)
{
	var lPoint = ComputeMapCoords(pMouse);

	var liX = lPoint.x; //Math.round((pMouse.x - 35.25) / 54.75);

	// limit movement to edges of the map
	if (liX >= MaxX)
		liX = MaX-1;
	if (liX < 0)
		liX = 0;

	// limit to range of unit
	if (Math.abs(liX - giUnitStartX) > 1)
	{
		if (liX - giUnitStartX < 0)
			liX = giUnitStartX - 1;
		else
			liX = giUnitStartX + 1;
	}

	var liY = lPoint.y; //Math.round((pMouse.y - 31.25 - (31.25 * (liX % 2))) / 62.5);

	// limit movement to edges of the map
	if (liY >= MaxY)
		liY = MaxY;
	if (liY < 0)
		liY = 0;

	// limit range of unit
	if (liX != giUnitStartX)
	{
		if (liX % 2 == 1)
		{
			// if odd, then y-1, y
			if (liY > giUnitStartY)
				liY = giUnitStartY;
			else if (liY < giUnitStartY - 1)
				liY = giUnitStartY - 1;
		}
		else
		{
			// if even, then y, y+1
			if (liY < giUnitStartY)
				liY = giUnitStartY;
			else if (liY > giUnitStartY + 1)
				liY = giUnitStartY + 1;
		}
	}
	else if (Math.abs(liY-giUnitStartY) > 1)
	{
		// make sure y-giUnitStartY < 1
		if (liY < giUnitStartY)
			liY = giUnitStartY - 1;
		else
			liY = giUnitStartY + 1;
	}

	// if map occupied, then snap unit back to starting position.
	if (giTurnPhase == 0)
	{
		if (MapOccupied(lPoint.x, lPoint.y, giUnitSelected))
		{
			liX = giUnitStartX;
			liY = giUnitStartY;
		}
	}

	//TODO: need to see if unit is starting next to enemy unit and snapping next to same or other enemy unit

	// convert back into screen coordinates
	var lnX = liX * 54.75 + 35.25;
	var lnY = liY * 62.5 + 31.25 * (liX % 2) + 31.25;

	var lMouse = new Point(lnX, lnY);

	return lMouse;
}

function HasUnitAttackedThisTurn(piUnitNumber) {
	// check to see if unit has already been moved
	loAttackUnit = document.getElementById('UnitAttackedThisTurn' + piUnitNumber);
	if (loAttackUnit) {
		if (parseInt(loAttackUnit.textContent) == 1)
		{
			return true;
		}
	}

	return false;
}
function HasUnitBeenMoved(piUnitNumber)
{
	// check to see if unit has already been moved
	loMovement = document.getElementById('Movement' + piUnitNumber);
	if (loMovement)
	{
		if (parseInt(loMovement.textContent) == 0)
		{
			return true;
		}
	}

	return false;
}
function ComputeMapCoords(pMouse)
{
	var liX = Math.round((pMouse.x - 35.25) / 54.75);
	var liY = Math.round((pMouse.y - 31.25 - (31.25 * (liX % 2))) / 62.5);

	var lPoint = new Point(liX, liY);

	return lPoint;
}
function MapOccupied(piX, piY, piUnitNumber)
{
	// see if a unit, other than piUnitNumber, occupies piX,piY
	for (var i = 0; i < 1000; i++)
	{
		if (i != piUnitNumber)
		{
			var lObjectLayer = document.getElementById('Unit' + i);
			if (lObjectLayer)
			{
				var lnGridX = parseInt(lObjectLayer.getAttribute('gridx'));
				var lnGridY = parseInt(lObjectLayer.getAttribute('gridy'));

				if (lnGridX == piX && lnGridY == piY)
					return true;
			}
		}
	}

	return false;
}

function FindGermanUnitAtCoords(piX, piY)
{
	for (var i = 0; i < 1000; i++)
	{
		var lObjectLayer = document.getElementById('Unit' + i);
		if (lObjectLayer)
		{
			if (lObjectLayer.getAttribute('nationality') == 'G')
			{
				var lnGridX = parseInt(lObjectLayer.getAttribute('gridx'));
				var lnGridY = parseInt(lObjectLayer.getAttribute('gridy'));

				if (lnGridX == piX && lnGridY == piY)
					return i;
			}
		}
	}

	return -1;
}

function RecomputeMapView()
{
	// initialize ViewMap array (MaxX x MaxY array)
	for (var y = 0; y < MaxY; y++) {
		for (var x = 0; x < MaxX; x++) {
			ViewMap[x][y] = false;
		}
	}

	// spin through all units and unmask + view areas where units are sitting
	for (var i = 0; i < 1000; i++)
	{
		var loUnit = document.getElementById('Unit' + i);
		if (loUnit)
		{
			// only look at allied units
			if (loUnit.getAttribute('nationality') == 'A')
			{
				// unmask and view all cells surrounding this x,y position
				var liX = parseInt(loUnit.getAttribute('gridx'));
				var liY = parseInt(loUnit.getAttribute('gridy'));

				ViewkMapRegion(liX, liY);
			}
		}
	}

	// flip any view masks
	for (var y = 0; y < MaxY; y++) {
		for (var x = 0; x < MaxX; x++) {
			var loView = document.getElementById('View' + x + "," + y);
			if (loView) {
				if (ViewMap[x][y]) {
					loView.style.display = 'none';
				}
				else {
					loView.style.display = '';
				}

				if (TestMode) {
					loView.style.display = 'none';
				}
			}
		}
	}

	// hide or show enemy units if they are under the view map mask
	for (var i = 0; i < 1000; i++)
	{
		var loUnit = document.getElementById('Unit' + i);
		if (loUnit)
		{
			// only look at german units
			if (loUnit.getAttribute('nationality') == 'G')
			{
				// see if this german unit is visible or not
				var liX = parseInt(loUnit.getAttribute('gridx'));
				var liY = parseInt(loUnit.getAttribute('gridy'));

				if (ViewMap[liX][liY])
				{
					loUnit.style.display = '';
				}
				else
				{
					loUnit.style.display = 'none';
				}

				if (TestMode) {
					loUnit.style.display = '';
				}
			}
		}
	}
}

function ViewkMapRegion(piX, piY)
{
	// TODO: if range factors are added, we'll need to account for that
	ViewMap[piX][piY] = true;

	// unmask cell above and below
	if (piY > 0)
	{
		ViewMap[piX][piY - 1] = true;
	}

	if (piY < MaxY-1)
	{
		ViewMap[piX][piY + 1] = true;
	}

	if (piX % 2 == 1)
	{
		// y and y+1
		if (piX > 0)
		{
			ViewMap[piX - 1][piY] = true;

			if (piY < MaxY-1)
			{
				ViewMap[piX - 1][piY + 1] = true;
			}
		}

		if (piX < MaxX-1)
		{
			ViewMap[piX + 1][piY] = true;

			if (piY < MaxY-1)
			{
				ViewMap[piX + 1][piY + 1] = true;
			}
		}
	}
	else
	{
		// y and y-1
		if (piX > 0)
		{
			ViewMap[piX - 1][piY] = true;

			if (piY > 0)
			{
				ViewMap[piX - 1][piY - 1] = true;
			}
		}

		if (piX < MaxX-1)
		{
			ViewMap[piX + 1][piY] = true;

			if (piY > 0)
			{
				ViewMap[piX + 1][piY - 1] = true;
			}
		}
	}
}