using UnityEngine;
using MVVMToolkit.DataBinding;

namespace MVVMToolkit.UI
{
    [RequireComponent(typeof(Canvas))]
    public class CanvasMultiBinder : MultiBinder
    {
        private bool? overrideSorting = null;
        private int? sortOrder = null;
        private Canvas canvas;
        public Canvas Canvas => canvas ??= GetComponent<Canvas>();

        public override void InitValue(string sourcePath, int sourceIndex, object value)
        {
            UpdateValue(sourcePath, sourceIndex, value);
        }

        public override void UpdateValue(string sourcePath, int sourceIndex, object value)
        {
            if (value != null)
            {
                switch (sourcePath)
                {
                    case "overrideSorting":
                        overrideSorting = bool.Parse(value.ToString());
                        break;
                    case "sortOrder":
                        sortOrder = int.Parse(value.ToString());
                        break;
                }
            }

            UpdateValues();
        }

        private void UpdateValues()
        {
            if (overrideSorting.HasValue)
                Canvas.overrideSorting = overrideSorting.Value;

            if (sortOrder.HasValue && Canvas.overrideSorting)
                Canvas.sortingOrder = sortOrder.Value;
        }
    }
}
