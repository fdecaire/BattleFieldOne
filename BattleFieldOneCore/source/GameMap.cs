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
		public bool Blocked
		{
			get
			{
				if (Terrain == 6) // add any blockable terrain number to this list
				{
					return true;
				}
				else
				{
					return false;
				}
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
	}
}
