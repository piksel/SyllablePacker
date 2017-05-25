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
        public const byte SignedOffset = 200;
        public const ushort SignedIntOffset = 2000;

        #region Float syllable helpers

        public static ushort Syllable(this float? value, int offset = 0)
            => value.HasValue ? Syllable(value.Value, offset) : Dash;

        public static ushort Syllable(this float value, int offset = 0)
            => Syllable((double)value, offset);

        public static float? GetFloat(ushort value, int offset = 0)
            => value != Dash ? new float?(((float)value / 10) + offset) : new float?();

        public static ushort SignedSyllable(this float? value)
            => Syllable(value, -SignedOffset);

        public static float? GetSignedFloat(ushort value)
            => GetFloat(value, -SignedOffset);

        #endregion

        #region Double syllable helpers

        public static ushort Syllable(this double? value, int offset = 0)
            => value.HasValue ? Syllable(value.Value, offset) : Dash;

        public static ushort Syllable(this double value, int offset = 0)
            => (ushort)((value - offset) * 10);

        public static double? GetDouble(ushort value, int offset = 0)
            => (value != Dash) ? new double?(((double)value / 10) + offset) : new double?();

        public static ushort SignedSyllable(this double? value)
            => Syllable(value, -SignedOffset);

        public static ushort SignedSyllable(this double value)
            => Syllable(value, -SignedOffset);

        public static double? GetSignedDouble(ushort value)
            => GetDouble(value, -SignedOffset);

        #endregion

        #region Decimal syllable helpers

        public static ushort Syllable(this decimal? value, int offset = 0)
            => value.HasValue ? Syllable(value.Value, offset) : Dash;

        public static ushort Syllable(this decimal value, int offset = 0)
            => (ushort)((value - offset) * 10);

        public static decimal? GetDecimal(ushort value, int offset = 0)
            => value != Dash ? new decimal?(((decimal)value / 10) + offset) : new decimal?();

        public static ushort SignedSyllable(this decimal? value)
            => Syllable(value, -SignedOffset);

        public static ushort SignedSyllable(this decimal value)
            => Syllable(value, -SignedOffset);

        public static decimal? GetSignedDecimal(ushort value)
            => GetDecimal(value, -SignedOffset);

        #endregion

        #region Integer syllable helpers

        public static ushort Syllable(this int? value, int offset = 0)
            => value.HasValue ? (ushort)(value.Value - offset) : Dash;

        public static ushort Syllable(this int value, int offset = 0)
            => (ushort)(value - offset);

        public static int? GetInteger(ushort value, int offset = 0)
            => value + offset;

        public static ushort SignedSyllable(this int? value)
            => Syllable(value, -SignedIntOffset);

        public static int? GetSignedInteger(ushort value)
            => GetInteger(value, -SignedIntOffset);

        #endregion
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
