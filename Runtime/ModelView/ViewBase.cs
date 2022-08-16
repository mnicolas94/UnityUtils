﻿using UnityEngine;

 namespace Utils.ModelView
{
    public abstract class ViewBase<TM> : MonoBehaviour, IView<TM>
    {
        public abstract bool CanRenderModel(TM model);
        public abstract void Initialize(TM model);
        public abstract void UpdateView(TM model);
    }
}