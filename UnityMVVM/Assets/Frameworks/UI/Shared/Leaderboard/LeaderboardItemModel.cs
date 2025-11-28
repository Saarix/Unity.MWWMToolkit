using MVVMToolkit.DataBinding;
using UnityEngine;

namespace MVVMToolkit.UI
{
    public class LeaderboardItemModel : Model
    {
        private GameObject prefab;
        public GameObject Prefab
        {
            get => prefab;
            set
            {
                prefab = value;
                OnPropertyChanged();
            }
        }

        private int position;
        public int Position
        {
            get => position;
            set
            {
                position = value;
                OnPropertyChanged();
            }
        }

        private string name;
        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }

        private double points;
        public double Points
        {
            get => points;
            set
            {
                points = value;
                OnPropertyChanged();
            }
        }

        private IRelayCommand playerProfileCommand;
        public IRelayCommand PlayerProfileCommand
        {
            get => playerProfileCommand;
            set
            {
                playerProfileCommand = value;
                OnPropertyChanged();
            }
        }
    }
}
