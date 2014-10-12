function MouseOverLink(obj)
{
	obj.style.color = 'blue';
}

function MouseOutLink(obj)
{
	obj.style.color = 'black';
}

function GetWindowHeight()
{
	var Width = 0, Height = 0;
	if (typeof (window.innerWidth) == 'number')
	{
		//Non-IE
		Width = window.innerWidth;
		Height = window.innerHeight;
	}
	else if (document.documentElement && (document.documentElement.clientWidth || document.documentElement.clientHeight))
	{
		//IE 6+ in 'standards compliant mode'
		Width = document.documentElement.clientWidth;
		Height = document.documentElement.clientHeight;
	}
	else if (document.body && (document.body.clientWidth || document.body.clientHeight))
	{
		//IE 4 compatible
		Width = document.body.clientWidth;
		Height = document.body.clientHeight;
	}
	return Height;
}

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

function ResizeWindow()
{
	var liHeight = parseInt(GetWindowHeight());

	var loBottomTable = document.getElementById('BottomTable');
	if (loBottomTable)
	{
		var liPosition = parseInt(loBottomTable.style.top);
		
		if (liPosition < liHeight - 200)
		{
			liHeight -= 200;
			//loBottomTable.style.position='fixed';
			//loBottomTable.style.top = liHeight + 'px';
		}
	}
}

window.onresize = ResizeWindow;
addLoadEvent(ResizeWindow);

function AjaxCall(type, url, returnfunction)
{
	var xmlhttp = null;

	if (window.XMLHttpRequest)
	{// code for IE7+, Firefox, Chrome, Opera, Safari
		xmlhttp = new XMLHttpRequest();
	}
	else try
		{
		xmlhttp = new ActiveXObject('Msxml2.XMLHTTP');
	}
    catch (e)
	{// code for IE6, IE5
		try
		{
			xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
		}
		catch (e)
		{

		}
	}

	xmlhttp.open(type, url, true);

	if (returnfunction == null)
	{
		xmlhttp.onreadystatechange = function () { };
	}
	else
	{
		xmlhttp.onreadystatechange = function ()
		{
			if (xmlhttp.readyState == 4)
			{
				if (xmlhttp.status == 200)
				{
					returnfunction(xmlhttp.responseText);
				}
			}
		};
	}

	xmlhttp.send();
}
function SjaxCall(type, url, returnfunction)
{
	var xmlhttp = null;

	if (window.XMLHttpRequest)
	{// code for IE7+, Firefox, Chrome, Opera, Safari
		xmlhttp = new XMLHttpRequest();
	}
	else try
		{
		xmlhttp = new ActiveXObject('Msxml2.XMLHTTP');
	}
    catch (e)
	{// code for IE6, IE5
		try
		{
			xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
		}
		catch (e)
		{

		}
	}

	xmlhttp.open(type, url, false);

	if (returnfunction == null)
	{
		xmlhttp.onreadystatechange = function () { };
	}
	else
	{
		xmlhttp.onreadystatechange = function ()
		{
			if (xmlhttp.readyState == 4)
			{
				if (xmlhttp.status == 200)
				{
					returnfunction(xmlhttp.responseText);
				}
			}
		};
	}

	xmlhttp.send();
}