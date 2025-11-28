using System;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
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
    }

    public enum WindowType
    {
        None = 0,
        Cheats = 1,
    }

    [RequireComponent(typeof(AudioSource))]
    public class UIManager : Initializable
    {
        public static UIManager Instance { get; private set; }

        public event Action<PointerEventData> Clicked;
        public event Action<WindowType> WindowClosed;
        public event Action<ScreenType> ScreenOpened;
        public event Action<WindowType> WindowOpened;
        public event Action<string> ComponentOpened;
        public event Action Initialized;

        [SerializeField] private UiComponentBase topbarComponent;
        [SerializeField] private UiComponentBase navbarComponent;
        [SerializeField] private float topbarOffset = -90f;
        [SerializeField] private float navbarOffset = 150f;
        [SerializeField] private CoroutineRunner coroutineRunner;

        [Header("Instantiation containers")]
        [SerializeField] private RectTransform screensContainer;
        [SerializeField] private RectTransform windowsContainer;

        [Space]
        [SerializeField] private UiScreenBase[] staticScreens;
        [SerializeField] private UiScreenBase[] screenPrefabs;
        [SerializeField] private UiWindowBase[] windowPrefabs;

        public UiScreenBase Screen { get; private set; }
        public UiWindowBase Window { get; private set; }
        public UiWindowBase SecondaryWindow { get; private set; }
        public CoroutineRunner CoroutineRunner => coroutineRunner;

        private AudioSource audioSource;

        private void Awake()
        {
            Instance = this;
            audioSource = GetComponent<AudioSource>();
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
                    UiScreenBase screenPrefab = screenPrefabs.First(x => x.Type == type);

                    UiScreenBase createdScreen = Instantiate(screenPrefab, screensContainer);
                    createdScreen.gameObject.SetActive(false);

                    Screen = createdScreen;
                }
                catch (InvalidOperationException ioe)
                {
                    Debug.LogError($"Unable to find screen of type: {type}");
                    return;
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
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
                    Debug.LogError($"Unable to find window of type: {type}");
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

        public void PlaySfx(AudioClip clip)
        {
            audioSource.PlayOneShot(clip, 1f);
        }

        public void OpenCheats()
        {
            // Temp method for opening cheats
            OpenWindow(WindowType.Cheats);
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
