using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using log4net;

namespace BattleFieldOneCore
{
	public class ShortestPath
	{
		private AStarNodeList OpenList = new AStarNodeList();
		private AStarNodeList ClosedList = new AStarNodeList();
		public List<Point> WayPoint = new List<Point>(); // final list of waypoints
		private GameBoard gameBoard;
		private int StartX;
		private int StartY;
		private int EndX;
		private int EndY;
		private int Iterations = 0; // if we go through too many iterations, then assume no path possible
		private ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public ShortestPath(int startX, int startY, int endX, int endY, GameBoard gameBoard)
		{
			this.gameBoard = gameBoard;
			EndX = endX;
			EndY = endY;
			StartX = startX;
			StartY = startY;

			// push the starting cell
			AStarNode currentNode = new AStarNode(startX, startY, startX, startY, EndX, EndY, 0);
			OpenList.Push(currentNode);

			FindPath();
		}

		private void FindPath()
		{
			Iterations++;

			if (WayPoint.Count > 0)
			{
				return;
			}

			if (Iterations > 50)
			{
				return;
			}

			// pop the top item off the openlist and find surrounding cells
			AStarNode node = OpenList.Pop();

			//log.DebugFormat("FindPath({0},{1})", node.X, node.Y);

			ClosedList.Push(node);

			List<MapCoordinates> surroundingCells = gameBoard.FindAdjacentCells(node.X, node.Y);

			for (int i = 0; i < surroundingCells.Count; i++)
			{
				//log.DebugFormat("{0},{1}", surroundingCells[i].X, surroundingCells[i].Y);

				// skip any nodes that are already in the closed list
				if (!ClosedList.Contains(surroundingCells[i].X, surroundingCells[i].Y))
				{
					if (OpenList.Contains(surroundingCells[i].X, surroundingCells[i].Y))
					{
						// check to see if this path is shorter than the one on the open list, if so, then update it, otherwise skip
						AStarNode tempNode = new AStarNode(node.X, node.Y, surroundingCells[i].X, surroundingCells[i].Y, EndX, EndY, node.G + 1);

						OpenList.UpdateNodeIfBetter(tempNode);
					}
					else
					{
						OpenList.Push(new AStarNode(node.X, node.Y, surroundingCells[i].X, surroundingCells[i].Y, EndX, EndY, node.G + 1));
					}
				}
			}

			AStarNode smallestNode = OpenList.FindSmallestNode();

			if (smallestNode == null)
			{
				return;
			}

			// check if this is the destination node
			if (smallestNode.X == EndX && smallestNode.Y == EndY)
			{
				// consolidate the actual path into the WayPoint list
				WayPoint.Add(new Point(EndX, EndY));
				
				// walk back to the starting point
				AStarNode tempNode = ClosedList.GetNode(smallestNode.Source.X, smallestNode.Source.Y);
				while (tempNode.X != StartX || tempNode.Y != StartY)
				{
					WayPoint.Insert(0, new Point(tempNode.X, tempNode.Y));
					tempNode = ClosedList.GetNode(tempNode.Source.X, tempNode.Source.Y);
				}

				// clear the open and closed lists
				OpenList.Clear();
				ClosedList.Clear();
				return;
			}

			OpenList.Push(smallestNode);
			FindPath();
		}

		public MapCoordinates GetNextWaypoint(int X, int Y)
		{
			if (WayPoint.Count > 0)
			{
				// if there are waypoints set, then use those first
				Point point = WayPoint[0];
				if (X == point.X && Y == point.Y)
				{
					WayPoint.RemoveAt(0);

					if (WayPoint.Count > 0)
					{
						point = WayPoint[0];
						MapCoordinates mapCoordinates = new MapCoordinates(point.X, point.Y);
						return mapCoordinates;
					}
					else
					{
						return null;
					}
				}
				else
				{
					point = WayPoint[0];
					MapCoordinates mapCoordinates = new MapCoordinates(point.X, point.Y);
					return mapCoordinates;
				}
			}

			return null;
		}
	}
}
