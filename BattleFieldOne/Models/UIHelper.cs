using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BattleFieldOne.Models
{
	public static class UIHelper
	{
		public static int ToInt(this object poField)
		{
			int liReturn;
			try
			{
				liReturn = Convert.ToInt32(poField);
			}
			catch (InvalidCastException)
			{
				liReturn = 0;
			}
			catch (FormatException)
			{
				liReturn = 0;
			}
			return liReturn;
		}
	}
}