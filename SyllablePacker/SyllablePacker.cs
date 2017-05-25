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
        public const long DateTimeOffset = 100000;

        #region Float syllable helpers

        public static ushort Syllable(this float? value, int offset = 0)
            => value.HasValue ? Syllable(value.Value, offset) : Dash;

        public static ushort Syllable(this float value, int offset = 0)
            => Syllable((double)value, offset);

        public static float? GetFloat(long value, int offset = 0)
            => value != Dash ? ((float)value / 10) + offset : new float?();

        public static ushort SignedSyllable(this float? value)
            => Syllable(value, -SignedOffset);

        public static float? GetSignedFloat(long value)
            => GetFloat(value, -SignedOffset);

        #endregion

        #region Double syllable helpers

        public static ushort Syllable(this double? value, int offset = 0)
            => value.HasValue ? Syllable(value.Value, offset) : Dash;

        public static ushort Syllable(this double value, int offset = 0)
            => (ushort)((value - offset) * 10);

        public static double? GetDouble(long value, int offset = 0)
            => (value != Dash) ? ((double)value / 10) + offset : new double?();

        public static ushort SignedSyllable(this double? value)
            => Syllable(value, -SignedOffset);

        public static ushort SignedSyllable(this double value)
            => Syllable(value, -SignedOffset);

        public static double? GetSignedDouble(long value)
            => GetDouble(value, -SignedOffset);

        #endregion

        #region Decimal syllable helpers

        public static ushort Syllable(this decimal? value, int offset = 0)
            => value.HasValue ? Syllable(value.Value, offset) : Dash;

        public static ushort Syllable(this decimal value, int offset = 0)
            => (ushort)((value - offset) * 10);

        public static decimal? GetDecimal(long value, int offset = 0)
            => value != Dash ? ((decimal)value / 10) + offset : new decimal?();

        public static ushort SignedSyllable(this decimal? value)
            => Syllable(value, -SignedOffset);

        public static ushort SignedSyllable(this decimal value)
            => Syllable(value, -SignedOffset);

        public static decimal? GetSignedDecimal(long value)
            => GetDecimal(value, -SignedOffset);

        #endregion

        #region Integer syllable helpers

        public static ushort Syllable(this int? value, int offset = 0)
            => value.HasValue ? (ushort)(value.Value - offset) : Dash;

        public static ushort Syllable(this int value, int offset = 0)
            => (ushort)(value - offset);

        public static int? GetInteger(long value, int offset = 0)
            => value != Dash ? (int)value + offset : new int?();

        public static ushort SignedSyllable(this int? value)
            => Syllable(value, -SignedIntOffset);

        public static int? GetSignedInteger(long value)
            => GetInteger(value, -SignedIntOffset);

        #endregion

        #region DateTime helpers

        public static long Syllables(this DateTime? value)
            => value.HasValue ? (value.Value.Ticks / DateTimeOffset) : Dash;

        public static DateTime? GetDateTime(long value)
            => value != Dash ? new DateTime(value * DateTimeOffset) : new DateTime?();

        #endregion
    }

    public class SyllablePacker<T> where T : new()
    {
        private List<byte> buffer = new List<byte>();
        private int cursor = 0;
        private bool halfTriplet = false;
        private ushort halfSyllable;

        private List<Func<T, long>> getters = new List<Func<T, long>>();
        private List<Action<T, long>> setters = new List<Action<T, long>>();
        private List<byte> sizes = new List<byte>();

        public SyllablePacker<T> AddProperty(Func<T, long> getter, Action<T, long> setter, byte size = 1)
        {
            getters.Add(getter);
            setters.Add(setter);
            sizes.Add(size);
            return this;
        }

        public byte[] Pack(T target)
        {
            halfTriplet = false;

            for (var i = 0; i < getters.Count; i++)
            {
                var syllables = splitSyllables(getters[i](target), sizes[i]);
                for (var si = 0; si < syllables.Length; si++)
                {
                    if (halfTriplet)
                    {
                        var triplet = packTriplet(halfSyllable, syllables[si]);
                        buffer.AddRange(triplet);
                        halfTriplet = false;
                    }
                    else
                    {
                        if (i < getters.Count - 1 || si < syllables.Length - 1)
                        {
                            halfSyllable = syllables[si];
                            halfTriplet = true;
                        }
                        else
                            buffer.AddRange(packTriplet(syllables[si], 0));
                    }
                }
            }

            return buffer.ToArray();
        }

        private ushort[] splitSyllables(long value, byte size)
        {
            var syllables = new ushort[size];

            for (int i = 0; i < size; i++)
                syllables[size - (i + 1)] = (ushort)((value >> i * 12) & 0xfff);

            return syllables;
        }

        private long combineSyllables(ushort[] syllables, byte size)
        {
            long value = 0;

            for (int i = 0; i < size; i++)
                value |= (long)syllables[size - (i + 1)] << (i * 12);

            return value;
        }

        public T Unpack(byte[] bytes)
        {
            var target = new T();
            cursor = 0;
            halfTriplet = false;
            for (var i = 0; i < setters.Count; i++)
            {
                var syllables = new ushort[sizes[i]];
                for (int si = 0; si < sizes[i]; si++)
                {

                    if (halfTriplet)
                    {
                        syllables[si] = halfSyllable;
                        halfTriplet = false;
                    }
                    else
                    {
                        var triplet = new byte[] { bytes[cursor], bytes[cursor + 1], bytes[cursor + 2] };
                        cursor += 3;
                        var syllablePair = unpackTriplet(triplet);
                        syllables[si] = syllablePair.Item1;
                        halfSyllable = syllablePair.Item2;
                        halfTriplet = true;
                    }
                }

                setters[i](target, combineSyllables(syllables, sizes[i]));

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
