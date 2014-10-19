using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BattleFieldOneCore;

namespace BattleFieldOne.Tests
{
	[TestClass]
	public class BoardTests
	{
		[TestMethod]
		public void TestBoardInitialize()
		{
			GameBoard gameBoard = new GameBoard();
			gameBoard.InitializeBoard(10, 20);

			Assert.AreEqual(gameBoard.MaxX, 10);
			Assert.AreEqual(gameBoard.MaxY, 20);
			Assert.IsTrue(gameBoard.Map[9, 19].Terrain > 2 && gameBoard.Map[9, 19].Terrain < 6);
		}

		[TestMethod]
		public void TestGBoardResize()
		{
			GameBoard gameBoard = new GameBoard();
			gameBoard.InitializeBoard(2, 2);

			gameBoard.InitializeBoard(15, 10);

			Assert.AreEqual(gameBoard.MaxX, 15);
			Assert.AreEqual(gameBoard.MaxY, 10);
			Assert.IsTrue(gameBoard.Map[14, 9].Terrain > 2 && gameBoard.Map[14, 9].Terrain < 6);
		}

		[TestMethod]
		public void TestViewMapRegion()
		{
			GameBoard gameBoard = new GameBoard();
			gameBoard.InitializeBoard(10, 20);
			gameBoard.ViewkMapRegion(3, 3);

			Assert.IsTrue(gameBoard.Map[3, 3].Visible); //center
			Assert.IsTrue(gameBoard.Map[2, 3].Visible); //left
			Assert.IsTrue(gameBoard.Map[3, 2].Visible); //above
			Assert.IsTrue(gameBoard.Map[4, 3].Visible); //right
			Assert.IsTrue(gameBoard.Map[3, 4].Visible); //below

			for (int x = 0; x < 10; x++)
			{
				for (int y = 0; y < 20; y++)
				{
					if (x == 3 && y == 3)
					{
						continue;
					}
					if (x == 2 && y == 3)
					{
						continue;
					}
					if (x == 3 && y == 2)
					{
						continue;
					} if (x == 4 && y == 3)
					{
						continue;
					} if (x == 3 && y == 4)
					{
						continue;
					} 
					Assert.IsFalse(gameBoard.Map[2, 2].Visible); 
				}
			}
		}

		[TestMethod]
		public void FindAdjacentCellsTopLeftCornerTest()
		{
			GameBoard gameBoard = new GameBoard();
			gameBoard.InitializeBoard(7, 6);

			// build a wall of mountains
			for (int i = 0; i < 4; i++)
			{
				gameBoard.Map[3, i + 1].Terrain = 6;
			}

			List<MapCoordinates> surroundingCells = gameBoard.FindAdjacentCells(0, 0, 2).OrderBy(u => u.X).ThenBy(u => u.Y).ToList();

			Assert.AreEqual(2, surroundingCells.Count);
			Assert.AreEqual(0, surroundingCells[0].X);
			Assert.AreEqual(1, surroundingCells[0].Y);
			Assert.AreEqual(1, surroundingCells[1].X);
			Assert.AreEqual(0, surroundingCells[1].Y);
		}

		[TestMethod]
		public void FindAdjacentCellsLeftSideTest()
		{
			GameBoard gameBoard = new GameBoard();
			gameBoard.InitializeBoard(7, 6);

			// build a wall of mountains
			for (int i = 0; i < 4; i++)
			{
				gameBoard.Map[3, i + 1].Terrain = 6;
			}

			List<MapCoordinates> surroundingCells = gameBoard.FindAdjacentCells(0, 3, 2).OrderBy(u => u.X).ThenBy(u => u.Y).ToList();

			Assert.AreEqual(4, surroundingCells.Count);
			Assert.AreEqual(0, surroundingCells[0].X);
			Assert.AreEqual(2, surroundingCells[0].Y);
			Assert.AreEqual(0, surroundingCells[1].X);
			Assert.AreEqual(4, surroundingCells[1].Y);
			Assert.AreEqual(1, surroundingCells[2].X);
			Assert.AreEqual(2, surroundingCells[2].Y);
			Assert.AreEqual(1, surroundingCells[3].X);
			Assert.AreEqual(3, surroundingCells[3].Y);
		}

		[TestMethod]
		public void FindAdjacentCellsNearUnpassibleTerrainTest()
		{
			GameBoard gameBoard = new GameBoard();
			gameBoard.InitializeBoard(7, 6);

			// build a wall of mountains
			for (int i = 0; i < 4; i++)
			{
				gameBoard.Map[3, i + 1].Terrain = 6;
			}

			List<MapCoordinates> surroundingCells = gameBoard.FindAdjacentCells(2, 3, 2).OrderBy(u => u.X).ThenBy(u => u.Y).ToList();

			Assert.AreEqual(4, surroundingCells.Count);
			Assert.AreEqual(1, surroundingCells[0].X);
			Assert.AreEqual(2, surroundingCells[0].Y);
			Assert.AreEqual(1, surroundingCells[1].X);
			Assert.AreEqual(3, surroundingCells[1].Y);
			Assert.AreEqual(2, surroundingCells[2].X);
			Assert.AreEqual(2, surroundingCells[2].Y);
			Assert.AreEqual(2, surroundingCells[3].X);
			Assert.AreEqual(4, surroundingCells[3].Y);
		}

		[TestMethod]
		public void FindAdjacentCellsCenterMapTest()
		{
			GameBoard gameBoard = new GameBoard();
			gameBoard.InitializeBoard(7, 6);

			// build a wall of mountains
			for (int i = 0; i < 4; i++)
			{
				gameBoard.Map[3, i + 1].Terrain = 6;
			}

			List<MapCoordinates> surroundingCells = gameBoard.FindAdjacentCells(1, 3, 2).OrderBy(u => u.X).ThenBy(u => u.Y).ToList();

			Assert.AreEqual(6, surroundingCells.Count);
			Assert.AreEqual(0, surroundingCells[0].X);
			Assert.AreEqual(3, surroundingCells[0].Y);
			Assert.AreEqual(0, surroundingCells[1].X);
			Assert.AreEqual(4, surroundingCells[1].Y);
			Assert.AreEqual(1, surroundingCells[2].X);
			Assert.AreEqual(2, surroundingCells[2].Y);
			Assert.AreEqual(1, surroundingCells[3].X);
			Assert.AreEqual(4, surroundingCells[3].Y);
			Assert.AreEqual(2, surroundingCells[4].X);
			Assert.AreEqual(3, surroundingCells[4].Y);
			Assert.AreEqual(2, surroundingCells[5].X);
			Assert.AreEqual(4, surroundingCells[5].Y);
		}
	}
}
