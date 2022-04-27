﻿using UnityEngine;

 namespace Utils.ModelView
{
    public abstract class ViewBase<TM> : MonoBehaviour, IView where TM : IModel
    {
        public abstract bool CanRenderModel(TM model);
        public abstract void Initialize(TM model);
        public abstract void UpdateView(TM model);
        
        public bool CanRenderModel(IModel model)
        {
            if (model is TM tModel)
            {
                return CanRenderModel(tModel);
            }

            return false;
        }

        public void Initialize(IModel model)
        {
            if (model is TM tModel)
            {
                Initialize(tModel);
            }
        }

        public void UpdateView(IModel model)
        {
            if (model is TM tModel)
            {
                UpdateView(tModel);
            }
        }
    }
}