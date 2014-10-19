using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleFieldOneCore
{
	public class GameBoard
	{
		public int MaxX = 1;
		public int MaxY = 1;
		public GameMap[,] Map = new GameMap[1, 1];
		public bool TestMode = true;

		public static int _totalCities = -1;
		public int TotalCities
		{
			get
			{
				if (_totalCities == -1)
				{
					_totalCities = 0;
					for (int y = 0; y < MaxY; y++)
					{
						for (int x = 0; x < MaxX; x++)
						{
							if (Map[x, y].Terrain == 1)
							{
								_totalCities++;
							}
						}
					}
				}

				return _totalCities;
			}
		}

		private List<MapCoordinates> _cityList = null;
		public List<MapCoordinates> CityList
		{
			get
			{
				if (_cityList == null)
				{
					List<MapCoordinates> laCities = new List<MapCoordinates>();

					for (int y = 0; y < MaxY; y++)
					{
						for (int x = 0; x < MaxX; x++)
						{
							if (Map[x, y].Terrain == 1)
							{
								laCities.Add(new MapCoordinates(x, y));
							}
						}
					}
					_cityList = laCities;
				}
				return _cityList;
			}
		}

		public void InitializeBoard(int width, int height)
		{
			Map = new GameMap[width, height];
			MaxX = width;
			MaxY = height;
			_totalCities = -1;

			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					Map[x, y] = new GameMap();
				}
			}
		}

		public void UnmaskMapRegion(int X, int Y)
		{
			// TODO: if range factors are added, we'll need to account for that

			Map[X, Y].Mask = true;

			// unmask cell above and below
			if (Y > 0)
			{
				Map[X, Y - 1].Mask = true;
			}

			if (Y < MaxY-1)
			{
				Map[X, Y + 1].Mask = true;
			}

			if (X % 2 == 1)
			{
				// y and y+1
				if (X > 0)
				{
					Map[X - 1, Y].Mask = true;

					if (Y < MaxY-1)
					{
						Map[X - 1, Y + 1].Mask = true;
					}
				}

				if (X < MaxX-1)
				{
					Map[X + 1, Y].Mask = true;

					if (Y < MaxY-1)
					{
						Map[X + 1, Y + 1].Mask = true;
					}
				}
			}
			else
			{
				// y and y-1
				if (X > 0)
				{
					Map[X - 1, Y].Mask = true;

					if (Y > 0)
					{
						Map[X - 1, Y - 1].Mask = true;
					}
				}

				if (X < MaxX-1)
				{
					Map[X + 1, Y].Mask = true;

					if (Y > 0)
					{
						Map[X + 1, Y - 1].Mask = true;
					}
				}
			}
		}

		public void ViewkMapRegion(int X, int Y)
		{
			// TODO: if range factors are added, we'll need to account for that
			Map[X, Y].Visible = true;

			// unmask cell above and below
			if (Y > 0)
			{
				Map[X, Y - 1].Visible = true;
			}

			if (Y < MaxY-1)
			{
				Map[X, Y + 1].Visible = true;
			}

			if (X % 2 == 1)
			{
				// y and y+1
				if (X > 0)
				{
					Map[X - 1, Y].Visible = true;

					if (Y < MaxY-1)
					{
						Map[X - 1, Y + 1].Visible = true;
					}
				}

				if (X < MaxX-1)
				{
					Map[X + 1, Y].Visible = true;

					if (Y < MaxY-1)
					{
						Map[X + 1, Y + 1].Visible = true;
					}
				}
			}
			else
			{
				// y and y-1
				if (X > 0)
				{
					Map[X - 1, Y].Visible = true;

					if (Y > 0)
					{
						Map[X - 1, Y - 1].Visible = true;
					}
				}

				if (X < MaxX-1)
				{
					Map[X + 1, Y].Visible = true;

					if (Y > 0)
					{
						Map[X + 1, Y - 1].Visible = true;
					}
				}
			}
		}

		public List<MapCoordinates> FindAdjacentCells(int X, int Y, int unitType)
		{
			List<MapCoordinates> list = new List<MapCoordinates>();

			if (Y - 1 >= 0)
			{
				if (!Map[X, Y - 1].Blocked(unitType))
				{ 
					list.Add(new MapCoordinates(X, Y - 1)); 
				}
			}

			if (Y + 1 < MaxY)
			{
				if (!Map[X, Y + 1].Blocked(unitType))
				{
					list.Add(new MapCoordinates(X, Y + 1));
				}
			}

			if (X % 2 == 1)
			{
				// odd (y and y+1)
				if (X - 1 >= 0)
				{
					if (!Map[X - 1, Y].Blocked(unitType))
					{
						list.Add(new MapCoordinates(X - 1, Y));
					}
					if (Y + 1 < MaxY)
					{
						if (!Map[X - 1, Y + 1].Blocked(unitType))
						{
							list.Add(new MapCoordinates(X - 1, Y + 1));
						}
					}
				}
				if (X + 1 < MaxX)
				{
					if (!Map[X + 1, Y].Blocked(unitType))
					{
						list.Add(new MapCoordinates(X + 1, Y));
					}
					if (Y + 1 < MaxY)
					{
						if (!Map[X + 1, Y + 1].Blocked(unitType))
						{
							list.Add(new MapCoordinates(X + 1, Y + 1));
						}
					}
				}
			}
			else
			{
				// even (y and y-1)
				if (X - 1 >= 0)
				{
					if (!Map[X - 1, Y].Blocked(unitType))
					{
						list.Add(new MapCoordinates(X - 1, Y));
					}
					if (Y - 1 >= 0)
					{
						if (!Map[X - 1, Y - 1].Blocked(unitType))
						{
							list.Add(new MapCoordinates(X - 1, Y - 1));
						}
					}
				}
				if (X + 1 < MaxX)
				{
					if (!Map[X + 1, Y].Blocked(unitType))
					{
						list.Add(new MapCoordinates(X + 1, Y));
					}
					if (Y - 1 >= 0)
					{
						if (!Map[X + 1, Y - 1].Blocked(unitType))
						{
							list.Add(new MapCoordinates(X + 1, Y - 1));
						}
					}
				}
			}

			return list;
		}

		public MapCoordinates FindClosestCity(int X, int Y)
		{
			double closestDistance = 9999;
			int index = -1;

			for (int i = 0; i < CityList.Count; i++)
			{
				double distance = BattleFieldOneCommonObjects.Distance(CityList[i].X, CityList[i].Y, X, Y);
				if (distance < closestDistance)
				{
					closestDistance = distance;
					index = i;
				}
			}

			if (index > -1)
			{
				return new MapCoordinates(CityList[index].X, CityList[index].Y);
			}

			return null;
		}

		private string DrawLine(double x1, double y1, double x2, double y2, string psColor)
		{
			return "<line x1='" + x1 + "' y1='" + y1 + "' x2='" + x2 + "' y2='" + y2 + "' stroke='" + psColor + "' />";
		}

		private string DrawRectangle(double x, double y, double Width, double Height, string psColor, int piUnitNumber)
		{
			return "<rect x='" + x + "' y='" + y + "' width='" + Width + "' height='" + Height + "' fill='" + psColor + "' stroke='black' stroke-width='2' id='Unit" + piUnitNumber + "' onmouseover='MouseOverUnit(" + piUnitNumber + ");' onmouseout='MouseOutUnit(" + piUnitNumber + ");' />";
		}

		public string DrawHexBox(int piX, int piY)
		{
			double lnX = (15.75 + 39) * piX;
			double lnY = 31.25 * (piX % 2) + piY * (31.25 * 2);
			StringBuilder @out = new StringBuilder();

			@out.Append(DrawLine(15.75 + lnX, lnY, (15.75 + 39) + lnX, lnY, "black"));
			@out.Append(DrawLine((15.75 + 39) + lnX, lnY, (15.75 + 39 + 15.75) + lnX, 31.25 + lnY, "black"));
			@out.Append(DrawLine((15.75 + 39 + 15.75) + lnX, 31.25 + lnY, (15.75 + 39) + lnX, 31.25 * 2 + lnY, "black"));
			@out.Append(DrawLine((15.75 + 39) + lnX, 31.25 * 2 + lnY, 15.75 + lnX, 31.25 * 2 + lnY, "black"));
			@out.Append(DrawLine(15.75 + lnX, 31.25 * 2 + lnY, lnX, 31.25 + lnY, "black"));
			@out.Append(DrawLine(lnX, 31.25 + lnY, 15.75 + lnX, lnY, "black"));

			// center point
			//@out.Append(DrawLine((19.5 + 15.75) * Scale + lnX - 5, 31.25 + lnY, (19.5 + 15.75) * Scale + lnX + 5, 31.25 + lnY, "red"));
			//@out.Append(DrawLine((19.5 + 15.75) * Scale + lnX, 31.25 + lnY - 5, (19.5 + 15.75) * Scale + lnX, 31.25 + lnY + 5, "red"));

			return @out.ToString();
		}

		public string Render()
		{
			StringBuilder @out = new StringBuilder();

			for (int x = 0; x < MaxX; x++)
			{
				for (int y = 0; y < MaxY; y++)
				{
					// overlay with visible hex
					@out.Append(DrawViewHex(x, y, (TestMode ? true : Map[x, y].Visible)));

					// overlay with mask hex
					@out.Append(DrawMaskHex(x, y, (TestMode ? true : Map[x, y].Mask)));

					@out.Append(DrawHexBox(x, y));
				}
			}

			return @out.ToString();
		}

		public string RenderTerrain()
		{
			StringBuilder @out = new StringBuilder();

			for (int x = 0; x < MaxX; x++)
			{
				for (int y = 0; y < MaxY; y++)
				{
					@out.Append(DrawTerrain(x, y, Map[x, y].Terrain));
				}
			}

			return @out.ToString();
		}

		private string DrawMaskHex(int piX, int piY, bool plVisible)
		{
			double lnX = (15.75 + 39) * piX;
			double lnY = 31.25 * (piX % 2) + piY * (31.25 * 2);
			return "<image xlink:href='/Content/img/blank_hex.png' x='" + lnX + "' y='" + lnY + "' width='71' height='63' style='display:" + (plVisible ? "none" : "") + ";' id='Mask" + piX + "," + piY + "' />";
		}

		private string DrawViewHex(int piX, int piY, bool plVisible)
		{
			double lnX = (15.75 + 39) * piX;
			double lnY = 31.25 * (piX % 2) + piY * (31.25 * 2);
			return "<image xlink:href='/Content/img/blank_hex.png' x='" + lnX + "' y='" + lnY + "' width='71' height='63' style='opacity:0.3; display:" + (plVisible ? "none" : "") + ";' id='View" + piX + "," + piY + "' />";
		}

		private string DrawTerrain(int piX, int piY, int terrainType)
		{
			double lnX = (15.75 + 39) * piX;
			double lnY = 31.25 * (piX % 2) + piY * (31.25 * 2);

			string hexPngName = "grass_background_hex";
			switch (terrainType)
			{
				case 0:
					hexPngName = "grass_background_hex";
					break;
				case 1:
					hexPngName = "city_hex";
					break;
				case 2:
					hexPngName = "terrain_grass_01";
					break;
				case 3:
					hexPngName = "terrain_grass_02";
					break;
				case 4:
					hexPngName = "terrain_grass_03";
					break;
				case 5:
					hexPngName = "terrain_grass_04";
					break;
				case 6:
					hexPngName = "mountains_01";
					break;
				case 7:
					hexPngName = "forest_01";
					break;
			}

			return "<image xlink:href='/Content/img/" + hexPngName + ".png' x='" + lnX + "' y='" + lnY + "' width='71' height='63' />";
		}
	}
}
