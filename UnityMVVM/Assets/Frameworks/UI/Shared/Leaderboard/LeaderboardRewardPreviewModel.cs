using MVVMToolkit.DataBinding;
using UnityEngine;

namespace MVVMToolkit.UI
{
    public class LeaderboardRewardPreviewModel : Model
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

        private string bracket;
        public string Bracket
        {
            get => bracket;
            set
            {
                bracket = value;
                OnPropertyChanged();
            }
        }

        private ObservableList<RewardItemModel> rewards;
        public ObservableList<RewardItemModel> Rewards
        {
            get => rewards;
            set
            {
                rewards = value;
                OnPropertyChanged();
            }
        }

        public LeaderboardRewardPreviewModel()
        {
            Rewards = new ObservableList<RewardItemModel>();
        }
    }
}
