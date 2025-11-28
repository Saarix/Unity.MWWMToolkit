
namespace MVVMToolkit.DataBinding
{
    public class LocalizableString : ObservableObject
    {
        public static LocalizableString Empty => new("general.emptyText");

        private string key;
        public string Key
        {
            get => key;
            set
            {
                key = value;
                OnPropertyChanged();
            }
        }

        private string table;
        public string Table
        {
            get => table;
            set
            {
                table = value;
                OnPropertyChanged();
            }
        }

        private (string Key, string Value)[] tokens;
        public (string Key, string Value)[] Tokens
        {
            get => tokens;
            set
            {
                tokens = value;
                OnPropertyChanged();
            }
        }

        public LocalizableString(string key, params (string Key, string Value)[] tokens)
        {
            Key = key;
            Tokens = tokens;
        }

        public LocalizableString(string key, string table = null, params (string Key, string Value)[] tokens)
        {
            Key = key;
            Table = table;
            Tokens = tokens;
        }
    }
}
