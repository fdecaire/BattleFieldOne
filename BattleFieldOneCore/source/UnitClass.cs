using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BattleFieldOneCore
{
	public class UnitClass
	{
		public int Number { get; private set; }
		public int Defense { get; set; }
		public int Offense { get; private set; }
		public int Movement { get; private set; }
		public int Range { get; private set; }
		public int UnitType { get; private set; }
		public int X { get; set; }
		public int Y { get; set; }
		public NATIONALITY Nationality { get; private set; }
		public UNITCOMMAND Command { get; set; } // used by enemy units only
		public int DestX; // used by enemy units only
		public int DestY; // used by enemy units only
		public bool Destroyed; // flag to indicate unit has been destroyed but not removed from the list yet
		public bool UnitHasAttackedThisTurn;

		public UnitClass(int piUnitType, NATIONALITY pNationality, int piX, int piY, int piUnitNumber)
		{
			X = piX;
			Y = piY;
			Nationality = pNationality;
			UnitType = piUnitType;
			Command = UNITCOMMAND.None;
			Number = piUnitNumber;
			Destroyed = false;
			UnitHasAttackedThisTurn = false;

			switch (piUnitType)
			{
				case 1: // troop
					Defense = 2;
					Offense = 1;
					Movement = 1;
					Range = 1;
					break;
				case 2: // tank
					Defense = 16;
					Offense = 6;
					Movement = 3;
					Range = 1;
					break;
			}
		}

		public string Plot()
		{
			double lnCX = BattleFieldOneCommonObjects.ComputeCenterX(X);
			double lnCY = BattleFieldOneCommonObjects.ComputeCenterY(X, Y);
			StringBuilder @out = new StringBuilder();

			string lsUnitColor = "#ddd";
			if (Nationality == NATIONALITY.Allied)
				lsUnitColor = "#d0fed0";

			string lsMouseEvents = "";
			if (Nationality == NATIONALITY.Allied)
				lsMouseEvents = "onmouseover='MouseOverUnit(" + Number + ");' onmouseout='MouseOutUnit(" + Number + ");' onmousedown='MouseDownUnit(" + Number + ");' onmouseup='MouseUpUnit();' ontouchstart='MouseDownUnit(" + Number + ");' ontouchend='MouseUpUnit();'";

			// wrap entire unit in a canvas
			@out.Append("<g id='Unit" + Number + "' nationality='" + (Nationality == NATIONALITY.Allied ? "A" : "G") + "' canvasx='" + lnCX.ToString("0") + "' canvasy='" + lnCY.ToString("0") + "' gridx='" + X + "' gridy='" + Y + "' transform='translate(" + lnCX + " " + lnCY + ")' >");

			@out.Append("<rect x='" + (-23.5) + "' y='" + (-23.5) + "' width='" + (23.5 * 2) + "' height='" + (23.5 * 2) + "' fill='" + lsUnitColor + "' stroke='black' stroke-width='2' id='UnitRect" + Number + "' " + lsMouseEvents + " />");

			switch (UnitType)
			{
				case 1: // troop
					@out.Append("<image xlink:href='/Content/img/troop.png' x='" + (0 - 12) + "' y='" + (0 - 6) + "' width='23' height='12' " + lsMouseEvents + " />");
					break;
				case 2: // tank
					@out.Append("<image xlink:href='/Content/img/tank.png' x='" + (0 - 12) + "' y='" + (0 - 6) + "' width='23' height='12' " + lsMouseEvents + " />");
					break;
			}

			// offense (upper left)
			@out.Append("<text style='editable:none;cursor:default;' font-family='Arial' font-size='16' x='" + (-23.5 + 1) + "' y='" + (-23.5 + 16) + "' fill='black' " + lsMouseEvents + " id='Offense" + Number + "'>" + Offense + "</text>");

			// defense (lower left)
			@out.Append("<text style='editable:none;cursor:default;' font-family='Arial' font-size='16' x='" + (-23.5 + 1) + "' y='" + (23.5 - 3) + "' fill='black' " + lsMouseEvents + " id='Defense" + Number + "'>" + Defense + "</text>");

			// range (upper right)
			@out.Append("<text style='editable:none;cursor:default;' font-family='Arial' font-size='16' x='" + (23.5 - 12) + "' y='" + (-23.5 + 16) + "' fill='black' " + lsMouseEvents + " id='Range" + Number + "'>" + Range + "</text>");

			// movement (lower right)
			@out.Append("<text style='editable:none;cursor:default;' font-family='Arial' font-size='16' x='" + (23.5 - 12) + "' y='" + (23.5 - 3) + "' fill='black' " + lsMouseEvents + " id='Movement" + Number + "'>" + Movement + "</text>");

			// unit number (temporary - upper center)
			@out.Append("<text style='editable:none;text-anchor:middle;cursor:default;' font-family='Arial' font-size='12' x='" + (0) + "' y='" + (-23.5 + 12) + "' fill='black' " + lsMouseEvents + " id='UnitNumber" + Number + "'>" + Number + "</text>");

			// unit attacked this turn flag (always hidden)
			@out.Append("<text style='display:none;' x='" + (0) + "' y='" + (-23.5 + 12) + "' " + lsMouseEvents + " id='UnitAttackedThisTurn" + Number + "'>" + (UnitHasAttackedThisTurn ? "1" : "0") + "</text>");

			@out.Append("</g>");

			return @out.ToString();
		}
	}
}
