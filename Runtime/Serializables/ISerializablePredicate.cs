﻿namespace Utils.Serializables
{
    public interface ISerializablePredicate
    {
        bool IsMet();
    }
    
    public interface ISerializablePredicate<T>
    {
        bool IsMet(T input);
    }
    
    public interface ISerializablePredicate<T1, T2>
    {
        bool IsMet(T1 i1, T2 i2);
    }
    
    public interface ISerializablePredicate<T1, T2, T3>
    {
        bool IsMet(T1 i1, T2 i2, T3 i3);
    }
    
    public interface ISerializablePredicate<T1, T2, T3, T4>
    {
        bool IsMet(T1 i1, T2 i2, T3 i3, T4 i4);
    }
    
    public interface ISerializablePredicate<T1, T2, T3, T4, T5>
    {
        bool IsMet(T1 i1, T2 i2, T3 i3, T4 i4, T5 i5);
    }
}