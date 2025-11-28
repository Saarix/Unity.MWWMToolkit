using System;
using MVVMToolkit.DataBinding;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace MVVMToolkit.UI
{
    public class StringCountupMultiBinder : MultiBinder
    {
        [SerializeField] private bool shouldLocalize = false;

        [Header("Objects enabled when finished")]
        [SerializeField] private GameObject[] enabledOnFinished;

        [Header("Events")] public UnityEvent OnFinished;

        private TextMeshProUGUI labelValue;
        private bool isUpdating = false;
        private DateTime? targetTime = null;
        private DateTime? startTime = null;

        public TimeSpan CurrentTime => DateTime.UtcNow - (startTime ?? default);

        #region Overrides

        protected override void Awake()
        {
            base.Awake();

            labelValue = GetComponent<TextMeshProUGUI>();
        }

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
                switch (sourceIndex)
                {
                    case 0:
                    {
                        if (DateTime.TryParse(value.ToString(), out DateTime result))
                        {
                            startTime = result;
                            isUpdating = true;
                        }
                    }

                    break;
                    case 1:
                    {
                        if (DateTime.TryParse(value.ToString(), out DateTime result))
                        {
                            targetTime = result;
                        }
                    }

                    break;
                }
            }
        }

        #endregion

        protected override void OnEnable()
        {
            base.OnEnable();

            if (startTime.HasValue)
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
                Refresh(CurrentTime);

                if (targetTime.HasValue && targetTime.Value <= DateTime.UtcNow)
                {
                    isUpdating = false;

                    Refresh((targetTime.Value - startTime.Value));
                    OnFinished?.Invoke();

                    targetTime = null;
                    startTime = null;

                    if (enabledOnFinished != null)
                    {
                        // First disable and then re-enable so they get always invoked
                        foreach (GameObject go in enabledOnFinished)
                            go.SetActive(false);

                        foreach (GameObject go in enabledOnFinished)
                            go.SetActive(true);
                    }
                }
            }
        }

        public void Refresh(TimeSpan timeSpan)
        {
            labelValue.text = $"{(int)timeSpan.TotalHours:0}:{timeSpan.ToString(@"mm\:ss")}";
        }
    }
}
