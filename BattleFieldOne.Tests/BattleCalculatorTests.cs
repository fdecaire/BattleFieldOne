using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BattleFieldOneCore;
using UnitTestHelpersNS;

namespace BattleFieldOne.Tests
{
	[TestClass]
	public class BattleCalculatorTests
	{
		[TestInitialize]
		public void Initialize()
		{
			UnitTestHelpers.ClearDieRoll();
		}

		[TestMethod]
		public void TestBattleResultDoubleDamaged()
		{
			UnitTestHelpers.SetDieRoll = 5;
			UnitTestHelpers.SetDieRoll = 2;
			UnitTestHelpers.SetDieRoll = 6;

			var result = BattleCalculator.Result(6);

			Assert.AreEqual(BATTLERESULT.EnemyDoubleDamaged, result);
		}

		[TestMethod]
		public void TestBattleResultDamaged()
		{
			UnitTestHelpers.SetDieRoll = 2;
			UnitTestHelpers.SetDieRoll = 2;
			UnitTestHelpers.SetDieRoll = 4;

			var result = BattleCalculator.Result(6);

			Assert.AreEqual(BATTLERESULT.EnemyDamaged, result);
		}

		[TestMethod]
		public void TestBattleResultNone()
		{
			UnitTestHelpers.SetDieRoll = 1;
			UnitTestHelpers.SetDieRoll = 1;
			UnitTestHelpers.SetDieRoll = 1;

			var result = BattleCalculator.Result(6);

			Assert.AreEqual(BATTLERESULT.None, result);
		}

		[TestMethod]
		public void TestBattleResultSingleDamageUnit1()
		{
			UnitTestHelpers.SetDieRoll = 6;
			UnitTestHelpers.SetDieRoll = 6;
			UnitTestHelpers.SetDieRoll = 6;

			var result = BattleCalculator.Result(1);

			Assert.AreEqual(BATTLERESULT.EnemyDamaged, result);
		}
	}
}
