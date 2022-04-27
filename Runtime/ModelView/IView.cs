﻿namespace Utils.ModelView
{
    public interface IView
    {
        bool CanRenderModel(IModel model);
        void Initialize(IModel model);
        void UpdateView(IModel model);
    }
}