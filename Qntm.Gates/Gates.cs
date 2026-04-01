using Qntm.Constants;

namespace Qntm.Gates
{
    // https://sharetechnote.com/html/WebProgramming/Websim_QuantumGate.html
    public static class Gates
    {
        public static void H(Quantum q) 
        {
            q.Angle = Angles._90degree - q.Angle;
        }

        public static void X(Quantum q)
        {
            q.Angle = Angles._180degree - q.Angle; // negate value
        }
    }
}
