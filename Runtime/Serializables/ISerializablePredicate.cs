﻿namespace Utils.Serializables
{
    public interface ISerializablePredicate<T>
    {
        bool IsMet(T input);
    }
}