using Enums;
using MyScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace Core.UI
{
    public class ChestDetailsPanelController : PanelController
    {
        [SerializeField] private TMP_Text chestName;
        [SerializeField] private TMP_Text chestAmount;
        [SerializeField] private SVGImage chestClosedIcon;
        [SerializeField] private SVGImage chestOpenIcon;
        [SerializeField] private Button openChestButton;
        [SerializeField] private Button chestMergeButton;
        [SerializeField] private TMP_Text openChestButtonText;
        [SerializeField] private TMP_Text bcMaxText;
        [SerializeField] private TMP_Text bpMaxText;
        [SerializeField] private TMP_Text sfMaxText;

        private ChestVariable _chestVariable;
        
        public override void Start()
        {
            base.Start();

            openChestButton.onClick.RemoveAllListeners();
            openChestButton.onClick.AddListener(OpenChest);
            chestMergeButton.onClick.RemoveAllListeners();
            chestMergeButton.onClick.AddListener(OpenMergePanel);

            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.refreshUiEvent, RefreshPanel);
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.chestOpenEvent, CloseChestIcon);
        }

        public override void OpenPanel(params object[] args)
        {
            base.OpenPanel();

            var chestType = (ChestType) args[0];
            
            var chestTypesLength = ChestManager.chestTypes.Length;
            for (var i = 0; i < chestTypesLength; i++)
            {
                if (ChestManager.chestTypes[i].chestType != chestType) continue;

                _chestVariable = ChestManager.chestTypes[i];
            }
            
            InitPanel();
        }

        private void InitPanel()
        {
            if (_chestVariable == null) return;

            chestName.text = _chestVariable.chestName;
            chestClosedIcon.sprite = _chestVariable.chestIcon;
            chestOpenIcon.sprite = _chestVariable.chestOpenIcon;
            bcMaxText.text = Constants.BananaCoinIcon + Constants.ChestRewardPrefix + _chestVariable.bcMax;
            bpMaxText.text = Constants.BluePrintIcon + Constants.ChestRewardPrefix + _chestVariable.bpMax;
            sfMaxText.text = Constants.StarFruitIcon + Constants.ChestRewardPrefix + _chestVariable.sfMax;
            
            RefreshPanel();
            CloseChestIcon();
        }
        
        private void RefreshPanel()
        {
            var chestCount = PlayerData.GetChestCount(_chestVariable.chestType);
            RefreshOpenButton(chestCount > 0);
            chestAmount.text = chestCount.ToString();
        }

        private void CloseChestIcon()
        {
            OpenChestIcon(false);
        }
        
        public void OpenChestIcon(bool value)
        {
            chestOpenIcon.enabled = value;
            chestClosedIcon.enabled = !value;
        }

        private void RefreshOpenButton(bool value)
        {
            openChestButton.interactable = value;
            openChestButtonText.color = value ? Color.white : Color.grey;
        }

        private void OpenChest()
        {
            if (PlayerData.GetChestCount(_chestVariable.chestType) < 1) return;
            
            FirebaseFunctionality.OpenChest(_chestVariable.chestType);
        }

        private static void OpenMergePanel()
        {
            PanelManager.OpenSubPanel<ChestMergePanelController>();
        }
    }
}