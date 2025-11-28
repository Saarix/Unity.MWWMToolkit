using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MVVMToolkit.DataBinding;

namespace MVVMToolkit.UI
{
    public class ProgressActivationMultiBinder : MultiBinder
    {
        [SerializeField]
        private bool hideFutureProgress = false;

        [SerializeField]
        private RectTransform container;

        [SerializeField]
        private GameObject prefab;

        private List<GameObject> views = new();
        private int currentValue = 0;

        #region Overrides

        public override void InitValue(string sourcePath, int sourceIndex, object value)
        {
            base.InitValue(sourcePath, sourceIndex, value);
            UpdateValue(sourcePath, sourceIndex, value);
        }

        public override void UpdateValue(string sourcePath, int sourceIndex, object value)
        {
            base.UpdateValue(sourcePath, sourceIndex, value);

            if (value != null)
            {
                switch (sourcePath)
                {
                    case "value":
                        {
                            if (int.TryParse(value.ToString(), out int result))
                            {
                                currentValue = result;
                                Refresh(result);
                            }
                        }

                        break;
                    case "max":
                        {
                            if (int.TryParse(value.ToString(), out int result))
                            {
                                if (result == views.Count)
                                    return;

                                // Clear any prev. items
                                foreach (GameObject item in views)
                                    ReleaseView(item);

                                for (int i = 0; i < result; i++)
                                    views.Add(CreateView());

                                Refresh(currentValue);
                            }
                        }

                        break;
                }
            }
        }

        #endregion

        public void Refresh(int value)
        {
            for (int i = 0; i < views.Count; i++)
            {
                Transform childTra = views[i].transform;

                if (hideFutureProgress)
                    views[i].SetActive(i <= (value - 1));

                if (childTra.childCount == 2)
                {
                    GameObject empty = childTra.GetChild(0).gameObject;
                    GameObject filled = childTra.GetChild(1).gameObject;

                    empty.SetActive(i > (value - 1));
                    filled.SetActive(i <= (value - 1));
                }
            }
        }

        private GameObject CreateView()
        {
            // Create new view
            GameObject view;
            try
            {
                view = Instantiate(prefab, container);
            }
            catch
            {
                // Construct dummy prefab
                view = new GameObject();
                Image image = view.AddComponent<Image>();
                image.raycastTarget = false;
                image.color = Color.cyan;
                view.transform.SetParent(container);
            }

            // Set hierarchy
            view.transform.SetAsLastSibling();
            view.name = "[UI] " + prefab.name;

            // Active view if not
            if (!view.activeSelf)
                view.SetActive(true);

            return view;
        }

        private void ReleaseView(GameObject view)
        {
            Destroy(view);
        }
    }
}
