namespace MVVMToolkit.DataBinding
{
    public class Progress : ObservableObject
    {
        private int currentValue;
        public int CurrentValue
        {
            get => currentValue;
            set
            {
                currentValue = value;
                OnPropertyChanged();
            }
        }

        private int maxValue;
        public int MaxValue
        {
            get => maxValue;
            set
            {
                maxValue = value;
                OnPropertyChanged();
            }
        }

        private float? value;
        public float? Value
        {
            get => value;
            set
            {
                this.value = value;
                OnPropertyChanged();
            }
        }

        private bool shouldAnimate;
        public bool ShouldAnimate
        {
            get => shouldAnimate;
            set
            {
                shouldAnimate = value;
                OnPropertyChanged();
            }
        }

        public void Update(float value, int currentValue, int maxValue, bool shouldAnimate = false)
        {
            Value = value;
            CurrentValue = currentValue;
            MaxValue = maxValue;
            ShouldAnimate = shouldAnimate;
        }

        public void Update(int currentValue, int maxValue, bool shouldAnimate = false)
        {
            Value = (float)currentValue / maxValue;
            CurrentValue = currentValue;
            MaxValue = maxValue;
            ShouldAnimate = shouldAnimate;
        }

        public Progress(float value, int currentValue, int maxValue, bool shouldAnimate = false)
        {
            Update(value, currentValue, maxValue, shouldAnimate);
        }

        public Progress(int currentValue, int maxValue, bool shouldAnimate = false)
        {
            Update(currentValue, maxValue, shouldAnimate);
        }

        public Progress(float value, bool shouldAnimate = false)
        {
            Value = value;
            ShouldAnimate = shouldAnimate;
        }
    }
}
