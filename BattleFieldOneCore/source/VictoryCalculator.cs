using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleFieldOneCore
{
	public class VictoryCalculator
	{
		private int TotalAlliedCitiesToCapture = 0;
		private int TotalGermanCitiesToCapture = 0;
		private int TotalAlliedUnitsAlive = 0;
		private int TotalGermanUnitsAlive = 0;
		private int TotalGermanCitiesToDefend = 0;
		private int TotalAlliedCitiesToDefend = 0;

		public void AlliesCaptureCitiesToWin(int totalCities)
		{
			TotalAlliedCitiesToCapture = totalCities;
		}

		public void GermanCaptureCitiesToWin(int totalCities)
		{
			TotalGermanCitiesToCapture = totalCities;
		}

		public void AlliedCitiesToDefend(int totalCities)
		{
			TotalAlliedCitiesToDefend = totalCities;
		}

		public void GermanCitiesToDefend(int totalcities)
		{
			TotalGermanCitiesToDefend = totalcities;
		}

		public string Result(UnitsList AllUnits, GameBoard gameBoard)
		{
			string lsResult = "";

			// check to see if german units occupy all cities
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

			if (TotalGermanCitiesToDefend > 0)
			{
				if (liTotal < TotalGermanCitiesToDefend)
				{
					return "Germany Failed to Defend " + TotalGermanCitiesToDefend + " Cit" + (TotalGermanCitiesToDefend == 1 ? "y" : "ies") + ".  Allies Win!";
				}
			}
			else
			{
				if (liTotal >= TotalGermanCitiesToCapture)
				{
					if (gameBoard.CityList.Count == liTotal)
					{
						return "Germany Captured All Cities!";
					}
					else
					{
						return "Germany Captured " + liTotal + " Cit" + (TotalGermanCitiesToCapture == 1 ? "y" : "ies") + " Needed for a Victory!";
					}
				}
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

			if (liTotal >= TotalAlliedCitiesToCapture)
			{
				if (liTotal == gameBoard.CityList.Count)
				{
					return "Allies Captured All Cities!";
				}
				else
				{
					return "Allies Captured " + liTotal + " Citi" + (TotalAlliedCitiesToCapture == 1 ? "y" : "ies") + " Needed for a Victory!";
				}
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
	}
}
