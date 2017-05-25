using Piksel.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Syllable Packer Example";

            var packer = new SyllablePacker<ExampleClass>()
                .AddProperty(
                    o => o.FloatValue.Syllable(),
                    (o, v) => o.FloatValue = SyllablePacker.GetFloat(v).Value)
                .AddProperty(
                    o => o.DoubleValue.Syllable(),
                    (o, v) => o.DoubleValue = SyllablePacker.GetFloat(v).Value)
                .AddProperty(
                    o => o.Null.Syllable(),
                    (o, v) => o.Null = SyllablePacker.GetFloat(v))
                .AddProperty(
                    o => o.Zero.Syllable(),
                    (o, v) => o.Zero = SyllablePacker.GetFloat(v).Value)
                .AddProperty(
                    o => o.Signed.SignedSyllable(),
                    (o, v) => o.Signed = SyllablePacker.GetSignedFloat(v).Value)
                .AddProperty(
                    o => o.DecimalValue.Syllable(),
                    (o, v) => o.DecimalValue = SyllablePacker.GetDecimal(v).Value)
                 .AddProperty(
                    o => o.Large.HasValue ? o.Large.Value : SyllablePacker.Dash,
                    (o, v) => o.Large = v == SyllablePacker.Dash ? new ushort?() : new ushort?(v)
                 );

            var pre = new ExampleClass()
            {
                FloatValue = 33.5f,
                DoubleValue = 200.1f,
                DecimalValue = 304.9m,
                Null = null,
                Zero = 0,
                Signed = -75.1f,
                Large = 4094
            };

            Console.WriteLine($"Pre pack:\n{pre}\n");

            var bytes = packer.Pack(pre);

            Console.WriteLine($"Bytes:");
            foreach (var b in bytes)
                Console.Write(" " + b.ToString("x2"));

            Console.WriteLine("\n");

            var post = packer.Unpack(bytes);

            Console.WriteLine($"Post pack:\n{post}\n");

            Console.Write("Press any key to exit...");
            Console.ReadKey();

        }
    }
}
