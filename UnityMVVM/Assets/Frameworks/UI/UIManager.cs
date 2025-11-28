using System;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using Geewa.Api.Settings;
using Geewa.Framework;
using Geewa.Framework.UserAction;
using MVVMToolkit.UI;
using JoshH.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MVVMToolkit.UI
{
    public enum ScreenType
    {
        None = 0,
        Home = 1,
        Core = 2,
        PostmatchWin = 3,
        PostmatchLose = 4,
        Collection = 5,
        UnitDetail = 6,
        SoldierDetail = 7,
        HeroDetail = 8,
        PortalResult = 9,
        Portal = 10,
        Event = 11,
        EventFloor = 12,
        EventLeaderboard = 13,
        Talents = 14,
        AssetsDownload = 15,
        Shop = 16,
        Offers = 17,
        BattleSim = 18,
        BattleSimArmy = 19,
        BattleSimGlobalPerks = 20,
        BattleSimUnits = 21,
        BattleSimResult = 22,
        NewUnit = 23,
        TalentsRankUp = 24,
        TalentsReward = 25,
        GearMerge = 26,
        FeatureShop = 27,
        SeasonPass = 28,
        Gear = 29,
        HeroCollection = 30,
        Events = 31,
        PriviledgeCards = 32,
        PostmatchBigBoss = 33,
        Pvp = 34,
        PostmatchPvpWin = 35,
        PostmatchPvpLose = 36,
        DeckBuilding = 37,
        RuneMerge = 38
    }

    public enum WindowType
    {
        None = 0,
        Cheats = 1,
        Perks = 2,
        PerkDetail = 3,
        Quests = 4,
        DailyLogin = 5,
        Settings = 6,
        SettingsGame = 7,
        RankingRewards = 8,
        Revive = 9,
        GearDetail = 10,
        GearChestResult = 11,
        FeatureUnlock = 12,
        OfferPreview = 13,
        AfkReward = 14,
        Challenge = 15,
        TalentRanks = 16,
        StatsSummary = 17,
        RanksSummary = 18,
        AbilityDetail = 19,
        NewbieOffer = 20,
        ChestContent = 21,
        Rankings = 22,
        CurrencyNavigation = 23,
        LeaderboardResult = 24,
        ChangeName = 25,
        RateApp = 26,
        SummonRates = 27,
        RuneDetail = 28,
        MergeResult = 29,
        ChainOffer = 30,
        PlayerProfile = 31,
        DivisionChange = 32,
        MatchMetrics = 33,
        UnlockShowcase = 34,
        LeagueInfo = 35
    }

    [RequireComponent(typeof(AudioSource))]
    public class UIManager : Initializable
    {
        // debt: we have the ui manager reference, remove this -js
        public static UIManager Instance { get; private set; }

        public event Action<PointerEventData> Clicked;
        public event Action<WindowType> WindowClosed;
        public event Action<ScreenType> ScreenOpened;
        public event Action<WindowType> WindowOpened;
        public event Action<string> ComponentOpened;
        public event Action Initialized;

        [SerializeField] private UIManagerReference reference;
        [SerializeField] private UiComponentBase topbarComponent;
        [SerializeField] private UiComponentBase navbarComponent;
        [SerializeField] private float topbarOffset = -90f;
        [SerializeField] private float navbarOffset = 150f;
        [SerializeField] private UserActionHandler userActionHandler;
        [SerializeField] private Animator upgradeAnimator;
        [SerializeField] private UIGradient upgradeAnimationGradient;
        [SerializeField] private GradientKeyMap upgradeAnimationGradientKeyMap;
        [SerializeField] private OnboardingManager onboardingManager;
        [SerializeField] private SuggestionManager suggestionManager;
        [SerializeField] private SettingsApi settingsApi;
        [SerializeField] private GameObject blockingOverlay;
        [SerializeField] private LoadingManager loadingManager;
        [SerializeField] private GameObject cloudsTransition;
        [SerializeField] private Animator cloudsTransitionAnimator;
        [SerializeField] private AnimationClip cloudsCloseClip;
        [SerializeField] private float cloudsCloseSpeed = 0.5f;
        [SerializeField] private CoroutineRunner coroutineRunner;

        [Header("Instantiation containers")]
        [SerializeField] private RectTransform screensContainer;
        [SerializeField] private RectTransform windowsContainer;

        [Space]
        [SerializeField] private UiScreenBase[] staticScreens;
        [SerializeField] private UiScreenBase[] screenPrefabs;
        [SerializeField] private UiWindowBase[] windowPrefabs;

        public LoadingManager LoadingManager => loadingManager;
        public OnboardingManager OnboardingManager => onboardingManager;
        public SuggestionManager SuggestionManager => suggestionManager;
        public MusicController MusicController => musicController;
        public UiScreenBase Screen { get; private set; }
        public UiWindowBase Window { get; private set; }
        public UiWindowBase SecondaryWindow { get; private set; }
        public UserActionHandler UserActionHandler => userActionHandler;
        public CoroutineRunner CoroutineRunner => coroutineRunner;

        private RectTransform topbarRectTra;
        private RectTransform navbarRectTra;
        private MusicController musicController;
        private AudioSource audioSource;

        private void Awake()
        {
            Instance = this;
            reference.Value = this;
            topbarRectTra = topbarComponent.GetComponent<RectTransform>();
            navbarRectTra = navbarComponent.GetComponent<RectTransform>();
            musicController = FindFirstObjectByType<MusicController>();
            audioSource = GetComponent<AudioSource>();
            onboardingManager.Initialize(this);
            suggestionManager.Initialize(this);
            SetBlockingOverlayActive(false);
        }

        private void OnApplicationFocus(bool focus)
        {
            if (focus)
                SetBlockingOverlayActive(false);
        }

        public override void Initialize()
        {
            Initialized?.Invoke();
        }

        public void OpenScreen(ScreenType type, object data = null)
        {
            if (type == ScreenType.None)
                return;

            CloseAllWindows(null, true);

            if (Screen != null)
            {
                Screen.Close();

                if (!staticScreens.Any(x => x.Type == Screen.Type))
                {
                    Destroy(Screen.gameObject);
                    Screen = null;
                }
            }

            if (staticScreens.Any(x => x.Type == type))
            {
                Screen = staticScreens.First(x => x.Type == type);
            }
            else
            {
                try
                {
                    UiScreenBase screenPrefab = null;

                    if (type is ScreenType.Event)
                    {
                        if (data != null)
                        {
                            string eventType = data.ToString();
                            switch (eventType)
                            {
                                case "Gold":
                                case "Portal":
                                case "Hero":
                                    screenPrefab = screenPrefabs.First(x => x.Type == ScreenType.EventFloor);
                                    break;
                                default:
                                    screenPrefab = screenPrefabs.First(x => x.Type == ScreenType.EventLeaderboard);
                                    break;
                            }
                        }
                        else
                        {
                            Trace.Error($"Cannot open Event withou eventType, data={data}");
                        }
                    }
                    else
                    {
                        screenPrefab = screenPrefabs.First(x => x.Type == type);
                    }

                    UiScreenBase createdScreen = Instantiate(screenPrefab, screensContainer);
                    createdScreen.gameObject.SetActive(false);
                    Screen = createdScreen;
                }
                catch (InvalidOperationException ioe)
                {
                    Trace.Error($"Unable to find screen of type: {type}");
                    return;
                }
                catch (Exception e)
                {
                    Trace.Error(e.Message);
                    return;
                }
            }

            if (Screen.ShowTopbar)
            {
                Screen.RectTra.offsetMax = new Vector2(Screen.RectTra.offsetMax.x, topbarOffset);
                topbarComponent.Open();
            }
            else
            {
                Screen.RectTra.offsetMax = new Vector2(Screen.RectTra.offsetMax.x, 0);
                topbarComponent.Close();
            }

            if (Screen.ShowNavbar)
            {
                Screen.RectTra.offsetMin = new Vector2(Screen.RectTra.offsetMin.x, navbarOffset);
                navbarComponent.Open(type);
            }
            else
            {
                Screen.RectTra.offsetMin = new Vector2(Screen.RectTra.offsetMin.x, 0);
                navbarComponent.Close();
            }

            // Safety check, make sure when transitioning to new screen, blocking overlay never remains enabled
            SetBlockingOverlayActive(false);

            Screen.Open(data);
            ScreenOpened?.Invoke(type);
        }

        public void OpenWindow(WindowOpenData data)
        {
            OpenWindow(data.Type, data.Data, data.UseAnimation, data.OpenAsPrimary);
        }

        public void OpenWindow(WindowType type, object data = null, bool useAnimation = false, bool openAsPrimary = false)
        {
            if (!openAsPrimary)
                CloseSecondaryWindow();
            else
                CloseAllWindows();

            bool showSecondaryWindow = false;

            // Here we are closing secondary window, but the window we want to return to is already there as primary
            // so we want to avoid Instantiating a new window
            if (Window != null && !openAsPrimary && Window.Type == type)
            {
                Window.Open(data, false);
                WindowOpened?.Invoke(type);
                return;
            }
            else
            {
                if (windowPrefabs.Any(x => x.Type == type))
                {
                    UiWindowBase windowPrefab = windowPrefabs.First(x => x.Type == type);
                    UiWindowBase createdWindow = Instantiate(windowPrefab, windowsContainer);
                    createdWindow.gameObject.SetActive(false);

                    if (Window == null)
                    {
                        Window = createdWindow;
                    }
                    else
                    {
                        showSecondaryWindow = true;
                        SecondaryWindow = createdWindow;

                        // Bump secondary windows above primary windows
                        createdWindow.Canvas.sortingOrder += 9;
                    }
                }
                else
                {
                    Trace.Error($"Unable to find window of type: {type}");
                    return;
                }
            }

            if (Window != null && !showSecondaryWindow)
                Window.Open(data, useAnimation);
            else if (SecondaryWindow != null)
                SecondaryWindow.Open(data, useAnimation);

            WindowOpened?.Invoke(type);
        }

        public void CloseWindow(object data = null, bool screenRedirect = false, bool useAnimation = false)
        {
            // Close Secondary or Main window, but never both at once (it only closes the top-most window)
            if (CloseSecondaryWindow(data, useAnimation))
                return;

            if (Window != null)
            {
                Window.Close(useAnimation);
                WindowType type = Window.Type;

                // Close window
                Destroy(Window.gameObject);
                Window = null;

                // Notify others about close
                WindowClosed?.Invoke(type);

                // Notify screen about window close
                Screen?.OnWindowClosed(type, data, screenRedirect);
            }
        }

        public bool CloseMainWindow(object data = null, bool screenRedirect = false, bool useAnimation = false)
        {
            if (Window != null)
            {
                Window.Close(useAnimation);
                WindowType type = Window.Type;

                // Close window
                Destroy(Window.gameObject);
                Window = null;

                // Notify others about close
                WindowClosed?.Invoke(type);

                // Notify screen about window close
                Screen?.OnWindowClosed(type, data, screenRedirect);

                return true;
            }

            return false;
        }

        public bool CloseSecondaryWindow(object data = null, bool useAnimation = false)
        {
            if (SecondaryWindow != null)
            {
                SecondaryWindow.Close(useAnimation);
                WindowType type = SecondaryWindow.Type;

                // Close window
                Destroy(SecondaryWindow.gameObject);
                SecondaryWindow = null;

                // Notify others about close
                WindowClosed?.Invoke(type);

                // Notify primary window about window close
                Window?.OnWindowClosed(type, data);

                return true;
            }

            return false;
        }

        public void CloseAllWindows(object data = null, bool screenRedirect = false, bool useAnimation = false)
        {
            CloseSecondaryWindow(data, useAnimation);
            CloseWindow(data, screenRedirect, useAnimation);
        }

        public void ResetTopAndNavbarPosition()
        {
            topbarRectTra.DOKill();
            navbarRectTra.DOKill();
            topbarRectTra.anchoredPosition = new Vector2(topbarRectTra.anchoredPosition.x, 0f);
            navbarRectTra.anchoredPosition = new Vector2(navbarRectTra.anchoredPosition.x, 0f);
        }

        public void RollUpTopAndNavbar()
        {
            topbarRectTra.DOAnchorPosY(220f, 0.5f);
            navbarRectTra.DOAnchorPosY(-340, 0.5f);
        }

        public void SetBlockingOverlayActive(bool active)
        {
            blockingOverlay.SetActive(active);
        }

        public void PlaySfx(AudioClip clip)
        {
            if (settingsApi.IsSfxOn)
                audioSource.PlayOneShot(clip, musicController.GetSfxVolume());
        }

        public void PlayUpgradeAnimation(string upgradeType)
        {
            upgradeAnimationGradient.LinearGradient = upgradeAnimationGradientKeyMap.Get(upgradeType);
            upgradeAnimator.SetTrigger("Show");
        }

        public void OpenCheats()
        {
            // Temp method for opening cheats
            OpenWindow(WindowType.Cheats);
        }

        public async Task ShowCloudsTransition()
        {
            cloudsTransition.SetActive(true);
            cloudsTransitionAnimator.SetTrigger("Close");

            await Task.Delay((int)(1000f * (cloudsCloseClip.length / cloudsCloseSpeed)));
        }

        public void HideCloudsTransition()
        {
            // Hide of the canvas is called from animation after it finishes using event
            if (cloudsTransitionAnimator.isActiveAndEnabled)
                cloudsTransitionAnimator.SetTrigger("Open");
        }

        public void OnClick(PointerEventData eventData)
        {
            Clicked?.Invoke(eventData);
        }

        public void InvokeComponentOpened(string name)
        {
            ComponentOpened?.Invoke(name);
        }
    }
}
