using System;
using TMPro;
using UnityEngine;
using MVVMToolkit.DataBinding;

namespace MVVMToolkit.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class StringProgressMultiBinder : MultiBinder
    {
        [SerializeField]
        private Color progressColor;

        [SerializeField]
        private Color finishedColor;

        private TextMeshProUGUI labelText;
        public TextMeshProUGUI LabelText => labelText ?? (labelText = GetComponent<TextMeshProUGUI>());

        private bool isUpdating = false;
        private DateTime? targetTime = null;
        private DateTime? startTime = null;

        public TimeSpan ElapsedTime => DateTime.UtcNow - (startTime ?? default);

        #region Overrides

        public override void InitValue(string sourcePath, int sourceIndex, object value)
        {
            UpdateValue(sourcePath, sourceIndex, value);
        }

        public override void UpdateValue(string sourcePath, int sourceIndex, object value)
        {
            if (value != null)
            {
                if (DateTime.TryParse(value.ToString(), out DateTime result))
                {
                    switch (sourcePath)
                    {
                        case "started_at":
                            startTime = result;
                            break;
                        case "end_at":
                            targetTime = result;
                            break;
                    }

                    if (startTime.HasValue && targetTime.HasValue)
                    {
                        // Initial refresh
                        Refresh();
                        isUpdating = true;
                    }
                }
            }
        }

        #endregion

        protected override void OnEnable()
        {
            base.OnEnable();

            if (targetTime.HasValue)
                isUpdating = true;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            isUpdating = false;
        }

        private void Update()
        {
            if (isUpdating)
            {
                if (ElapsedTime < (targetTime - startTime))
                {
                    Refresh();
                }
                else
                {
                    isUpdating = false;
                    targetTime = null;
                }
            }
        }

        public void Refresh()
        {
            // Clamp
            TimeSpan result = ElapsedTime;
            if (result > (targetTime - startTime))
            {
                LabelText.color = finishedColor;
                result = targetTime.Value - startTime.Value;
            }
            else
            {
                LabelText.color = progressColor;
            }

            LabelText.text = $"{(result.Days * 24) + result.Hours}:{result.ToString("mm\\:ss")}";
        }
    }
}
