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

        public static List<bool> RandomSequence(int length)
        {
            List<bool> sequence = new List<bool>();

            Random random = RandomHelper.Create();

            for (int i = 0; i < length; i++)
            {
                int intValue = random.Next();

                byte[] intArray = BitConverter.GetBytes(intValue);

                for (int j = 0; j < 4; j++)
                {
                    byte b = intArray[j];

                    WriteByteValue(b, sequence);
                }
            }

            return sequence;
        }

        private static void WriteByteValue(byte b, List<bool> sequence)
        {
            for (int i = 0; i < 8; i++)
            {
                bool bValue = (b & 1) == 1;
                sequence.Add(bValue);
                b = (byte)(b >> 1);
            }
        }
    }
}
