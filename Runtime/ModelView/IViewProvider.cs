﻿namespace Utils.ModelView
{
    public interface IViewProvider
    {
        IView TryGetViewForModel(IModel model, out bool exists);
    }
}