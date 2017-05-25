using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example
{
    public class ExampleClass
    {
        public float FloatVal1 { get; set; }
        public float FloatVal2 { get; set; }
        public float? Nullable { get; set; }
        public float Zero { get; set; }
        public float Signed { get; set; }

        public override string ToString()
        {
            return $" FloatVal1: {FloatVal1}\n FloatVal2: {FloatVal2}\n Nullable: {(Nullable.HasValue ? Nullable.Value.ToString() : "null")}\n"
                +$" Zero: {Zero}\n Signed: {Signed}\n";
        }
    }
}
