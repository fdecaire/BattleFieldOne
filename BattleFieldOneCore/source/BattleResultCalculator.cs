using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleFieldOneCore
{
	public static class BattleCalculator
	{
		public static BATTLERESULT Result(int offense)
		{
			int liDieRoll = DieRoller.DieRoll() + DieRoller.DieRoll() + DieRoller.DieRoll();

			if (liDieRoll > 10 && offense > 12)
			{
				return BATTLERESULT.EnemyTripleDamaged;
			}

			if (liDieRoll > 8 && offense > 1)
			{
				return BATTLERESULT.EnemyDoubleDamaged;
			}

			if (liDieRoll > 6)
			{
				return BATTLERESULT.EnemyDamaged;
			}

			return BATTLERESULT.None;
		}
	}
}
