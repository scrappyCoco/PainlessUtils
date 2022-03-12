using System;

namespace Coding4fun.PainlessUtils
{
    public readonly struct Range
    {
        public int Offset { get; }
        public int Length { get; }

        public Range(int offset, int length)
        {
            if (offset < 0) throw new ArgumentException($"{nameof(offset)} must be greater than 0.", nameof(offset));
            if (length < 0) throw new ArgumentException($"{nameof(length)} must be greater than 0.", nameof(length));
            Offset = offset;
            Length = length;
        }

        public int EndOffset => Offset + Length - 1;

        private bool Equals(Range other) => Offset == other.Offset && Length == other.Length;

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is Range other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return (Offset * 397) ^ Length;
            }
        }

        public bool Contains(int value) => Offset <= value && value <= EndOffset;
        public bool Contains(Range value) => Offset <= value.Offset && value.EndOffset <= EndOffset;

        public bool Intersect(Range value) => Offset <= value.Offset && value.Offset <= EndOffset
                                              || Offset <= value.EndOffset && value.EndOffset <= EndOffset;

        public static bool operator ==(Range left, Range right) => left.Equals(right);

        public static bool operator !=(Range left, Range right) => !left.Equals(right);

        public static Range operator -(Range left, Range right)
        {
            bool isInvalid = false;
            
            if (left.Offset == right.Offset)
            {
                if (left.EndOffset <= right.EndOffset)
                {
                    isInvalid = true;
                }
            }
            else if (left.EndOffset == right.EndOffset)
            {
                if (right.Offset <= left.Offset)
                {
                    isInvalid = true;
                }
            }
            else if (right.Contains(left) || left.Contains(right))
            {
                isInvalid = true;
            }
            else if (!left.Intersect(right))
            {
                isInvalid = true;
            }
            
            if (isInvalid)
            {
                throw new InvalidOperationException("Unable to subtract sub range");
            }
            
            int newOffset;
            int newLength;

            if (right.EndOffset < left.EndOffset)
            {
                newOffset = right.EndOffset + 1;
                newLength = left.EndOffset - right.EndOffset;
            }
            else
            {
                newOffset = left.Offset;
                newLength = right.Offset - left.Offset;
            }

            return new Range(newOffset, newLength);
        }

        public static Range operator +(Range left, Range right)
        {
            if (left.EndOffset + 1 < right.Offset || right.EndOffset + 1 < left.Offset)
            {
                throw new InvalidOperationException("Unable to add sub range");
            }

            int newOffset;
            int newLength;

            if (right.Offset < left.EndOffset)
            {
                newOffset = right.Offset;
                newLength = left.EndOffset - right.Offset;
            }
            else
            {
                newOffset = left.Offset;
                newLength = right.EndOffset - left.Offset;
            }

            return new Range(newOffset, newLength + 1);
        }

        /// <inheritdoc />
        public override string ToString() => $"[{Offset}, {EndOffset}]";
    }
}