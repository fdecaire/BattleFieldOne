using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleFieldOneCore
{
	public class GameMap
	{
		public int Terrain { get; set; } // terrain and cities
		public bool Mask { get; set; } // terrain not explored
		public bool Visible { get; set; } // hide cells not visible to allied units
		public int Overlay { get; set; } // -1 = no overlay, 0-n = terrain overlay
		private static Random RandomNumberGenerator = new Random(DateTime.Now.Millisecond);
		public bool Blocked(int unitType)
		{
			if (Terrain == 6 || Terrain == 9) // add any blockable terrain number to this list
			{
				return true;
			}
			else if (Terrain == 7 && unitType == 2)
			{
				// tanks cannot penetrate forests
				return true;
			}
			else
			{
				return false;
			}
		}

		public GameMap()
		{
			//Terrain = 3 + RandomNumberGenerator.Next() % 3; // use random grass background
			Terrain = 5;
			Mask = false;
			Visible = false;
			Overlay = -1;
		}

		public string DrawTerrain(int piX, int piY)
		{
			double lnX = (15.75 + 39) * piX;
			double lnY = 31.25 * (piX % 2) + piY * (31.25 * 2);

			string hexPngName = "grass_background_hex";
			switch (Terrain)
			{
				case 0:
					hexPngName = "grass_background_hex";
					break;
				case 1:
					hexPngName = "city_hex";
					break;
				case 2:
					hexPngName = "terrain_grass_01";
					break;
				case 3:
					hexPngName = "terrain_grass_02";
					break;
				case 4:
					hexPngName = "terrain_grass_03";
					break;
				case 5:
					hexPngName = "terrain_grass_04";
					break;
				case 6:
					hexPngName = "mountains_01";
					break;
				case 7:
					hexPngName = "forest_01";
					break;
				case 8:
					hexPngName = "beach_01";
					break;
				case 9:
					hexPngName = "ocean_01";
					break;
			}

			return "<image xlink:href='/Content/img/" + hexPngName + ".png' x='" + lnX + "' y='" + lnY + "' width='71' height='63' />";
		}
	}
}
