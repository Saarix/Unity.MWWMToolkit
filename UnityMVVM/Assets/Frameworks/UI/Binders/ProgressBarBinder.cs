using MVVMToolkit.DataBinding;
using UnityEngine;

namespace MVVMToolkit.UI
{
    public enum ProgressBarAnimation
    {
        Fill,
        Preview
    }

    [RequireComponent(typeof(ProgressBar))]
    public class ProgressBarBinder : Binder<ProgressBar>
    {
        [SerializeField] private bool usePreviewAnimation = false;
        [SerializeField] private ProgressBarAnimation progressBarAnimation = ProgressBarAnimation.Fill;
        [SerializeField] private float animationDelay = 0f;
        [SerializeField] private float animationDuration = 0.5f;
        [SerializeField] private bool useMinValue = false;

        private float? percentage = null;
        private int? minValue = null;
        private int? oldValue = null;
        private int? currentValue = null;
        private int? maxValue = null;
        private bool? shouldAnimate = null;

        public bool UsePreviewAnimation { get => usePreviewAnimation; set => usePreviewAnimation = value; }

        #region Overrides

        public override void InitValue(object value)
        {
            base.InitValue(value);
            UpdateValue(value);
        }

        public override void UpdateValue(object value)
        {
            base.UpdateValue(value);

            if (value != null && value is Progress progress)
            {
                shouldAnimate = progress.ShouldAnimate;

                if (progress.Value.HasValue)
                {
                    percentage = progress.Value.Value;
                }

                currentValue = progress.CurrentValue;
                maxValue = progress.MaxValue;

                Refresh();
            }
        }

        #endregion

        public void Refresh()
        {
            if (percentage.HasValue)
                Component.SetProgress(percentage.Value);

            if (usePreviewAnimation || (shouldAnimate.HasValue && shouldAnimate.Value))
            {
                if (currentValue.HasValue && maxValue.HasValue && oldValue.HasValue)
                {
                    switch (progressBarAnimation)
                    {
                        case ProgressBarAnimation.Fill:
                        {
                            if (useMinValue && minValue.HasValue)
                                Component.SetProgressAnimFill(currentValue.Value, maxValue.Value, oldValue.Value, animationDelay, animationDuration, minValue.Value);
                            else if (!useMinValue)
                                Component.SetProgressAnimFill(currentValue.Value, maxValue.Value, oldValue.Value, animationDelay, animationDuration);
                            break;
                        }
                        case ProgressBarAnimation.Preview:
                        {
                            if (useMinValue && minValue.HasValue)
                                Component.SetProgressAnimPreview(currentValue.Value, maxValue.Value, oldValue.Value, animationDelay, animationDuration, minValue.Value);
                            else if (!useMinValue)
                                Component.SetProgressAnimPreview(currentValue.Value, maxValue.Value, oldValue.Value, animationDelay, animationDuration);
                            break;
                        }
                    }
                }
            }
            else
            {
                if (currentValue.HasValue && maxValue.HasValue)
                {
                    if (oldValue.HasValue)
                    {
                        if (useMinValue && minValue.HasValue)
                            Component.SetProgress(currentValue.Value, maxValue.Value, oldValue.Value, minValue.Value);
                        else if (!useMinValue)
                            Component.SetProgress(currentValue.Value, maxValue.Value, oldValue.Value);
                    }
                    else
                    {
                        if (useMinValue && minValue.HasValue)
                            Component.SetProgress(currentValue.Value, maxValue.Value, minValue.Value);
                        else if (!useMinValue)
                            Component.SetProgress(currentValue.Value, maxValue.Value);
                    }
                }
            }
        }
    }
}
