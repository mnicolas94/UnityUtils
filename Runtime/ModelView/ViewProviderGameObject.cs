using System.Collections.Generic;
using UnityEngine;

namespace Utils.ModelView
{
    public abstract class ViewProviderGameObject<T> : MonoBehaviour, IViewProvider<T>
    {
        [SerializeField] private List<IView<T>> _views;
        
        public IView<T> TryGetViewForModel(T model, out bool exists)
        {
            foreach (var view in _views)
            {
                if (view.CanRenderModel(model))
                {
                    exists = true;
                    view.Initialize(model);
                    return view;
                }
            }

            exists = false;
            return null;
        }
    }
}