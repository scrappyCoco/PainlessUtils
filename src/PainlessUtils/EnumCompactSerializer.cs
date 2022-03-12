using System;
using System.Globalization;

namespace Coding4fun.PainlessUtils
{
    public class EnumCompactSerializer<TEnum>: ICompactSerializer<TEnum> where TEnum : Enum
    {
        public EnumCompactSerializer()
        {
            int enumValuesCount = Enum.GetValues(typeof(TEnum)).Length - 1;
            if (enumValuesCount > 255) throw new ArgumentException("Enum must contains less than 255 items.");
            BitsCountPerEntity = GetBitsCount(enumValuesCount);
        }

        /// <inheritdoc />
        public void Serialize(TEnum entity, ICompactArray compactArray)
        {
            byte byteValue = ((IConvertible)entity).ToByte(CultureInfo.InvariantCulture);
            compactArray.Write(byteValue);
        }

        /// <inheritdoc />
        public TEnum Deserialize(byte data) => (TEnum)Enum.ToObject(typeof(TEnum), data);

        /// <inheritdoc />
        public int BitsCountPerEntity { get; }

        internal static int GetBitsCount(int enumCapacity)
        {
            if (enumCapacity <= 0) throw new ArgumentOutOfRangeException(nameof(enumCapacity));
            double bitesPerItem = Math.Log(enumCapacity, 2d);
            bitesPerItem = Math.Floor(bitesPerItem);
            return 1 + (int)bitesPerItem;
        }
    }
}