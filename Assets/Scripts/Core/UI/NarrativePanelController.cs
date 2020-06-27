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
        [SerializeField] private Image backgroundBlackoutImage;
        [SerializeField] private Button consumeNarrativeButton;
        [SerializeField] private SVGImage narrativeImage;
        [SerializeField] private TMP_Text headerText;
        [SerializeField] private TMP_Text narrativeText;
        [SerializeField] private TMP_Text footer;

        private NarrativePoint _narrativePoint;
        private NarrativeShard _activeShard;

        private int _currentChunk;
        private int _currentShard;
        private int _currentStage;
        private int _narrativeStages;

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

            backgroundBlackoutImage.gameObject.SetActive(args.Length > 1);
            _narrativePoint = NarrativeManager.LoadNarrativePoint((NarrativeTypes) args[0]);

            SetupNarrativeStage();
            ResetCounters();
            RefreshPanel();
        }

        private void SetupNarrativeStage()
        {
            var stageCount = 0;

            var narrativeShardLength = _narrativePoint.narrativeShard.Length;
            for (var i = 0; i < narrativeShardLength; i++)
                stageCount += _narrativePoint.narrativeShard[i].textChunks.Length;

            _narrativeStages = stageCount;
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
            headerText.text = _narrativePoint.title;
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
        
        private static void RemoveExistingNarrativeObjects()
        {
            Destroy(HudManager.helpButton.gameObject);
            Destroy(PanelManager.GetPanel<NarrativePanelController>().gameObject);
            GlobalComponents.Instance.RemoveGlobalComponent<NarrativeManager>();
        }

        protected override void ClosePanel()
        {
            base.ClosePanel();

            NarrativeManager.currentNarrativeSeen = true;

            switch (_narrativePoint.id)
            {
                case NarrativeTypes.Intro:
                    FirebaseFunctionality.UpdateNarrativeProgress(_narrativePoint.id);
                    CurrencyManager.HideCurrencies(false);
                    break;
                case NarrativeTypes.UpgradeSlider:
                case NarrativeTypes.Starfruits:
                    FirebaseFunctionality.UpdateNarrativeProgress(_narrativePoint.id);
                    break;
                case NarrativeTypes.Finale:
                    FirebaseFunctionality.UpdateNarrativeProgress(_narrativePoint.id);
                    RemoveExistingNarrativeObjects();
                    break;
                default:
                    NarrativeManager.currentNarrativePointType = _narrativePoint.id;
                    break;
            }

            NarrativeManager.RefreshHelpButton();
        }
    }
}