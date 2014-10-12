using Microsoft.VisualStudio.TestTools.UnitTesting;
using BattleFieldOneCore;
using UnitTestHelpersNS;

namespace BattleFieldOne.Tests
{
	[TestClass]
	public class GameClassTests
	{
		[TestInitialize]
		public void Initialize()
		{
			UnitTestHelpers.ClearDieRoll();
		}

		[TestMethod]
		public void CollectGermanAttackDataUnitOnCity()
		{
			// attack unit on city square first
			UnitTestHelpers.SetDieRoll = 6;
			UnitTestHelpers.SetDieRoll = 6;
			UnitTestHelpers.SetDieRoll = 5;

			GameClass gameClass = new GameClass();
			gameClass.InitializeCustomGame(5, 5);
			gameClass.gameBoard.Map[2, 2].Terrain = 1;

			gameClass.AllUnits.AddUnit(1, NATIONALITY.Allied, 2, 2); // allied infantry
			gameClass.AllUnits.AddUnit(1, NATIONALITY.Allied, 1, 2); // allied infantry
			gameClass.AllUnits.AddUnit(2, NATIONALITY.German, 1, 1); // german tank

			gameClass.RecomputeMapMask();
			gameClass.RecomputeMapView();

			gameClass.SetEnemyStrategy();

			var result = gameClass.CollectGermanAttackData();

			// test to see if the correct unit has be eliminated
			// german unit #3 attacks allied unit #1, EnemyDestroyed = 1
			Assert.AreEqual("3,1,1", result);
		}

		[TestMethod]
		public void CollectGermanAttackDataUnitOnCity2()
		{
			// attack unit on city square first (allied units reversed)
			UnitTestHelpers.SetDieRoll = 6;
			UnitTestHelpers.SetDieRoll = 6;
			UnitTestHelpers.SetDieRoll = 5;

			GameClass gameClass = new GameClass();
			gameClass.InitializeCustomGame(5, 5);
			gameClass.gameBoard.Map[2, 2].Terrain = 1;

			gameClass.AllUnits.AddUnit(1, NATIONALITY.Allied, 1, 2); // allied infantry
			gameClass.AllUnits.AddUnit(1, NATIONALITY.Allied, 2, 2); // allied infantry
			gameClass.AllUnits.AddUnit(2, NATIONALITY.German, 1, 1); // german tank

			gameClass.RecomputeMapMask();
			gameClass.RecomputeMapView();

			gameClass.SetEnemyStrategy();

			var result = gameClass.CollectGermanAttackData();

			// test to see if the correct unit has be eliminated
			// german unit #3 attacks allied unit #2, EnemyDestroyed = 1
			Assert.AreEqual("3,2,1", result);
		}

		[TestMethod]
		public void CollectGermanAttackDataLowestDefenseFirst()
		{
			// attack unit that has lower defense point value first
			// attack unit on city square first
			UnitTestHelpers.SetDieRoll = 6;
			UnitTestHelpers.SetDieRoll = 6;
			UnitTestHelpers.SetDieRoll = 5;

			GameClass gameClass = new GameClass();
			gameClass.InitializeCustomGame(5, 5);

			gameClass.AllUnits.AddUnit(2, NATIONALITY.Allied, 2, 2); // allied tank
			gameClass.AllUnits.AddUnit(1, NATIONALITY.Allied, 1, 2); // allied infantry (attack first)
			gameClass.AllUnits.AddUnit(2, NATIONALITY.German, 1, 1); // german tank

			gameClass.RecomputeMapMask();
			gameClass.RecomputeMapView();

			gameClass.SetEnemyStrategy();

			var result = gameClass.CollectGermanAttackData();

			// test to see if the correct unit has be eliminated
			// german unit #3 attacks allied unit #2, EnemyDestroyed = 1
			Assert.AreEqual("3,2,1", result);
		}

		[TestMethod]
		public void CollectGermanAttackDataLowestDefenseFirst2()
		{
			// attack unit that has lower defense point value first (allied units reversed)
			// attack unit on city square first
			UnitTestHelpers.SetDieRoll = 6;
			UnitTestHelpers.SetDieRoll = 6;
			UnitTestHelpers.SetDieRoll = 5;

			GameClass gameClass = new GameClass();
			gameClass.InitializeCustomGame(5, 5);

			gameClass.AllUnits.AddUnit(2, NATIONALITY.Allied, 1, 2); // allied tank
			gameClass.AllUnits.AddUnit(1, NATIONALITY.Allied, 2, 2); // allied infantry (attack first)
			gameClass.AllUnits.AddUnit(2, NATIONALITY.German, 1, 1); // german tank

			gameClass.RecomputeMapMask();
			gameClass.RecomputeMapView();

			gameClass.SetEnemyStrategy();

			var result = gameClass.CollectGermanAttackData();

			// test to see if the correct unit has be eliminated
			// german unit #3 attacks allied unit #2, EnemyDestroyed = 1
			Assert.AreEqual("3,2,1", result);
		}
	}
}
