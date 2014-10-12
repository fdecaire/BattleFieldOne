using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BattleFieldOneCore
{
	public class MapCoordinates
	{
		public int X { get; set; }
		public int Y { get; set; }
		public double Distance { get; set; }

		public MapCoordinates(int piX, int piY)
		{
			X = piX;
			Y = piY;
		}

		public void SnapTo(int piSourceX, int piSourceY)
		{
			// correct x,y to snap to the closest grid coordinate, one hex away from sourcex,sourcey
			int liX = X;

			// limit movement to edges of the map
			if (liX > 12)
				liX = 12;
			if (liX < 0)
				liX = 0;

			// limit to range of unit
			if (Math.Abs(liX - piSourceX) > 1)
			{
				if (liX - piSourceX < 0)
					liX = piSourceX - 1;
				else
					liX = piSourceX + 1;
			}

			int liY = Y;

			// limit movement to edges of the map
			if (liY > 9)
				liY = 9;
			if (liY < 0)
				liY = 0;

			// limit range of unit
			if (liX != piSourceX)
			{
				if (liX % 2 == 1)
				{
					// if odd, then y-1, y
					if (liY > piSourceY)
						liY = piSourceY;
					else if (liY < piSourceY - 1)
						liY = piSourceY - 1;
				}
				else
				{
					// if even, then y, y+1
					if (liY < piSourceY)
						liY = piSourceY;
					else if (liY > piSourceY + 1)
						liY = piSourceY + 1;
				}
			}
			else if (Math.Abs(liY - piSourceY) > 1)
			{
				// make sure y-giUnitStartY < 1
				if (liY < piSourceY)
					liY = piSourceY - 1;
				else
					liY = piSourceY + 1;
			}

			X = liX;
			Y = liY;
		}
	}
}
