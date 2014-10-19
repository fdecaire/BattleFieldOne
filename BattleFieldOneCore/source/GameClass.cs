using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using UnitTestHelpersNS;
using System.Drawing;
using log4net;

//TODO: there is a bug that resets the pieces moved counters when the browser is refreshed allowing the user to move anywhere in one turn.
//TODO: random terrain squares
//TODO: roads
//TODO: random city locations
//TODO: add reinforcements according to cities captured
//TODO: save/restore game from file saved on pc
//TODO: enhance enemy attack to choose attacking piece more wisely (unit on city, or lowest defense first)

namespace BattleFieldOneCore
{
	public class GameClass
	{
		public UnitsList AllUnits = new UnitsList();
		public GameBoard gameBoard = new GameBoard();

		public GameClass()
		{
			log4net.Config.XmlConfigurator.Configure();
		}

		public void RecomputeMapMask()
		{
			// spin through all units and unmask + view areas where units are sitting
			for (int i = 0; i < AllUnits.Items.Count; i++)
			{
				if (AllUnits.Items[i].Nationality == NATIONALITY.Allied)
				{
					// unmask and view all cells surrounding this x,y position
					gameBoard.UnmaskMapRegion(AllUnits.Items[i].X, AllUnits.Items[i].Y);
				}
			}
		}

		public string CheckForEndOfGameCondition()
		{
			string lsResult = "";

			// check to see if german units occupy all 4 cities
			int liTotal = 0;
			for (int i = 0; i < AllUnits.Items.Count; i++)
			{
				if (AllUnits.Items[i].Nationality == NATIONALITY.German)
				{
					if (gameBoard.Map[AllUnits.Items[i].X, AllUnits.Items[i].Y].Terrain == 1)
					{
						liTotal++;
					}
				}
			}

			if (liTotal == gameBoard.TotalCities)
			{ 
				return "Germany Captured All Cities!"; 
			}

			// check to see if allied units occupy all cities
			liTotal = 0;
			for (int i = 0; i < AllUnits.Items.Count; i++)
			{
				if (AllUnits.Items[i].Nationality == NATIONALITY.Allied)
				{
					if (gameBoard.Map[AllUnits.Items[i].X, AllUnits.Items[i].Y].Terrain == 1)
					{
						liTotal++;
					}
				}
			}

			if (liTotal == gameBoard.TotalCities)
			{
				return "Allies Captured All Cities!";
			}

			// check to see if all german units destroyed
			liTotal = 0;
			for (int i = 0; i < AllUnits.Items.Count; i++)
			{
				if (AllUnits.Items[i].Nationality == NATIONALITY.German)
				{
					liTotal++;
				}
			}

			if (liTotal == 0)
			{
				return "All German Units Destroyed!";
			}

			// check to see if all allied units destroyed
			liTotal = 0;
			for (int i = 0; i < AllUnits.Items.Count; i++)
			{
				if (AllUnits.Items[i].Nationality == NATIONALITY.Allied)
				{
					liTotal++;
				}
			}

			if (liTotal == 0)
			{
				return "All Allied Units Destroyed!";
			}

			return lsResult;
		}

