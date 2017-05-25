using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Piksel.Serialization
{
    public static class SyllablePacker
    {
        public const ushort Dash = 0xFFF;

        public static ushort Syllable(this float? value, int offset = 0)
            => value.HasValue ? Syllable(value.Value, offset) : Dash;

        public static ushort Syllable(this float value, int offset = 0)
            => (ushort)((value - offset) * 10);

        public static float? GetFloat(ushort value, int offset = 0)
        {
            if (value != Dash)
                return ((float)value / 10) + offset;
            return null;
        }

        public static ushort SignedSyllable(this float? value)
            => Syllable(value, -200);

        public static ushort SignedSyllable(this float value)
            => Syllable(value, -200);

        public static float? GetSignedFloat(ushort value)
            => GetFloat(value, -200);
    }

    public class SyllablePacker<T> where T : new()
    {
        private List<byte> buffer = new List<byte>();
        private int cursor = 0;
        private ushort halfTriplet;

        private List<Func<T, ushort>> getters = new List<Func<T, ushort>>();
        private List<Action<T, ushort>> setters = new List<Action<T, ushort>>();

        public SyllablePacker<T> AddProperty(Func<T, ushort> getter, Action<T, ushort> setter)
        {
            getters.Add(getter);
            setters.Add(setter);
            return this;
        }

        public byte[] Pack(T target)
        {
            for (var i = 0; i < getters.Count; i++)
            {
                var syllable = getters[i](target);
                if ((i & 1) == 1)
                {
                    var triplet = packTriplet(halfTriplet, syllable);
                    buffer.AddRange(triplet);
                }
                else
                {
                    if (i < getters.Count - 1)
                        halfTriplet = syllable;
                    else
                        buffer.AddRange(packTriplet(syllable, 0));
                }
            }

            return buffer.ToArray();
        }

        public T Unpack(byte[] bytes)
        {
            var target = new T();
            cursor = 0;
            for (var i = 0; i < setters.Count; i++)
            {
                if ((i & 1) == 0)
                {
                    var triplet = new byte[] { bytes[cursor], bytes[cursor + 1], bytes[cursor + 2] };
                    cursor += 3;
                    var syllables = unpackTriplet(triplet);
                    setters[i](target, syllables.Item1);
                    halfTriplet = syllables.Item2;
                }
                else
                {
                    setters[i](target, halfTriplet);
                }

            }
            return target;
        }

        private byte[] packTriplet(ushort syllable1, ushort syllable2)
        {
            return new byte[]
            {
                (byte)(syllable1 >> 4),
                (byte)(((syllable1 & 0xf)<< 4) | (syllable2 >> 8)),
                (byte)(syllable2)
            };
        }

        private Tuple<ushort, ushort> unpackTriplet(byte[] triplet)
        {
            return new Tuple<ushort, ushort>(
                (ushort)(triplet[0] << 4 | (triplet[1] & 0xf0) >> 4),
                (ushort)(((triplet[1] & 0xf) << 8) | triplet[2])
            );
        }
    }
}
