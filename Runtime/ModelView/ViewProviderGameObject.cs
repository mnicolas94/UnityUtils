using System.Collections.Generic;
using UnityEngine;

namespace Utils.ModelView
{
    public class ViewProviderGameObject : MonoBehaviour, IViewProvider
    {
        [SerializeField] private List<IView> _views;
        
        public IView TryGetViewForModel(IModel model, out bool exists)
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