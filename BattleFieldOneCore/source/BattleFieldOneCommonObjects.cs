using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BattleFieldOneCore
{
    public class BattleFieldOneCommonObjects
    {
        public static double ComputeCenterX(int piX)
        {
            return 35.25 + (54.75 * piX);
        }

        public static double ComputeCenterY(int piX, int piY)
        {
            return 31.25 + (31.25 * (piX % 2) + piY * 62.5);
        }

				public static double Distance(int piSX, int piSY, int piEX, int piEY)
				{
					// convert the map index coordinates into screen coordinates to get the correct distance
					double lnSX = ComputeCenterX(piSX);
					double lnSY = ComputeCenterY(piSX, piSY);
					double lnEX = ComputeCenterX(piEX);
					double lnEY = ComputeCenterY(piEX, piEY);

					return Math.Sqrt(Math.Pow((lnSX - lnEX), 2) + Math.Pow((lnSY - lnEY), 2));
				}
    }
}
