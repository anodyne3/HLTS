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
        [SerializeField] private SVGImage chestClosedIcon;
        [SerializeField] private SVGImage chestOpenIcon;
        [SerializeField] private Button openChestButton;
        [SerializeField] private TMP_Text openChestButtonText;
        [SerializeField] private TMP_Text bcMaxText;
        [SerializeField] private TMP_Text bpMaxText;
        [SerializeField] private TMP_Text sfMaxText;

        private ChestVariable _chestVariable;
        
        public override void Start()
        {
            base.Start();

            openChestButton.onClick.RemoveAllListeners();
            openChestButton.onClick.AddListener(ButtonAction);

            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.payoutFinishEvent, RefreshPanel);
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
        }
        
        private void RefreshPanel()
        {
            OpenChestIcon(false);
            RefreshOpenButton(PlayerData.GetChestCount(_chestVariable.chestType) > 0);
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

        private void ButtonAction()
        {
            if (PlayerData.GetChestCount(_chestVariable.chestType) < 1) return;
            
            FirebaseFunctionality.OpenChest(_chestVariable.chestType);
        }
    }
}