using UnityEngine;

namespace MVVMToolkit.DataBinding
{
    [RequireComponent(typeof(IDataContext))]
    public class DataContextBinder : Binder
    {
        private IDataContext target;
        public IDataContext Target => target ?? (target = GetComponent<IDataContext>());

        public override void InitValue(object value)
        {
            base.InitValue(value);
            UpdateValue(value);
        }

        // Note : this should be fixed by now, we need to try this approach

        // Debt: MAJOR! sub data context with data context binder doesn't work at this moment
        // Had an issue with commands inside the sub context being null and then getting error of already destroyed object access
        // make it work so we can split data models into multiple parts and work with sub contexts

        public override void UpdateValue(object value)
        {
            base.UpdateValue(value);

            if (Target == null)
                return;

            Target.Data = value;
        }
    }
}
