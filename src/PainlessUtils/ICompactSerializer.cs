using System;

namespace Coding4fun.PainlessUtils
{
    public interface ICompactSerializer<TEntity>
    {
        void Serialize(TEntity entity, ICompactArray compactArray);
        TEntity Deserialize(byte data);
        int BitsCountPerEntity { get; }
    }
}