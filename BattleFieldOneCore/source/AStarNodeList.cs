using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleFieldOneCore
{
	public class AStarNodeList
	{
		private List<AStarNode> Items = new List<AStarNode>();

		public int Count
		{
			get
			{
				return Items.Count;
			}
		}

		public void Push(AStarNode node)
		{
			Items.Add(node);
		}

		public AStarNode Pop()
		{
			if (Items.Count > 0)
			{
				AStarNode node = Items[Items.Count - 1];
				Items.RemoveAt(Items.Count - 1);
				return node;
			}
			return null;
		}

		public AStarNode FindSmallestNode()
		{
			// find the smallest node and remove from list, return node
			int smallestNumber = int.MaxValue;
			int smallestNodeNumber=-1;
			for (int i = 0; i < Items.Count; i++)
			{
				if (Items[i].F < smallestNumber)
				{
					smallestNumber = Items[i].F;
					smallestNodeNumber = i;
				}
			}

			if (smallestNodeNumber > -1)
			{
				AStarNode node = Items[smallestNodeNumber];
				Items.RemoveAt(smallestNodeNumber);
				return node;
			}

			return null;
		}

		public bool Contains(int X, int Y)
		{
			return (Items.FindIndex(t => t.X == X && t.Y == Y) > -1);
		}

		public void UpdateNodeIfBetter(AStarNode node)
		{
			// if the node passed in has a better "G" rating, then replace the old node
			int index = Items.FindIndex(t => t.X == node.X && t.Y == node.Y);
			if (index > -1)
			{
				if (node.G < Items[index].G)
				{
					Items[index].G = node.G;
					Items[index].H = node.H;
					Items[index].Source.X = node.Source.X;
					Items[index].Source.Y = node.Source.Y;
				}
			}
		}

		public AStarNode GetNode(int X, int Y)
		{
			int index = Items.FindIndex(t => t.X == X && t.Y == Y);
			if (index > -1)
			{
				return Items[index];
			}
			return null;
		}

		public void Clear()
		{
			Items.Clear();
		}
	}
}
