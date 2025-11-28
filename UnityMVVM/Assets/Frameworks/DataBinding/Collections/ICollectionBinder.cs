using System;

namespace MVVMToolkit.DataBinding
{
    public interface ICollectionBinder : IBinder 
    {
        event Action OnItemsAdded;
        event Action OnItemsRemoved;
        void ItemsAdded();
        void ItemsRemoved();
    }
}
