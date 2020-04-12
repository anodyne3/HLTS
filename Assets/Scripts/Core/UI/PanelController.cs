﻿using DG.Tweening;
using Enums;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public class PanelController : GlobalAccess
    {
        [SerializeField] private Button closeButton;
        [SerializeField] private Button backgroundButton;
        [SerializeField] private TMP_TextAnimation[] textAnimations;
        
        public RectTransform panelTransform;
        
        private Image _backgroundImage;
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
            }

            if (backgroundButton == null) return;

            backgroundButton.onClick.RemoveAllListeners();
            backgroundButton.onClick.AddListener(ClosePanel);
        }

        public virtual void OpenPanel()
        {
            if (_backgroundImage == null && backgroundButton != null)
                _backgroundImage = (Image) backgroundButton.GetComponent(typeof(Image));
            
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
                })
                .Insert(0.0f, panelTransform.DOLocalMoveY(panelOffset, PanelManager.openPanelTweenSettings.moveDuration))
                .InsertCallback(0.0f, () => PanelManager.openPanelTweenSettings.DoPunch(panelTransform, false))
                .Insert(PanelManager.openPanelTweenSettings.fadeStartDelay, _backgroundImage.DOFade(0.66f, 1.0f))
                .InsertCallback(PanelManager.openPanelTweenSettings.fadeStartDelay, () => PanelManager.openPanelTweenSettings.DoFade(backgroundButton.image, false))
                .SetEase(PanelManager.openPanelTweenSettings.sequenceEasing)
                .SetAutoKill(false)
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
            AudioManager.PlayClip(SoundEffectType.ClosePanel);

            var closePanelSequence = DOTween.Sequence();
            closePanelSequence.Insert(0.0f, panelTransform.DOLocalMoveY(_defaultLocalPositionY, PanelManager.closePanelTweenSettings.moveDuration))
                .Insert(0.0f, panelTransform.DOScale(PanelManager.closePanelTweenSettings.scaleEndValue,
                    PanelManager.closePanelTweenSettings.scaleDuration))
                .InsertCallback(PanelManager.closePanelTweenSettings.fadeStartDelay, 
                    () => PanelManager.closePanelTweenSettings.DoFade(backgroundButton.image, false))
                .SetEase(PanelManager.closePanelTweenSettings.sequenceEasing)
                .SetAutoKill(false)
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