		public string CollectGermanAttackData()
		{
			string lsResult = "";

			for (int liGermanUnitIndex = 0; liGermanUnitIndex < AllUnits.Items.Count; liGermanUnitIndex++)
			{
				if (AllUnits.Items[liGermanUnitIndex].Nationality == NATIONALITY.German && AllUnits.Items[liGermanUnitIndex].Destroyed == false)
				{
					// check for a unit that is one hex away and is allied
					List<int> AlliedUnitList = AllUnits.LocateAllAdjacentAlliedUnits(AllUnits.Items[liGermanUnitIndex].X, AllUnits.Items[liGermanUnitIndex].Y);
					if (AlliedUnitList.Count > 0)
					{
						int AlliedUnitIndex = AlliedUnitList[0]; // default to the first in the list 

						// attack unit on city terrain - first
						int AlliedUnitOnCityTerrain = LocateAlliedUnitOnCityTerrain(AlliedUnitList);

						if (AlliedUnitOnCityTerrain > -1)
						{
							AlliedUnitIndex = AlliedUnitOnCityTerrain;
						}
						else
						{
							// find lowest defense unit - second
							int AlliedUnitLowestDefense = AllUnits.LocateAlliedUnitLowestDefense(AlliedUnitList);

							if (AlliedUnitLowestDefense > -1)
							{
								AlliedUnitIndex = AlliedUnitLowestDefense;
							}
						}

						if (lsResult != "")
							lsResult += "|";

						BATTLERESULT liBattleResult = BattleCalculator.Result(AllUnits.Items[liGermanUnitIndex].Offense);

						switch (liBattleResult)
						{
							case BATTLERESULT.EnemyDamaged:
								AllUnits.Items[AlliedUnitIndex].Defense--;
								if (AllUnits.Items[AlliedUnitIndex].Defense <= 0)
								{
									AllUnits.Items[AlliedUnitIndex].Destroyed = true;
									liBattleResult = BATTLERESULT.EnemyDestroyed;
								}
								break;
							case BATTLERESULT.EnemyDoubleDamaged:
								AllUnits.Items[AlliedUnitIndex].Defense -= 2;
								if (AllUnits.Items[AlliedUnitIndex].Defense <= 0)
								{
									AllUnits.Items[AlliedUnitIndex].Destroyed = true;
									liBattleResult = BATTLERESULT.EnemyDestroyed;
								}
								break;
							case BATTLERESULT.EnemyDestroyed:
								AllUnits.Items[AlliedUnitIndex].Destroyed = true;
								break;
							case BATTLERESULT.DefenderDestroyed:
								AllUnits.Items[liGermanUnitIndex].Destroyed = true;
								break;
						}

						// german unit # , allied unit # , result (0=none,1=allied destroyed,2=german destroyed)
						lsResult += AllUnits.Items[liGermanUnitIndex].Number + "," + AllUnits.Items[AlliedUnitIndex].Number + "," + (int)liBattleResult;
					}
				}
			}

			// remove items in the list that are marked as destroyed
			for (int i = AllUnits.Items.Count - 1; i > -1; i--)
			{
				if (AllUnits.Items[i].Destroyed)
				{
					AllUnits.Items.RemoveAt(i);
				}
			}

			return lsResult;
		}

		private int LocateAlliedUnitOnCityTerrain(List<int> AlliedUnitList)
		{
			foreach (var UnitIndex in AlliedUnitList)
			{
				if (gameBoard.Map[AllUnits.Items[UnitIndex].X, AllUnits.Items[UnitIndex].Y].Terrain == 1)
				{
					return UnitIndex;
				}
			}

			return -1;
		}

		public string CollectGermanMovementData()
		{
			// spin through the list and build a string of movement coordinates to send back to the javascript
			string lsResult = "";
			string lsDestinations = "";

			for (int i = 0; i < AllUnits.Items.Count; i++)
			{
				if (AllUnits.Items[i].Nationality == NATIONALITY.German)
				{
					if (AllUnits.Items[i].Command == UNITCOMMAND.Destination)
					{
						MapCoordinates lCoordinates = AllUnits.Items[i].Path.GetNextWaypoint(AllUnits.Items[i].X, AllUnits.Items[i].Y);

						if (lCoordinates == null)
						{
							// use the destination set, if there is one
							lCoordinates = FindNextMove(i);
						}

						// check to see if the map is occupied by another unit
						if (lCoordinates != null)
						{
							if (!AllUnits.MapOccupied(lCoordinates.X, lCoordinates.Y))
							{
								if (lsResult != "")
								{
									lsResult += "|";
								}

								// update the unit coordinates
								AllUnits.Items[i].X = lCoordinates.X;
								AllUnits.Items[i].Y = lCoordinates.Y;
								lsResult += "M," + AllUnits.Items[i].Number + "," + AllUnits.Items[i].X + "," + AllUnits.Items[i].Y;

								if (AllUnits.Items[i].X == AllUnits.Items[i].DestX && AllUnits.Items[i].Y == AllUnits.Items[i].DestY)
								{
									AllUnits.Items[i].Command = UNITCOMMAND.Wait;
								}
							}
							else
							{
								// keep unit in place for this turn, don't send back a move command
							}
						}

						lsDestinations += "|T," + AllUnits.Items[i].Number + "," + AllUnits.Items[i].DestX + "," + AllUnits.Items[i].DestY; //T=test destination data
					}
				}
			}

			if (gameBoard.TestMode)
			{
				lsResult += lsDestinations;
			}

			return lsResult;
		}

