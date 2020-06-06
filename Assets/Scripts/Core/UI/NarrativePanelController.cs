using System;
using Core.Managers;
using Enums;
using MyScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Core.UI
{
    public class NarrativePanelController : PanelController
    {
        [SerializeField] private Button consumeNarrativeButton;
        [SerializeField] private SVGImage narrativeImage;
        [SerializeField] private TMP_Text narrativeText;
        [SerializeField] private TMP_Text footer;

        private NarrativePoint _narrativePoint;
        private NarrativeShard _activeShard;

        [SerializeField] private int _currentChunk;
        [SerializeField] private int _currentShard;
        [SerializeField] private int _currentStage;
        [SerializeField] private int _narrativeStages;

        public override void Start()
        {
            base.Start();

            backgroundButton.onClick.RemoveAllListeners();

            consumeNarrativeButton.onClick.RemoveAllListeners();
            consumeNarrativeButton.onClick.AddListener(ConsumeNarrative);
        }

        public override void OpenPanel(params object[] args)
        {
            base.OpenPanel();

            _narrativePoint = NarrativeManager.currentNarrativePoint;

            _narrativeStages = CalculateNarrativeStageCount();

            ResetCounters();

            RefreshPanel();
        }

        private int CalculateNarrativeStageCount()
        {
            var stageCount = 0;

            var narrativeShardLength = _narrativePoint.narrativeShard.Length;
            for (var i = 0; i < narrativeShardLength; i++)
            {
                stageCount += _narrativePoint.narrativeShard[i].textChunks.Length;
            }

            return stageCount;
        }

        private void ResetCounters()
        {
            _currentChunk = 0;
            _currentShard = 0;
            _currentStage = 0;
            _activeShard = _narrativePoint.narrativeShard[0];
        }

        private void RefreshPanel()
        {
            narrativeImage.sprite = _activeShard.storyImage;
            narrativeText.text = _activeShard.textChunks[0];

            RefreshFooter();
        }

        private void RefreshFooter()
        {
            footer.text = _currentStage + 1 >= _narrativeStages
                ? Constants.NarrativeClose
                : Constants.NarrativeContinue;
        }

        private void ConsumeNarrative()
        {
            _currentStage++;
            if (_currentStage == _narrativeStages)
                ClosePanel();

            if (ProgressText())
                ProgressImage();

            RefreshFooter();
        }

        private void ProgressImage()
        {
            narrativeImage.sprite = _activeShard.storyImage;
        }

        private bool ProgressText()
        {
            _currentChunk++;

            var endOfChunk = _currentChunk >= _activeShard.textChunks.Length;

            if (endOfChunk)
            {
                _currentShard++;

                if (_currentShard >= _narrativePoint.narrativeShard.Length)
                    return false;

                _activeShard = _narrativePoint.narrativeShard[_currentShard];

                _currentChunk = 0;
            }

            narrativeText.text = _activeShard.textChunks[_currentChunk];

            return endOfChunk;
        }

        protected override void ClosePanel()
        {
            base.ClosePanel();

            switch (_narrativePoint.id)
            {
                case NarrativeTypes.Intro:
                case NarrativeTypes.SlotMachine:
                case NarrativeTypes.UpgradeSlider:
                case NarrativeTypes.Blueprints:
                case NarrativeTypes.ChestMerge:
                case NarrativeTypes.ChestClaim:
                    FirebaseFunctionality.ProgressNarrativePoint();
                    NarrativeManager.ShowHelpButton(false);
                    break;
                // case NarrativeTypes.LoadCoin:
                case NarrativeTypes.PullLever:
                case NarrativeTypes.ChestRoll:
                case NarrativeTypes.ChestOpen:
                case NarrativeTypes.CoinSlotUpgrade:
                    NarrativeManager.ShowHelpButton();
                    break;
                default:
                    NarrativeManager.ShowHelpButton(false);
                    break;
            }
        }
    }
}