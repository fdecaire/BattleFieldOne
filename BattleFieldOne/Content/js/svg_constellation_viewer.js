var transMatrix = [1, 0, 0, 1, 0, 0];

function addLoadEvent(func)
{
	var oldonload = window.onload;
	if (typeof window.onload != 'function')
	{
		window.onload = func;
	} else
	{
		window.onload = function ()
		{
			if (oldonload)
			{
				oldonload();
			}
			func();
		}
	}
}

function SetupEventHandlers()
{
	var loSVGObject = document.getElementById('SVGObject');

	if (loSVGObject)
	{
		loSVGObject.onmousedown = MouseDown;
		loSVGObject.onmousemove = MouseMove;
		loSVGObject.onmouseup = MouseUp;
		loSVGObject.onmouseout = MouseOut;
	}
}

function InitializeSVGViewer()
{
	// random rotation angle if needed
	var lnAngle = 0;


	var lEquipmentLayer = document.getElementById("lblMain");
	lEquipmentLayer.setAttribute("transform", "rotate(" + lnAngle + ", 400, 400)");

}
addLoadEvent(InitializeSVGViewer);

function MouseDown(event)
{

}

function MouseMove(event)
{

}

function MouseUp(event)
{
}

function MouseDown(event)
{
}

function MouseOverStar(obj)
{
	obj.style.fill = 'red';
}

function MouseOutStar(obj)
{
	obj.style.fill = 'white';
}

