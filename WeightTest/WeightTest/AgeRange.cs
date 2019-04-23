using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeightTest
{
    public class Range
    {
        public int Min { get; set; }

        public int Max { get; set; }

        public Range(int min, int max)
        {
            Min = min;
            Max = max;
        }

        public bool InsideRange(int age)
        {
            return age >= Min && age <= Max;
        }
    }
}
