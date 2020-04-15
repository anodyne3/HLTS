using DG.Tweening;
using Enums;
using MyScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public class PanelController : GlobalAccess
    {
        [SerializeField] private Button closeButton;
        [SerializeField] protected Button backgroundButton;
        [SerializeField] private TMP_TextAnimation[] textAnimations;
        private TweenPunchSetting _closeButtonPunchSetting;

        public RectTransform panelTransform;

        private float _defaultLocalPositionY;

        public virtual void Awake()
        {
            var textAnimationsLength = textAnimations.Length;
            for (var i = 0; i < textAnimationsLength; i++)
            {
                textAnimations[i].Init();
            }
        }

        public virtual void Start()
        {
            if (closeButton != null)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(ClosePanel);
                _closeButtonPunchSetting = (TweenPunchSetting) Resources.Load("TweenSettings/closeButtonPunchSetting");
            }

            if (backgroundButton == null) return;

            backgroundButton.onClick.RemoveAllListeners();
            backgroundButton.onClick.AddListener(ClosePanel);
        }

        public virtual void OpenPanel()
        {
            AudioManager.PlayClip(SoundEffectType.OpenPanel);

            panelTransform.localScale = PanelManager.closePanelTweenSettings.scaleStartValue;
            _defaultLocalPositionY = panelTransform.localPosition.y;
            var panelOffset = PanelManager.OpenPanelCount() * PanelManager.openPanelTweenSettings.moveOffset +
                              _defaultLocalPositionY;
            var openPanelSequence = DOTween.Sequence();
            openPanelSequence
                .InsertCallback(0.0f, () =>
                {
                    if (GameManager != null)
                        GameManager.interactionEnabled = false;

                    gameObject.SetActive(true);
                    Canvas.ForceUpdateCanvases();
                })
                .Insert(0.0f,
                    panelTransform.DOLocalMoveY(panelOffset, PanelManager.openPanelTweenSettings.moveDuration))
                .InsertCallback(0.0f, () => PanelManager.openPanelTweenSettings.DoPunch(panelTransform, false))
                .Insert(PanelManager.openPanelTweenSettings.fadeStartDelay,
                    backgroundButton.image.DOFade(PanelManager.openPanelFadeSettings.fadeEndValue,
                        PanelManager.openPanelFadeSettings.fadeDuration))
                .SetEase(PanelManager.openPanelTweenSettings.sequenceEasing)
                .SetRecyclable(true)
                .OnComplete(StartTextAnimations);
        }

        protected void StartTextAnimations()
        {
            if (!gameObject.activeInHierarchy) return;

            var textAnimationsLength = textAnimations.Length;
            for (var i = 0; i < textAnimationsLength; i++)
            {
                textAnimations[i].vertexAnimation.RefreshTargetText();
                StartCoroutine(textAnimations[i].vertexAnimation.AnimateTargetText());
            }
        }

        protected virtual void ClosePanel()
        {
            if (closeButton != null)
                _closeButtonPunchSetting.DoPunch(closeButton.transform, false);

            AudioManager.PlayClip(SoundEffectType.ClosePanel);

            var closePanelSequence = DOTween.Sequence();
            closePanelSequence.Insert(0.0f,
                    panelTransform.DOLocalMoveY(_defaultLocalPositionY,
                        PanelManager.closePanelTweenSettings.moveDuration))
                .Insert(0.0f, panelTransform.DOScale(PanelManager.closePanelTweenSettings.scaleEndValue,
                    PanelManager.closePanelTweenSettings.scaleDuration))
                .Insert(PanelManager.closePanelTweenSettings.fadeStartDelay,
                    backgroundButton.image.DOFade(PanelManager.closePanelFadeSettings.fadeEndValue,
                        PanelManager.closePanelFadeSettings.fadeDuration))
                .SetEase(PanelManager.closePanelTweenSettings.sequenceEasing)
                .SetRecyclable(true)
                .OnComplete(() =>
                {
                    gameObject.SetActive(false);

                    if (GameManager != null)
                        GameManager.interactionEnabled = true;

                    panelTransform.localScale = PanelManager.openPanelTweenSettings.scaleStartValue;
                });
        }

        public void OnDestroy()
        {
            if (PanelManager != null && PanelManager.allPanels.Contains(this))
                PanelManager.allPanels.Remove(this);
        }
    }
}