using UnityEngine;

namespace MVVMToolkit.DataBinding
{
    public interface IViewFactory
    {
        GameObject CreateView(GameObject viewPrefab = null, int index = -1);
        void ReleaseView(GameObject view);
    }
}