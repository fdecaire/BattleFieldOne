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
			Assert.AreEqual(gameBoard.Map[9, 19].Terrain, 0);
		}

		[TestMethod]
		public void TestGBoardResize()
		{
			GameBoard gameBoard = new GameBoard();
			gameBoard.InitializeBoard(2, 2);

			gameBoard.InitializeBoard(15, 10);

			Assert.AreEqual(gameBoard.MaxX, 15);
			Assert.AreEqual(gameBoard.MaxY, 10);
			Assert.AreEqual(gameBoard.Map[14, 9].Terrain, 0);
		}

		[TestMethod]
		public void TestViewMapRegion()
		{
			GameBoard gameBoard = new GameBoard();
			gameBoard.InitializeBoard(10, 20);
			gameBoard.ViewkMapRegion(3, 3);

			Assert.AreEqual(gameBoard.Map[3, 3].Visible, true); //center
			Assert.AreEqual(gameBoard.Map[2, 3].Visible, true); //left
			Assert.AreEqual(gameBoard.Map[3, 2].Visible, true); //above
			Assert.AreEqual(gameBoard.Map[4, 3].Visible, true); //right
			Assert.AreEqual(gameBoard.Map[3, 4].Visible, true); //below

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
					Assert.AreEqual(gameBoard.Map[2, 2].Visible, false); 
				}
			}
		}
	}
}
