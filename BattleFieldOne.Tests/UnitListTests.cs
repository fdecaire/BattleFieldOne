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
	public class UnitListTests
	{
		[TestMethod]
		public void TestTotalGermanUnits()
		{
			UnitsList unitsList = new UnitsList();

			unitsList.Items.Add(new UnitClass(1, NATIONALITY.German, 6, 7, 0));
			unitsList.Items.Add(new UnitClass(1, NATIONALITY.Allied, 6, 8, 1));
			unitsList.Items.Add(new UnitClass(1, NATIONALITY.German, 6, 7, 3));
			unitsList.Items.Add(new UnitClass(1, NATIONALITY.Allied, 6, 8, 4));
			unitsList.Items.Add(new UnitClass(1, NATIONALITY.German, 6, 7, 5));
			unitsList.Items.Add(new UnitClass(1, NATIONALITY.German, 6, 8, 6));

			Assert.AreEqual(unitsList.TotalGermanUnits, 4);
		}

		[TestMethod]
		public void TestTotalGermanUnitsNoGermanUnits()
		{
			UnitsList unitsList = new UnitsList();

			unitsList.Items.Add(new UnitClass(1, NATIONALITY.Allied, 6, 8, 0));
			unitsList.Items.Add(new UnitClass(1, NATIONALITY.Allied, 6, 12, 1));

			Assert.AreEqual(unitsList.TotalGermanUnits, 0);
		}

		[TestMethod]
		public void TestTotalGermanUnitsEmptyList()
		{
			UnitsList unitsList = new UnitsList();

			Assert.AreEqual(unitsList.TotalGermanUnits, 0);
		}

		[TestMethod]
		public void TestFindClosestGermanUnit()
		{
			UnitsList unitsList = new UnitsList();
			unitsList.Items.Add(new UnitClass(1, NATIONALITY.German, 1, 1, 0));
			unitsList.Items.Add(new UnitClass(1, NATIONALITY.German, 8, 8, 1));

			Assert.AreEqual(unitsList.FindClosestGermanUnit(2, 2), 0);
		}

		[TestMethod]
		public void TestMapOccupied()
		{
			UnitsList unitsList = new UnitsList();

			unitsList.Items.Add(new UnitClass(1, NATIONALITY.Allied, 6, 8, 0));
			unitsList.Items.Add(new UnitClass(1, NATIONALITY.Allied, 6, 12, 1));

			Assert.AreEqual(unitsList.MapOccupied(6,8), true);
		}

		[TestMethod]
		public void TestMapNotOccupied()
		{
			UnitsList unitsList = new UnitsList();

			unitsList.Items.Add(new UnitClass(1, NATIONALITY.Allied, 6, 8, 0));
			unitsList.Items.Add(new UnitClass(1, NATIONALITY.Allied, 6, 12, 1));

			Assert.AreEqual(unitsList.MapOccupied(6, 9), false);
		}

		[TestMethod]
		public void TestFindUnitIndexFromNumber()
		{
			UnitsList unitsList = new UnitsList();

			unitsList.Items.Add(new UnitClass(1, NATIONALITY.Allied, 6, 8, 22));
			unitsList.Items.Add(new UnitClass(1, NATIONALITY.Allied, 6, 12, 65));

			Assert.AreEqual(unitsList.FindUnitIndexFromNumber(22), 0);
			Assert.AreEqual(unitsList.FindUnitIndexFromNumber(65), 1);
			Assert.AreEqual(unitsList.FindUnitIndexFromNumber(3), -1);
		}
	}
}
