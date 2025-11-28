using UnityEngine;

namespace MVVMToolkit.DataBinding
{
    public class ImageResource : ObservableObject
    {
        private AddressableResource? addressable;
        public AddressableResource? Addressable
        {
            get => addressable;
            set
            {
                addressable = value;
                OnPropertyChanged();
            }
        }

        private Sprite? sprite;
        public Sprite? Sprite
        {
            get => sprite;
            set
            {
                sprite = value;
                OnPropertyChanged();
            }
        }

        public ImageResource(AddressableResource addressable)
        {
            Addressable = addressable;
        }

        public ImageResource(Sprite sprite)
        {
            Sprite = sprite;
        }
    }
}
