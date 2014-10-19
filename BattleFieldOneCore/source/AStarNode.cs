using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace BattleFieldOneCore
{
	public class AStarNode
	{
		public int F
		{
			get
			{
				return G + H;
			}
		}
		public int G { get; set; }
		public int H { get; set; }
		public int X { get; private set; }
		public int Y { get; private set; }
		public Point Source;

		public AStarNode(int sourceX, int sourceY, int X, int Y, int endX, int endY, int distance)
		{
			this.X = X;
			this.Y = Y;
			Source.X = sourceX;
			Source.Y = sourceY;

			G = distance;
			H = Distance(X, Y, endX, endY);
		}

		private int Distance(int X, int Y, int endX, int endY)
		{
			return (int)Math.Sqrt(Math.Pow((X - endX), 2) + Math.Pow((Y - endY), 2));
		}
	}
}
