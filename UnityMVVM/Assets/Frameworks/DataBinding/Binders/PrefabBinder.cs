using UnityEngine;

namespace MVVMToolkit.DataBinding
{
    public class PrefabBinder : Binder
    {
        [SerializeField] private Transform container;
        [SerializeField] private bool loadDataIntoDataContext = false;

        private GameObject instance;

        public override void InitValue(object value)
        {
            base.InitValue(value);
            UpdateValue(value);
        }

        public override void UpdateValue(object value)
        {
            base.UpdateValue(value);

            if (instance != null)
            {
                Destroy(instance);
                instance = null;
            }

            if (value != null)
            {
                if (value is GameObject go)
                {
                    SpawnGameObject(go);
                }
                else
                {
                    GameObject prefab = null;
                    object prefabProperty = BindingUtility.GetPropertyValue(value, "Prefab", true);
                    if (prefabProperty != null && prefabProperty is GameObject goProperty)
                        prefab = goProperty;

                    if (prefab != null)
                    {
                        SpawnGameObject(prefab);

                        if (loadDataIntoDataContext)
                            instance.GetComponent<DataContext>().Data = value;
                    }
                }
            }
        }

        private void SpawnGameObject(GameObject go)
        {
            instance = Instantiate(go, container);
            instance.transform.localPosition = Vector3.zero;
            instance.name = $"[Prefab] {go.name}";
        }
    }
}
