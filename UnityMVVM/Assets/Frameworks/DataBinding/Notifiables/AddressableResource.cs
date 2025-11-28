
namespace MVVMToolkit.DataBinding
{
    public class AddressableResource : ObservableObject
    {
        private string path;
        public string Path
        {
            get => path;
            set
            {
                path = value;
                OnPropertyChanged();
            }
        }

        public AddressableResource()
        {

        }

        public AddressableResource(string path)
        {
            Path = path;
        }
    }
}
