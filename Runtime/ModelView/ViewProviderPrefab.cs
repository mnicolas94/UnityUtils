using System.Collections.Generic;
using UnityEngine;

namespace Utils.ModelView
{
    public abstract class ViewProviderPrefab<TM> : ScriptableObject, IViewProvider<TM>
    {
        [SerializeField] private List<ViewBase<TM>> _viewsPrefabs;

        public IView<TM> TryGetViewForModel(TM model, out bool exists)
        {
            return TryGetViewPrefabForModel(model, out exists);
        }
        
        public ViewBase<TM> TryGetViewPrefabForModel(TM model, out bool exists)
        {
            foreach (var viewPrefab in _viewsPrefabs)
            {
                if (viewPrefab.CanRenderModel(model))
                {
                    exists = true;
                    var view = Instantiate(viewPrefab);
                    view.Initialize(model);
                    return view;
                }
            }

            exists = false;
            return null;
        }
    }
}