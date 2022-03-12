using System.Linq;
using Coding4fun.PainlessUtils;
using NUnit.Framework;

namespace Coding4fun.PainlessString.Test
{
    public class CompactArrayTests
    {
        /// <summary>
        /// Required 4 bits.
        /// </summary>
        private enum Bits4Number
        {
            Zero,
            One,
            Two,
            Three,
            Four,
            Five,
            Six,
            Seven,
            Eight,
            Nine,
            Ten
        }

        /// <summary>
        /// Required 3 bits.
        /// </summary>
        private enum Bits3Number
        {
            Zero,
            One,
            Two,
            Three,
            Four,
            Five,
            Six,
            Seven
        }
        
        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(3, 2)]
        [TestCase(4, 3)]
        [TestCase(5, 3)]
        [TestCase(6, 3)]
        [TestCase(7, 3)]
        [TestCase(8, 4)]
        [TestCase(9, 4)]
        [TestCase(16, 5)]
        public void GetBitesCount(int enumCapacity, int expectedBitesCount)
        {
            int actualBitesCount = EnumCompactSerializer<CharKind>.GetBitsCount(enumCapacity);
            Assert.AreEqual(expectedBitesCount, actualBitesCount);
        }

        [Test]
        public void Bits4()
        {
            EnumCompactSerializer<Bits4Number> enumCompactSerializer = new();
            Bits4Number[] sourceNumbers = {
                Bits4Number.Zero,
                Bits4Number.One,
                Bits4Number.Two,
                Bits4Number.Three,
                Bits4Number.Four,
                Bits4Number.Five,
                Bits4Number.Six,
                Bits4Number.Seven,
                Bits4Number.Eight,
                Bits4Number.Nine,
                Bits4Number.Ten
            };

            byte[] expectedArray = {
                0b00000001, // 0, 1
                0b00100011, // 2, 3
                0b01000101, // 4, 5
                0b01100111, // 6, 7
                0b10001001, // 8, 9
                0b10100000  // 10
            };
            
            CompactArray<Bits4Number> compactArray = new (sourceNumbers, enumCompactSerializer);
            byte[] compressedData = compactArray.CompressedData;
            CollectionAssert.AreEqual(expectedArray, compressedData);
            
            Bits4Number[] deserializedNumbers = compactArray.ToArray();
            CollectionAssert.AreEqual(sourceNumbers, deserializedNumbers);
        }
        
        [Test]
        public void Bits3()
        {
            EnumCompactSerializer<Bits3Number> enumCompactSerializer = new();
            Bits3Number[] sourceNumbers = {
                Bits3Number.Zero,
                Bits3Number.One,
                Bits3Number.Two,
                Bits3Number.Three,
                Bits3Number.Four,
                Bits3Number.Five,
                Bits3Number.Six,
                Bits3Number.Seven
            };

            byte[] expectedArray = {
                0b00000101, // 000_001_01  -> 0, 1
                0b00111001, // 0_011_100_1 -> 2, 3, 4
                0b01110111  // 01_110_111  -> 5, 6, 7
            };
            
            CompactArray<Bits3Number> compactArray = new (sourceNumbers, enumCompactSerializer);
            byte[] compressedData = compactArray.CompressedData;
            CollectionAssert.AreEqual(expectedArray, compressedData);

            Bits3Number[] deserializedNumbers = compactArray.ToArray();
            CollectionAssert.AreEqual(sourceNumbers, deserializedNumbers);
        }
    }
}