		private MapCoordinates FindNextMove(int unitNumber)
		{
			List<MapCoordinates> surroundingCells = gameBoard.FindAdjacentCells(AllUnits.Items[unitNumber].X, AllUnits.Items[unitNumber].Y, AllUnits.Items[unitNumber].UnitType);
			double shortestDistance = 9999;
			int shortestDistanceItem = -1;

			// choose the next closest cell between the unit and dest. 
			for (int i = 0; i < surroundingCells.Count; i++)
			{
				if (!AllUnits.MapOccupied(surroundingCells[i].X, surroundingCells[i].Y))
				{
					double distance = BattleFieldOneCommonObjects.Distance(surroundingCells[i].X, surroundingCells[i].Y, AllUnits.Items[unitNumber].DestX, AllUnits.Items[unitNumber].DestY);
					if (distance < shortestDistance)
					{
						shortestDistance = distance;
						shortestDistanceItem = i;
					}
				}
			}

			if (shortestDistanceItem > -1)
			{
				return surroundingCells[shortestDistanceItem];
			}

			return null;
		}

		public void InitializeCustomGame(int maxX, int maxY)
		{
			gameBoard.InitializeBoard(maxX, maxY);
		}

		public void InitializeGame(int gameType)
		{
			switch (gameType)
			{
				case 0:
					gameBoard.InitializeBoard(13, 10);
					gameBoard.Map[9, 0].Terrain = 1; // city 0 (upper right) 
					gameBoard.Map[2, 2].Terrain = 1; // city 1 (upper left)
					gameBoard.Map[11, 7].Terrain = 1; // city 2 (lower right)
					gameBoard.Map[4, 9].Terrain = 1; // city 3 (lower right)

					AllUnits.AddUnit(1, NATIONALITY.Allied, 6, 7);
					AllUnits.AddUnit(1, NATIONALITY.Allied, 6, 8);
					AllUnits.AddUnit(1, NATIONALITY.Allied, 6, 9);
					AllUnits.AddUnit(1, NATIONALITY.Allied, 7, 7);
					AllUnits.AddUnit(1, NATIONALITY.Allied, 7, 8);
					AllUnits.AddUnit(1, NATIONALITY.Allied, 7, 9);
					AllUnits.AddUnit(1, NATIONALITY.Allied, 8, 7);
					AllUnits.AddUnit(1, NATIONALITY.Allied, 8, 8);
					AllUnits.AddUnit(1, NATIONALITY.Allied, 8, 9);
					AllUnits.AddUnit(1, NATIONALITY.Allied, 9, 7);
					AllUnits.AddUnit(1, NATIONALITY.Allied, 9, 8);
					AllUnits.AddUnit(1, NATIONALITY.Allied, 9, 9);

					AllUnits.AddUnit(1, NATIONALITY.German, 4, 0);
					AllUnits.AddUnit(1, NATIONALITY.German, 4, 1);
					AllUnits.AddUnit(1, NATIONALITY.German, 4, 2);
					AllUnits.AddUnit(1, NATIONALITY.German, 5, 0);
					AllUnits.AddUnit(1, NATIONALITY.German, 5, 1);
					AllUnits.AddUnit(1, NATIONALITY.German, 5, 2);
					AllUnits.AddUnit(1, NATIONALITY.German, 6, 0);
					AllUnits.AddUnit(1, NATIONALITY.German, 6, 1);
					AllUnits.AddUnit(1, NATIONALITY.German, 6, 2);
					AllUnits.AddUnit(1, NATIONALITY.German, 7, 0);
					AllUnits.AddUnit(1, NATIONALITY.German, 7, 1);
					AllUnits.AddUnit(1, NATIONALITY.German, 7, 2);
					break;
				case 1:
					gameBoard.InitializeBoard(18, 15);
					gameBoard.Map[16, 0].Terrain = 1; // city 0 (upper right)
					gameBoard.Map[2, 2].Terrain = 1; // city 1 (upper left)
					gameBoard.Map[9, 7].Terrain = 1; // city 2 (center)
					gameBoard.Map[4, 6].Terrain = 1; // city 2 (center)
					gameBoard.Map[13, 5].Terrain = 1; // city 2 (center)
					gameBoard.Map[16, 12].Terrain = 1; // city 3 (lower right)
					gameBoard.Map[2, 13].Terrain = 1; // city 3 (lower left)


					AllUnits.AddUnit(1, NATIONALITY.Allied, 7, 12);
					AllUnits.AddUnit(1, NATIONALITY.Allied, 8, 12);
					AllUnits.AddUnit(1, NATIONALITY.Allied, 8, 13);
					AllUnits.AddUnit(1, NATIONALITY.Allied, 8, 14);
					AllUnits.AddUnit(2, NATIONALITY.Allied, 9, 12);
					AllUnits.AddUnit(2, NATIONALITY.Allied, 9, 13);
					AllUnits.AddUnit(2, NATIONALITY.Allied, 9, 14);
					AllUnits.AddUnit(2, NATIONALITY.Allied, 10, 12);
					AllUnits.AddUnit(1, NATIONALITY.Allied, 10, 13);
					AllUnits.AddUnit(1, NATIONALITY.Allied, 10, 14);
					AllUnits.AddUnit(1, NATIONALITY.Allied, 11, 12);
					AllUnits.AddUnit(1, NATIONALITY.Allied, 11, 13);
					AllUnits.AddUnit(1, NATIONALITY.Allied, 11, 14);

					AllUnits.AddUnit(1, NATIONALITY.German, 7, 0);
					AllUnits.AddUnit(1, NATIONALITY.German, 7, 1);
					AllUnits.AddUnit(1, NATIONALITY.German, 7, 2);
					AllUnits.AddUnit(2, NATIONALITY.German, 8, 0);
					AllUnits.AddUnit(2, NATIONALITY.German, 8, 1);
					AllUnits.AddUnit(2, NATIONALITY.German, 8, 2);
					AllUnits.AddUnit(2, NATIONALITY.German, 9, 0);
					AllUnits.AddUnit(1, NATIONALITY.German, 9, 1);
					AllUnits.AddUnit(1, NATIONALITY.German, 9, 2);
					AllUnits.AddUnit(1, NATIONALITY.German, 10, 0);
					AllUnits.AddUnit(1, NATIONALITY.German, 10, 1);
					AllUnits.AddUnit(1, NATIONALITY.German, 10, 2);
					AllUnits.AddUnit(1, NATIONALITY.German, 11, 2);
					break;
				case 2:
					InitializeCustomGame(5, 5);
					gameBoard.Map[2, 2].Terrain = 1;

					AllUnits.AddUnit(1, NATIONALITY.Allied, 2, 2);
					AllUnits.AddUnit(1, NATIONALITY.Allied, 1, 2);
					AllUnits.AddUnit(1, NATIONALITY.German, 1, 1);
					break;
				case 3: // test mountain cells
					InitializeCustomGame(7, 6);
					for (int i = 0; i < 4; i++)
					{
						gameBoard.Map[3, i + 1].Terrain = 6;
					}

					AllUnits.AddUnit(1, NATIONALITY.German, 0, 3);
					AllUnits.AddUnit(2, NATIONALITY.Allied, 6, 5);
					gameBoard.Map[6, 3].Terrain = 1;
					break;
				case 4: // test forest cells
					InitializeCustomGame(7, 6);
					for (int i = 0; i < 4; i++)
					{
						gameBoard.Map[3, i + 1].Terrain = 7;
					}

					AllUnits.AddUnit(1, NATIONALITY.German, 0, 3);
					AllUnits.AddUnit(2, NATIONALITY.German, 0, 4);
					AllUnits.AddUnit(2, NATIONALITY.Allied, 6, 0);
					gameBoard.Map[6, 3].Terrain = 1;
					break;
			}

			RecomputeMapMask();
			RecomputeMapView();
			SetEnemyStrategy();
		}

