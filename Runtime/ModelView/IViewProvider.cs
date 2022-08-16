﻿namespace Utils.ModelView
{
    public interface IViewProvider<T>
    {
        IView<T> TryGetViewForModel(T model, out bool exists);
    }
}