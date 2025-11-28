using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace MVVMToolkit.UI
{
    [RequireComponent(typeof(Canvas))]
    public abstract class UiWindowBase : UiBase, IUiWindow
    {
        [field: SerializeField] public RectTransform ContainerTra { get; set; }
        [field: SerializeField] public CanvasGroup ContainerCanvasGroup { get; set; }
        [field: SerializeField] public CanvasGroup OverlayCanvasGroup { get; set; }
        [field: SerializeField] public RectTransform LeftChainTra { get; set; }
        [field: SerializeField] public RectTransform RightChainTra { get; set; }

        public abstract WindowType Type { get; }

        // TODO: stop using unity events for these, nobody from components should hook to these
        // handle adding/removing listeners
        public UnityEvent WindowOpen;
        public UnityEvent WindowClose;
        public UnityEvent OpenAnimationComplete;

        protected object OpenData;

        private Canvas canvas = null;
        public Canvas Canvas => canvas ??= GetComponent<Canvas>();

        private Sequence closeAnimation, openAnimation;

        public virtual void Close(bool useAnimation)
        {
            // TODO: anim length 0.5s (15 frames)
            // Container rotation Z 0 - at 5 = 3, at 8 = 0
            // Container anchored pos Y 0, at 5 = -50, at 15 = 3070
            // Overlay (add canvas group) alpha - 0.98, at 10 = 0.98, at 15 = 0
            // Left/Right chain rotation Z 0 - at 5 = -3, at 8 = 0
            // Left/Right chain scale Y 2 - at 5 = Y 2.5, at 8 = 2

            // Note: right now there is no await for window Close so any animation would be cut,
            // once anim. is implemented this needs to be awaited and deactivation and destroy called after!

            // This will change once we will have close anim!
            StopAnyOngoingAnim();

            gameObject.SetActive(false);

            WindowClose?.Invoke();
        }

        public virtual void Open(object data, bool useAnimation)
        {
            OpenData = data;

            ContainerTra.anchoredPosition = new Vector2(ContainerTra.anchoredPosition.x, useAnimation ? 3070f : 0f);
            ContainerTra.localRotation = Quaternion.identity;

            if (OverlayCanvasGroup != null)
                OverlayCanvasGroup.alpha = useAnimation ? 0f : 1f;

            if (LeftChainTra != null)
            {
                LeftChainTra.localRotation = Quaternion.identity;
                LeftChainTra.localScale = new Vector3(2, 2, 1);
            }

            if (RightChainTra != null)
            {
                RightChainTra.localRotation = Quaternion.identity;
                RightChainTra.localScale = new Vector3(2, 2, 1);
            }

            gameObject.SetActive(true);

            // anim length 1s (30 frames)
            // Container rotation Z 0 - at 10 = 0, at 15 = 3, at 23 = -2, at 30 = 0
            // Container anchored pos Y 3070, at 15 = -50, at 23 = 50, at 30 = 0
            // Overlay (add canvas group) alpha - 0, at 10 = 0.98
            // Left/Right chain rotation Z 0 - at 10 = 0, at 15 = -3, at 23 = 2, at 30 = 0
            // Left/Right chain scale Y 2 - at 10 = Y 2.25, at 23 = Y 1.75, at 30 = 2

            // Original animation if we need it for now still
            /*  animation.Append(ContainerTra.DOScale(1.1f, 0.2f).SetEase(Ease.OutSine));
                animation.Append(ContainerTra.DOScale(1f, 0.1f).SetEase(Ease.OutSine));
                animation.Insert(0f, ContainerCanvasGroup.DOFade(1f, 0.3f).SetEase(Ease.OutSine));
             */

            if (useAnimation)
            {
                StopAnyOngoingAnim();

                openAnimation = DOTween.Sequence();

                // Y Movement
                openAnimation.Append(ContainerTra.DOAnchorPosY(-50f, 0.5f));
                openAnimation.Insert(0.5f, ContainerTra.DOAnchorPosY(50f, 0.25f));
                openAnimation.Insert(0.75f, ContainerTra.DOAnchorPosY(0f, 0.25f));

                // Rotation
                openAnimation.Insert(0.33f, ContainerTra.DOLocalRotate(new Vector3(0, 0, 3), 0.17f));
                openAnimation.Insert(0.5f, ContainerTra.DOLocalRotate(new Vector3(0, 0, -2), 0.25f));
                openAnimation.Insert(0.75f, ContainerTra.DOLocalRotate(Vector3.zero, 0.25f));

                // Overlay
                openAnimation.Insert(0f, OverlayCanvasGroup.DOFade(1f, 0.33f));

                // Left chain
                openAnimation.Insert(0.33f, LeftChainTra.DOLocalRotate(new Vector3(0, 0, -3), 0.17f));
                openAnimation.Insert(0.5f, LeftChainTra.DOLocalRotate(new Vector3(0, 0, 2), 0.25f));
                openAnimation.Insert(0.75f, LeftChainTra.DOLocalRotate(Vector3.zero, 0.25f));
                openAnimation.Insert(0f, LeftChainTra.DOScaleZ(2.25f, 0.33f));
                openAnimation.Insert(0.33f, LeftChainTra.DOScaleZ(1.75f, 0.42f));
                openAnimation.Insert(0.75f, LeftChainTra.DOScaleZ(2f, 0.25f));

                // Right chain
                openAnimation.Insert(0.33f, RightChainTra.DOLocalRotate(new Vector3(0, 0, -3), 0.17f));
                openAnimation.Insert(0.5f, RightChainTra.DOLocalRotate(new Vector3(0, 0, 2), 0.25f));
                openAnimation.Insert(0.75f, RightChainTra.DOLocalRotate(Vector3.zero, 0.25f));
                openAnimation.Insert(0f, RightChainTra.DOScaleZ(2.25f, 0.33f));
                openAnimation.Insert(0.33f, RightChainTra.DOScaleZ(1.75f, 0.42f));
                openAnimation.Insert(0.75f, RightChainTra.DOScaleZ(2f, 0.25f));

                openAnimation.SetUpdate(true);
                openAnimation.OnComplete(() =>
                {
                    OpenAnimationComplete?.Invoke();
                    openAnimation = null;
                });
                openAnimation.Play();
            }

            WindowOpen?.Invoke();
        }

        public virtual void OnWindowClosed(WindowType type, object data)
        {

        }

        private void StopAnyOngoingAnim()
        {
            if (closeAnimation != null)
            {
                closeAnimation.Kill();
                closeAnimation = null;
            }

            if (openAnimation != null)
            {
                openAnimation.Kill();
                openAnimation = null;
            }
        }
    }
}