		//TODO: need to change the strategy if the enemy gets down to less units than cities
		public void SetEnemyStrategy()
		{
			// split into 4 groups of units and set each group of units to a different city
			int liTotalGermanUnits = AllUnits.TotalGermanUnits;

			if (gameBoard.TotalCities == 0)
			{
				return;
			}

			int liUnitsInGroup = liTotalGermanUnits / gameBoard.TotalCities;

			// this means that we can't hold all 4 cities (because there are less units than cities), just try to block the allied units
			if (liUnitsInGroup < 1)
			{
				liUnitsInGroup = 1;
			}

			// now find the closest "liUnitsIngroup" number of german units to each city
			for (int i = 0; i < gameBoard.CityList.Count; i++)
			{
				int liTotalUnitsAssigned = 0;
				while (liTotalUnitsAssigned < liUnitsInGroup)
				{
					// find the next german unit not assigned
					int liNextClosestGermanUnit = AllUnits.FindClosestGermanUnit(gameBoard.CityList[i].X, gameBoard.CityList[i].Y);

					// must have run out of units (this can happen if the number of units down divide evenly)
					if (liNextClosestGermanUnit == -1)
					{
						break;
					}

					// if this one is the shortest distance, then assign it the destination
					AllUnits.Items[liNextClosestGermanUnit].Command = UNITCOMMAND.Destination;
					AllUnits.Items[liNextClosestGermanUnit].DestX = gameBoard.CityList[i].X;
					AllUnits.Items[liNextClosestGermanUnit].DestY = gameBoard.CityList[i].Y;

					AllUnits.Items[liNextClosestGermanUnit].ComputePath(gameBoard);

					liTotalUnitsAssigned++;
				}
			}

			// any remaining units, choose closest cities
			for (int i = 0; i < AllUnits.Items.Count; i++)
			{
				if (AllUnits.Items[i].Command == UNITCOMMAND.None)
				{
					MapCoordinates closestCity = gameBoard.FindClosestCity(AllUnits.Items[i].X, AllUnits.Items[i].Y);
					if (closestCity != null)
					{
						AllUnits.Items[i].Command = UNITCOMMAND.Destination;
						AllUnits.Items[i].DestX = closestCity.X;
						AllUnits.Items[i].DestY = closestCity.Y;

						AllUnits.Items[i].ComputePath(gameBoard);
					}
				}
			}
		}

