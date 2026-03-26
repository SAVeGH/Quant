namespace Qntm
{
    public class QuantumPointer
    {        
        private Quantum _quantum;

        public QuantumPointer(Quantum quantum) 
        {            
            _quantum = quantum;
        }

        public Quantum Quantum { get { return _quantum; } }

        // Инверсия связи означает:
        // Для IsInverse = False - значит вектор поворачивающего кванта вращаеся на заданое изменение в ту же сторону, что и вектор поворачиваемого кванта
        // Для IsInverse = True - значит вектор поворачиваемого кванта вращаеся на заданную величину в сторону противоположную вращиню поворачивающего кванта
        // См. реализацию W state
        public bool IsInverse { get; set; }
    }
}
