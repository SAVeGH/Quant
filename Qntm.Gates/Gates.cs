using Qntm.Constants;

namespace Qntm.Gates
{
    // https://sharetechnote.com/html/WebProgramming/Websim_QuantumGate.html
    public static class Gates
    {
        public static void H(Quantum q) 
        {
            double resultAngle = Angles._90degree - q.Angle;
            double angleShift = resultAngle - q.Angle;

            Quantum.ShiftAngle(q, angleShift);
        }

        public static void X(Quantum q)
        {
            double resultAngle = Angles._180degree - q.Angle;
            double angleShift = resultAngle - q.Angle;

            Quantum.ShiftAngle(q, angleShift); // negate value            
        }
    }
}
