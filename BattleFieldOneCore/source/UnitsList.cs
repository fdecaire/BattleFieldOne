using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleFieldOneCore
{
	public class UnitsList
	{
		public List<UnitClass> Items = new List<UnitClass>();

		public void AddUnit(int unitType, NATIONALITY nationality, int x, int y)
		{
			Items.Add(new UnitClass(unitType, nationality, x, y, Items.Count + 1));
		}

		public int TotalGermanUnits
		{
			get
			{
				int liTotal = 0;

				for (int i = 0; i < Items.Count; i++)
				{
					if (Items[i].Nationality == NATIONALITY.German)
						liTotal++;
				}

				return liTotal;
			}
		}

		public int FindClosestGermanUnit(int X, int Y)
		{
			double liDistance = double.MaxValue; // just make sure this is bigger than the map board
			int liClosestUnit = -1;

			for (int i = 0; i < Items.Count; i++)
			{
				if (Items[i].Nationality == NATIONALITY.German && Items[i].Command == UNITCOMMAND.None)
				{
					double liTempDistance = BattleFieldOneCommonObjects.Distance(X, Y, Items[i].X, Items[i].Y);
					if (liTempDistance < liDistance)
					{
						liDistance = liTempDistance;
						liClosestUnit = i;
					}
				}
			}

			return liClosestUnit;
		}

		public bool MapOccupied(int X, int Y)
		{
			for (int i = 0; i < Items.Count; i++)
			{
				if (Items[i].X == X && Items[i].Y == Y)
					return true;
			}

			return false;
		}

		public int FindUnitIndexFromNumber(int unitNumber)
		{
			for (int i = 0; i < Items.Count; i++)
			{
				if (unitNumber == Items[i].Number && Items[i].Destroyed == false)
					return i;
			}

			return -1;
		}

		public void ResetUnitAttackPerTernMarker()
		{
			foreach (var item in Items)
			{
				item.UnitHasAttackedThisTurn = false;
			}
		}

		public List<int> LocateAllAdjacentAlliedUnits(int X, int Y)
		{
			List<int> AlliedUnitList = new List<int>();

			// find an allied unit that is in an adjacent hex to pix,piy
			for (int i = 0; i < Items.Count; i++)
			{
				if (!Items[i].Destroyed)
				{
					if (Items[i].Nationality == NATIONALITY.Allied)
					{
						if (X == Items[i].X)
						{
							if (Y == Items[i].Y - 1 || Y == Items[i].Y + 1)
							{
								AlliedUnitList.Add(i);
							}
						}
						else if (X == Items[i].X - 1 || X == Items[i].X + 1)
						{
							if (X % 2 == 1)
							{
								// odd column (y-1,y) for pix-1 and pix+1
								if (Y == Items[i].Y - 1 || Y == Items[i].Y)
								{
									AlliedUnitList.Add(i);
								}
							}
							else
							{
								// even column (y,y+1) for pix-1 and pix+1
								if (Y == Items[i].Y || Y == Items[i].Y + 1)
								{
									AlliedUnitList.Add(i);
								}
							}
						}
					}
				}
			}

			return AlliedUnitList;
		}

		public int LocateAlliedUnitLowestDefense(List<int> alliedUnitList)
		{
			int lowestUnitDefenseNumber = 9000;
			int lowestUnitIndex = -1;

			foreach (var UnitIndex in alliedUnitList)
			{
				if (Items[UnitIndex].Defense < lowestUnitDefenseNumber)
				{
					lowestUnitDefenseNumber = Items[UnitIndex].Defense;
					lowestUnitIndex = UnitIndex;
				}
			}

			return lowestUnitIndex;
		}
	}
}