		public string Render()
		{
			StringBuilder @out = new StringBuilder();

			@out.Append("<input type='hidden' id='MaxX' value='" + gameBoard.MaxX + "' />");
			@out.Append("<input type='hidden' id='MaxY' value='" + gameBoard.MaxY + "' />");
			@out.Append("<input type='hidden' id='TestMode' value='" + (gameBoard.TestMode ? "1" : "0") + "' />");

			@out.Append("<svg xmlns='http://www.w3.org/2000/svg' version='1.1' width='" + ((gameBoard.MaxX + 1) * 54.75) + "' height='" + (gameBoard.MaxY * 31.25 * 2 + 31.25) + "' id='SVGObject'>");

			@out.Append(gameBoard.RenderTerrain());

			for (int i = 0; i < AllUnits.Items.Count; i++)
			{
				@out.Append(AllUnits.Items[i].Plot());
			}

			@out.Append(gameBoard.Render());

			for (int i = 0; i < AllUnits.Items.Count; i++)
			{
				@out.Append("<line style='stroke:rgb(255,0,0);stroke-width:4;' x1='0' y1='0' x2='0' y2='0' id='UnitDest" + (i + 1) + "' />");
			}

			@out.Append("</svg>");

			return @out.ToString();
		}

		public string AttackGermanUnit(int germanUnitNumber, int alliedUnitNumber)
		{
			int liGermanUnitIndex = AllUnits.FindUnitIndexFromNumber(germanUnitNumber);
			int liAlliedUnitIndex = AllUnits.FindUnitIndexFromNumber(alliedUnitNumber);
			BATTLERESULT liBattleResult = BattleCalculator.Result(AllUnits.Items[liAlliedUnitIndex].Offense);
			AllUnits.Items[liAlliedUnitIndex].UnitHasAttackedThisTurn = true;

			switch (liBattleResult)
			{
				case BATTLERESULT.DefenderDestroyed:
					AllUnits.Items.RemoveAt(liAlliedUnitIndex);
					break;
				case BATTLERESULT.EnemyDestroyed:
					AllUnits.Items.RemoveAt(liGermanUnitIndex);
					break;
				case BATTLERESULT.EnemyDamaged:
					AllUnits.Items[liGermanUnitIndex].Defense--;
					if (AllUnits.Items[liGermanUnitIndex].Defense <= 0)
					{
						AllUnits.Items.RemoveAt(liGermanUnitIndex);
						liBattleResult = BATTLERESULT.EnemyDestroyed;
					}
					break;
				case BATTLERESULT.EnemyDoubleDamaged:
					AllUnits.Items[liGermanUnitIndex].Defense-=2;
					if (AllUnits.Items[liGermanUnitIndex].Defense <= 0)
					{
						AllUnits.Items.RemoveAt(liGermanUnitIndex);
						liBattleResult = BATTLERESULT.EnemyDestroyed;
					}
					break;
			}

			return germanUnitNumber + "," + alliedUnitNumber + "," + (int)liBattleResult;
		}

		public void MoveUnit(int unitNumber, int X, int Y)
		{
			int liUnitIndex = AllUnits.FindUnitIndexFromNumber(unitNumber);
			if (liUnitIndex < AllUnits.Items.Count && liUnitIndex > -1)
			{
				AllUnits.Items[liUnitIndex].X = X;
				AllUnits.Items[liUnitIndex].Y = Y;

				gameBoard.UnmaskMapRegion(AllUnits.Items[liUnitIndex].X, AllUnits.Items[liUnitIndex].Y);
			}

			RecomputeMapView();
		}

		public void RecomputeMapView()
		{
			// reset visible flags
			for (int y = 0; y < gameBoard.MaxY; y++)
			{
				for (int x = 0; x < gameBoard.MaxX; x++)
				{
					gameBoard.Map[x, y].Visible = false;
				}
			}

			// spin through all units and unmask + view areas where units are sitting
			for (int i = 0; i < AllUnits.Items.Count; i++)
			{
				if (AllUnits.Items[i].Nationality == NATIONALITY.Allied)
				{
					// unmask and view all cells surrounding this x,y position
					gameBoard.ViewkMapRegion(AllUnits.Items[i].X, AllUnits.Items[i].Y);
				}
			}
		}
		
	}
}
