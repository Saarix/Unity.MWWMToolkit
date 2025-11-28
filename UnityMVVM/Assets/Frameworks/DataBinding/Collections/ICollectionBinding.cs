
using System.Collections.Generic;

namespace MVVMToolkit.DataBinding
{
    public interface ICollectionBinding 
    {
        Dictionary<object, CollectionViewRef> GetPrefabs();
    }
}
