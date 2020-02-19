﻿using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI
{
    public class MenuPanelController : PanelController
    {
        [SerializeField] private Button resetAccountButton;
        [SerializeField] private Button linkAccountButton;
        public TMP_Text userId;

        private void Start()
        {
            resetAccountButton.onClick.RemoveAllListeners();
            resetAccountButton.onClick.AddListener(ResetAccount);
            linkAccountButton.onClick.RemoveAllListeners();
            linkAccountButton.onClick.AddListener(LinkAccount);
        }

        public override void OpenPanel()
        {
            base.OpenPanel();

            userId.text = PlayerData.firebaseUser.UserId;
        }

        private void ResetAccount()
        {
            GameManager.ResetAccount();
        }

        private void LinkAccount()
        {
            GameManager.LinkAccount();
        }
    }
}