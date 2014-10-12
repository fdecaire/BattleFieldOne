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
	public class DieTest
	{
		[TestInitialize]
		public void Initialize()
		{
			UnitTestHelpers.ClearDieRoll();
		}

		[TestMethod]
		public void TestDieRoll()
		{
			UnitTestHelpers.SetDieRoll = 5;
			UnitTestHelpers.SetDieRoll = 2;
			UnitTestHelpers.SetDieRoll = 7;

			int result = DieRoller.DieRoll();
			Assert.AreEqual(5, result);

			result = DieRoller.DieRoll();
			Assert.AreEqual(2, result);

			result = DieRoller.DieRoll();
			Assert.AreEqual(7, result);
		}

		[TestMethod]
		public void TestDieReset()
		{
			UnitTestHelpers.SetDieRoll = 5;
			UnitTestHelpers.SetDieRoll = 3;
			UnitTestHelpers.SetDieRoll = 6;

			DieRoller.DieRoll();
			DieRoller.DieRoll();

			UnitTestHelpers.ClearDieRoll();
			UnitTestHelpers.SetDieRoll = 3;

			int result = DieRoller.DieRoll();

			Assert.AreEqual(3, result);
		}

		[TestMethod]
		public void TestDieWrapAround()
		{
			UnitTestHelpers.SetDieRoll = 6;
			UnitTestHelpers.SetDieRoll = 2;

			var result = DieRoller.DieRoll();
			Assert.AreEqual(6, result);

			result = DieRoller.DieRoll();
			Assert.AreEqual(2, result);

			result = DieRoller.DieRoll();
			Assert.AreEqual(6, result);
		}
	}
}
