using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BodyScanMulticamera
{
    public class DensityScale
    {
        public float DensityValue { get; set; }
        public float Delta { get; set; }

        public DensityScale(float densityValue, float delta)
        {
            DensityValue = densityValue;
            Delta = delta;
        }

        public float MaxDensityValue()
        {
            return DensityValue + Delta;
        }

        public float MinDensityValue()
        {
            return DensityValue - Delta;
        }
    }
}
