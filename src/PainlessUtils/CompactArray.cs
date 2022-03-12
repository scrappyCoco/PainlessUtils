using System;
using System.Collections;
using System.Collections.Generic;

namespace Coding4fun.PainlessUtils
{
    public class CompactArray<TEntity> : IReadOnlyCollection<TEntity>, ICompactArray
    {
        private const int BitsInByteCount = 8;
        private readonly int _bitMask;
        private readonly int _bitsCountPerEntity;
        private readonly ICompactSerializer<TEntity> _serializer;
        private int _availableCount = BitsInByteCount;
        private int _currentByte;
        private int _currentByteNumber;

        public CompactArray(TEntity[] data, ICompactSerializer<TEntity> serializer)
        {
            _serializer = serializer;
            _bitsCountPerEntity = _serializer.BitsCountPerEntity;
            Count = data.Length;
            int bytesCount = (int)Math.Ceiling(_bitsCountPerEntity * Count / 8.0);
            _bitMask = (1 << _bitsCountPerEntity) - 1;
            CompressedData = new byte[bytesCount];

            for (int itemNumber = 0; itemNumber < Count; ++itemNumber)
            {
                TEntity entity = data[itemNumber];
                _serializer.Serialize(entity, this);
            }

            if (_availableCount != BitsInByteCount) CompressedData[_currentByteNumber] = (byte)_currentByte;
        }

        internal byte[] CompressedData { get; }

        /// <inheritdoc />
        void ICompactArray.Write(byte data)
        {
            // capacity = 3 bits
            // 101 010 10 | 1 010 101 0 | 10 101 010

            int delta = _availableCount - _bitsCountPerEntity;

            if (data != 0)
            {
                if (delta == 0)
                {
                    _currentByte |= data;
                }
                else if (delta > 0)
                {
                    _currentByte |= data << delta;
                }
                else
                {
                    int pDelta = -delta;
                    _currentByte |= data >> pDelta;
                }
            }


            if (delta <= 0)
            {
                CompressedData[_currentByteNumber] = (byte)_currentByte;
                ++_currentByteNumber;
                _currentByte = 0;
            }

            if (delta < 0)
            {
                int shift = BitsInByteCount + delta;
                if (data != 0) _currentByte = data << shift;
            }

            _availableCount = delta <= 0 ? BitsInByteCount + delta : delta;
        }

        /// <inheritdoc />
        public IEnumerator<TEntity> GetEnumerator()
        {
            // capacity = 3 bits
            // 101 010 10 | 1 010 101 0 | 10 101 010
            for (int entityNumber = 0; entityNumber < Count; ++entityNumber)
            {
                int leftOffset = entityNumber * _serializer.BitsCountPerEntity;
                int rightOffset = (entityNumber + 1) * _serializer.BitsCountPerEntity;
                int leftByteNumber = leftOffset / 8;
                int rightByteNumber = (rightOffset - 1) / 8;
                int delta = BitsInByteCount - rightOffset % 8;
                int data = CompressedData[leftByteNumber];
                if (delta == 0 || delta == BitsInByteCount)
                {
                    data &= _bitMask;
                }
                else if (leftByteNumber == rightByteNumber)
                {
                    data = (data >> delta) & _bitMask;
                }
                else
                {
                    int lDelta = BitsInByteCount - delta;
                    int rDelta = BitsInByteCount - rightOffset % BitsInByteCount;
                    data = ((data << lDelta) | (CompressedData[rightByteNumber] >> rDelta)) & _bitMask;
                }

                TEntity entity = _serializer.Deserialize((byte)data);
                yield return entity;
            }
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <inheritdoc />
        public int Count { get; }
    }
}