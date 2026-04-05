using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qntm.Functions
{
    public static class DeutchFunctions
    {
        public static bool ConstFalse(bool inValue) 
        {
            return false;
        }

        public static bool ConstTrue(bool inValue)
        {
            return true;
        }

        public static bool BalancedId(bool inValue)
        {
            return inValue;
        }

        public static bool BalancedNot(bool inValue)
        {
            return !inValue;
        }
    }
}
