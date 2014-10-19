using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BattleFieldOneCore;
using System.Drawing;

namespace BattleFieldOne.Tests
{
	[TestClass]
	public class PathFindingTests
	{
		[TestMethod]
		public void TestAStarPathAlgorithm()
		{
			GameBoard gameBoard = new GameBoard();
			gameBoard.InitializeBoard(7, 6);

			// build a wall of mountains
			for (int i = 0; i < 4; i++)
			{
				gameBoard.Map[3, i + 1].Terrain = 6;
			}

			// add allied and german units
			UnitClass unit = new UnitClass(1, NATIONALITY.German, 0, 3, 1);

			// add destination city
			gameBoard.Map[6, 3].Terrain = 1;

			unit.Command = UNITCOMMAND.Destination;
			unit.DestX = 6;
			unit.DestY = 3;

			unit.ComputePath(gameBoard);

			Assert.AreEqual(8, unit.Path.WayPoint.Count);
			Assert.AreEqual(new Point(1, 3), unit.Path.WayPoint[0]);
			Assert.AreEqual(new Point(2, 4), unit.Path.WayPoint[1]);
			Assert.AreEqual(new Point(2, 5), unit.Path.WayPoint[2]);
			Assert.AreEqual(new Point(3, 5), unit.Path.WayPoint[3]);
			Assert.AreEqual(new Point(4, 5), unit.Path.WayPoint[4]);
			Assert.AreEqual(new Point(5, 4), unit.Path.WayPoint[5]);
			Assert.AreEqual(new Point(5, 3), unit.Path.WayPoint[6]);
			Assert.AreEqual(new Point(6, 3), unit.Path.WayPoint[7]);
		}
	}
}
