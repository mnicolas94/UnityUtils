﻿namespace Utils.ModelView
{
    public interface IView<T>
    {
        bool CanRenderModel(T model);
        void Initialize(T model);
        void UpdateView(T model);
    }
}