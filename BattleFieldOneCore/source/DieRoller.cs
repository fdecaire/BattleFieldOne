using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTestHelpersNS;

namespace BattleFieldOneCore
{
    public static class DieRoller
    {
        private static Random RandomNumberGenerator = new Random(DateTime.Now.Millisecond);

        public static int DieRoll()
        {
            if (UnitTestHelpers.IsInUnitTest)
            {
                return UnitTestHelpers.SetDieRoll;
            }
            else
            {
                return RandomNumberGenerator.Next() % 6;
            }
        }
    }
}
