using Piksel.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example
{
    using SP = SyllablePacker;

    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Syllable Packer Example";

            var packer = new SyllablePacker<ExampleClass>()
                .AddProperty( o => o.FloatValue.Syllable(),            (o, v) => o.FloatValue = SP.GetFloat(v).Value)
                .AddProperty( o => o.DoubleValue.Syllable(),           (o, v) => o.DoubleValue = SP.GetFloat(v).Value)
                .AddProperty( o => o.Null.Syllable(),                  (o, v) => o.Null = SP.GetFloat(v))
                .AddProperty( o => o.Zero.Syllable(),                  (o, v) => o.Zero = SP.GetFloat(v).Value)
                .AddProperty( o => o.Signed.SignedSyllable(),          (o, v) => o.Signed = SP.GetSignedFloat(v).Value)
                .AddProperty( o => o.DecimalValue.Syllable(),          (o, v) => o.DecimalValue = SP.GetDecimal(v).Value)
                .AddProperty( o => o.Large.GetValueOrDefault(SP.Dash), (o, v) => o.Large = v != SP.Dash ? (ushort)v : new ushort?())
                .AddProperty( o => o.Larger.GetValueOrDefault(SP.Dash),(o, v) => o.Larger = v != SP.Dash ? v : new long?(), 2)
                .AddProperty( o => o.Time.Syllables(),                 (o, v) => o.Time = SP.GetDateTime(v), 4);

            var pre = new ExampleClass()
            {
                FloatValue = 33.5f,
                DoubleValue = 200.1f,
                DecimalValue = 304.9m,
                Null = null,
                Zero = 0,
                Signed = -75.1f,
                Large = 4094,
                Larger = 16000,
                Time = DateTime.Now
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
