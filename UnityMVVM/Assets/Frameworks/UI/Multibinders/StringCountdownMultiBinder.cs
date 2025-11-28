using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization.Components;
using TMPro;
using Geewa.Framework;
using MVVMToolkit.DataBinding;

namespace MVVMToolkit.UI
{
    public class StringCountdownMultiBinder : MultiBinder
    {
        [SerializeField]
        private string stringFormat;

        [SerializeField]
        private bool shouldLocalize = false;

        [Header("Objects enabled when finished")]
        [SerializeField]
        private GameObject[] enabledOnFinished;

        [Header("Events")]
        public UnityEvent OnFinished;

        private TextMeshProUGUI labelValue;
        private bool isUpdating = false;
        private DateTime? targetTime = null;
        private LocalizeStringEvent localizeStringEvent;

        public TimeSpan RemainingTime => (targetTime ?? default) - DateTime.UtcNow;
        public string LocalizedText { get; set; }

        #region Overrides

        protected override void Awake()
        {
            base.Awake();

            localizeStringEvent = GetComponent<LocalizeStringEvent>();
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
                    // TODO: Edit this so targetTime doesn't need to have index 0 and it can be used for text again.
                    //case "text":
                    //{
                    //    localizeStringEvent.SetEntry(value.ToString());
                    //    localizeStringEvent.RefreshString();

                    //    LocalizedText = localizeStringEvent.StringReference.GetLocalizedString();
                    //}
                    //    break;
                    case 0:
                    {
                        if (DateTime.TryParse(value.ToString(), out DateTime result))
                        {
                            if ((result - DateTime.UtcNow).TotalSeconds > 0)
                            {
                                targetTime = result;
                                isUpdating = true;
                            }
                            else
                            {
                                Refresh("0s");
                            }
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
                if (RemainingTime.TotalSeconds > 0)
                {
                    Refresh(RemainingTime.FormatTimeSpan());
                }
                else
                {
                    isUpdating = false;

                    Refresh("0s");
                    OnFinished?.Invoke();

                    targetTime = null;

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

        public void Refresh(string timeString)
        {
            if (!string.IsNullOrEmpty(stringFormat))
                labelValue.text = string.Format(stringFormat, LocalizedText, timeString);
            else
                labelValue.text = $"{LocalizedText} {timeString}";
        }
    }
}
