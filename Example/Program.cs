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
                    o => o.FloatVal1.Syllable(),
                    (o, v) => o.FloatVal1 = SyllablePacker.GetFloat(v).Value)
                .AddProperty(
                    o => o.FloatVal2.Syllable(),
                    (o, v) => o.FloatVal2 = SyllablePacker.GetFloat(v).Value)
                .AddProperty(
                    o => o.Nullable.Syllable(),
                    (o, v) => o.Nullable = SyllablePacker.GetFloat(v))
                .AddProperty(
                    o => o.Zero.Syllable(),
                    (o, v) => o.Zero = SyllablePacker.GetFloat(v).Value)
                .AddProperty(
                    o => o.Signed.SignedSyllable(),
                    (o, v) => o.Signed = SyllablePacker.GetSignedFloat(v).Value);

            var pre = new ExampleClass()
            {
                FloatVal1 = 33.5f,
                FloatVal2 = 200.1f,
                Nullable = null,
                Zero = 0,
                Signed = -75.1f
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
