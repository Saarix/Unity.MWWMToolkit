using MVVMToolkit.DataBinding;
using UnityEngine;

namespace MVVMToolkit.UI
{
    public class ItemBonusModel : Model
    {
        private bool isActive;
        public bool IsActive
        {
            get => isActive;
            set
            {
                isActive = value;
                OnPropertyChanged();
            }
        }

        private Color backgroundColor;
        public Color BackgroundColor
        {
            get => backgroundColor;
            set
            {
                backgroundColor = value;
                OnPropertyChanged();
            }
        }

        private Color textColor;
        public Color TextColor
        {
            get => textColor;
            set
            {
                textColor = value;
                OnPropertyChanged();
            }
        }

        private Color backgroundShineColor;
        public Color BackgroundShineColor
        {
            get => backgroundShineColor;
            set
            {
                backgroundShineColor = value;
                OnPropertyChanged();
            }
        }

        private LocalizableString bonusText;
        public LocalizableString BonusText
        {
            get => bonusText;
            set
            {
                bonusText = value;
                OnPropertyChanged();
            }
        }
    }
}
