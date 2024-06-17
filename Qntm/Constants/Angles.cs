using System;

namespace Qntm.Constants
{
    // константы для часто используемых углов
    public static class Angles
    {
        public const double _0degree = 0.0;
        public const double _30degree = Math.PI / 6.0;        
        public const double _45degree = Math.PI / 4.0;
        public const double _60degree = _30degree * 2.0;
        public const double _90degree = _30degree * 3.0;
        public const double _120degree = _30degree * 4.0;
        public const double _180degree = Math.PI;
        public const double _240degree = _120degree * 2.0;
        public const double _270degree = _90degree * 3.0;
        public const double _360degree = 2.0 * Math.PI;
        public const double _rad = Math.PI / 180.0;
        public const double _grad = 180.0 / Math.PI;
    }
}
