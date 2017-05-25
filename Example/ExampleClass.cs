using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example
{
    public class ExampleClass
    {
        public float? FloatValue { get; set; }
        public double? DoubleValue { get; set; }
        public decimal? DecimalValue { get; set; }
        public float? Null { get; set; }
        public float? Zero { get; set; }
        public float? Signed { get; set; }
        public ushort? Large { get; set; }
        public long? Larger { get; set; }
        public DateTime? Time { get; set; }

        public override string ToString()
            => $" FloatVal1: {FormatValue(FloatValue)}\n"
             + $" DoubleVal: {FormatValue(DoubleValue)}\n"
             + $" Decimal: {FormatValue(DecimalValue)}\n"
             + $" Null: {FormatValue(Null)}\n"
             + $" Zero: {FormatValue(Zero)}\n"
             + $" Signed: {FormatValue(Signed)}\n"
             + $" Large: {FormatValue(Large)}\n"
             + $" Larger: {FormatValue(Larger)}\n"
             + $" Time: {FormatValue(Time, "yyyy-MM-dd HH:mm")}\n";

        private static string FormatValue<T>(T? value, string format = "F1") where T: struct, IFormattable
            => $"{(value.HasValue ? value.Value.ToString(format, null) : "null")}";
        
    }
}
