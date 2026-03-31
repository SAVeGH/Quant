using Qntm.Constants;

namespace Qntm.Gates
{
    public static class Gates
    {
        public static void H(Quantum q) 
        {
            q.Angle = q.Angle + Angles._90degree;
        }

        public static void X(Quantum q)
        {
            q.Angle = q.Angle + Angles._180degree; // negate value
        }
    }
}
