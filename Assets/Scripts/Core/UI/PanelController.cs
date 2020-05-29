using DG.Tweening;
using Enums;
using MyScriptableObjects;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Core.UI
{
    public class PanelController : GlobalAccess
    {
        [SerializeField] private Button closeButton;
        [SerializeField] protected Button backgroundButton;
        [SerializeField] private TMP_TextAnimation[] textAnimations;
        [HideInInspector] public TweenPunchSetting closeButtonPunchSetting;
        [SerializeField] private float defaultLocalPositionY = 560.0f;

        public RectTransform panelTransform;


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
                closeButtonPunchSetting = (TweenPunchSetting) Resources.Load(Constants.CloseButtonPunchSettingPath);
            }

            if (backgroundButton == null) return;

            backgroundButton.onClick.RemoveAllListeners();
            backgroundButton.onClick.AddListener(ClosePanel);
        }

        public virtual void OpenPanel(params object[] args)
        {
            AudioManager.PlayClip(SoundEffectType.OpenPanel);

            panelTransform.localScale = PanelManager.closePanelTweenSettings.scaleStartValue;
            var panelOffset = PanelManager.OpenPanelCount() * PanelManager.openPanelTweenSettings.moveOffset +
                              defaultLocalPositionY;
            var openPanelSequence = DOTween.Sequence();
            openPanelSequence
                .InsertCallback(0.0f, () =>
                {
                    if (GameManager != null)
                        GameManager.interactionEnabled = false;
                    
                    if (PanelManager != null)
                        PanelManager.dragPinchDisabled = true;

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
                closeButtonPunchSetting.DoPunch(closeButton.transform, false);

            AudioManager.PlayClip(SoundEffectType.ClosePanel);

            var closePanelSequence = DOTween.Sequence();
            closePanelSequence.Insert(0.0f,
                    panelTransform.DOLocalMoveY(defaultLocalPositionY,
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
                    
                    if (PanelManager != null)
                        PanelManager.dragPinchDisabled = false;

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