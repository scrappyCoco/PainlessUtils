using System;

namespace Coding4fun.PainlessUtils
{
    public readonly struct TextRange
    {
        public int Offset { get; }
        public int Length { get; }

        public TextRange(int offset, int length)
        {
            if (offset < 0) throw new ArgumentException($"{nameof(offset)} must be greater than 0.", nameof(offset));
            if (length < 0) throw new ArgumentException($"{nameof(length)} must be greater than 0.", nameof(length));
            Offset = offset;
            Length = length;
        }

        public int EndOffset => Offset + Length;

        private bool Equals(TextRange other) => Offset == other.Offset && Length == other.Length;

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is TextRange other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return (Offset * 397) ^ Length;
            }
        }

        public static bool operator ==(TextRange left, TextRange right) => left.Equals(right);

        public static bool operator !=(TextRange left, TextRange right) => !left.Equals(right);

        /// <inheritdoc />
        public override string ToString() => $"[{Offset}, {Length}]";
    }
}