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

		public GameMap()
		{
			Terrain = 0;
			Mask = false;
			Visible = false;
		}
	}
}
