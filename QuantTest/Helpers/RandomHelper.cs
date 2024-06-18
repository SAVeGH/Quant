using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuantTest.Helpers
{
    public static class RandomHelper
    {
        public static Random Create() 
        {
            Guid guid = Guid.NewGuid();
            int seed = BitConverter.ToInt32(guid.ToByteArray());
            Random random = new Random(seed);

            return random;
        }
    }
}
