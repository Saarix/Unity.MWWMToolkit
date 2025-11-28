using System.Collections;
using System.Collections.Generic;
using MVVMToolkit.DataBinding;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace MVVMToolkit.UI
{
    [RequireComponent(typeof(Image))]
    public class ImageBinder : Binder<Image>
    {
        [SerializeField] private bool ignoreTimeScale;
        [SerializeField] private bool releaseHandlesOnDestroyOnly;

        [Header("Optional")]
        [SerializeField] private Spinner spinner;

        private List<AsyncOperationHandle<Sprite>> handles = new();
        private AddressableResource addressableResource;
        private Coroutine resourceCoroutine;

        #region Binder implementation

        public override void InitValue(object value)
        {
            Component.enabled = false;

            base.InitValue(value);
            UpdateValue(value);
        }

        public override void UpdateValue(object value)
        {
            base.UpdateValue(value);

            if (value == null)
            {
                Debug.LogError($"[{GetType().Name}] Value cannot be null.");
                return;
            }

            if (value is ImageResource resource)
            {
                if (resource.Addressable != null)
                {
                    value = resource.Addressable;
                }
                else if (resource.Sprite != null)
                {
                    value = resource.Sprite;
                }
                else
                {
                    Debug.LogError($"[{GetType().Name}] Image Source has both properties null, cannot get valid image.");
                    return;
                }
            }

            if (resourceCoroutine != null)
            {
                StopCoroutine(resourceCoroutine);
                resourceCoroutine = null;
            }

            if (value is Sprite sprite)
            {
                if (spinner != null)
                    spinner.gameObject.SetActive(false);

                Component.enabled = true;
                Component.sprite = sprite;
                Component.CrossFadeAlpha(1f, 0.1f, ignoreTimeScale);
            }
            else if (value is AddressableResource addressableResource)
            {
                if (string.IsNullOrEmpty(addressableResource.Path))
                {
                    Debug.LogError($"[{GetType().Name}] Addressable resource cannot have empty path. Path:{Binding.Path}, gameObject: {gameObject.name}, parent: {gameObject.transform.parent.name}");
                    return;
                }

                if (spinner != null)
                    spinner.gameObject.SetActive(true);

                this.addressableResource = addressableResource;
                Component.enabled = true;
                Component.CrossFadeAlpha(0f, 0f, ignoreTimeScale);

                resourceCoroutine = StartCoroutine(ResourceLoad());
            }
            else
            {
                Debug.LogError($"[{GetType().Name}] Trying to use different type than Sprite. value is {value}, gameObject: {gameObject.name}");
            }
        }

        #endregion Binder implementation

        private IEnumerator ResourceLoad()
        {
            Sprite result = null;
            AsyncOperationHandle<Sprite> handle = Addressables.LoadAssetAsync<Sprite>(addressableResource.Path);
            handles.Add(handle);

            while (!handle.IsDone)
                yield return null;

            if (handle.IsValid() && handle.IsDone)
                result = handle.Result;
            else
                Debug.LogError($"[{GetType().Name}] Handle is not valid for asset with for path: {addressableResource.Path}");

            if (spinner != null)
                spinner.gameObject.SetActive(false);

            Component.enabled = true;
            Component.sprite = result;
            Component.CrossFadeAlpha(1f, 0.1f, ignoreTimeScale);

            resourceCoroutine = null;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            if (!releaseHandlesOnDestroyOnly)
            {
                if (handles.Count > 0)
                {
                    foreach (AsyncOperationHandle<Sprite> handle in handles)
                    {
                        if (handle.IsValid())
                            Addressables.Release(handle);
                    }

                    handles.Clear();
                }
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            // In case object gets destroyed while disabled, we still ahve to release handles
            if (handles.Count > 0)
            {
                foreach (AsyncOperationHandle<Sprite> handle in handles)
                {
                    if (handle.IsValid())
                        Addressables.Release(handle);
                }

                handles.Clear();
            }
        }
    }
}
