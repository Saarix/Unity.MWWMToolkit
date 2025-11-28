namespace MVVMToolkit.UI
{
    public class StarItemModel : Model
    {
        private int state;
        public int State
        {
            get => state;
            set
            {
                state = value;
                OnPropertyChanged();
            }
        }
    }
}
