using System.Collections.Generic;
using Core.UI.Prefabs;
using UnityEngine;
using Utils;

namespace Core.UI
{
    public class ChestMergePanelController : PanelController
    {
        [SerializeField] private ChestMergePrefab chestMergePrefab;
        [SerializeField] private Transform prefabHolder;

        private readonly List<ChestMergePrefab> _mergeButtons = new List<ChestMergePrefab>();

        public override void Start()
        {
            base.Start();

            InitPrefabs();

            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.upgradeRefreshEvent, RefreshButtons);
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.chestRefreshEvent, RefreshButtons, true);
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.refreshUiEvent, RefreshIndicators,
                true);
        }

        private void InitPrefabs()
        {
            var chestMergeTypesLength = ChestManager.chestMergeTypes.Length;
            for (var i = 0; i < chestMergeTypesLength; i++)
            {
                var chestMergeButton = Instantiate(chestMergePrefab, prefabHolder);
                chestMergeButton.Init(ChestManager.chestMergeTypes[i]);
                _mergeButtons.Add(chestMergeButton);
            }
        }

        private void RefreshButtons()
        {
            foreach (var button in _mergeButtons)
                button.RefreshButton();
        }

        private void RefreshIndicators()
        {
            foreach (var button in _mergeButtons)
                button.RefreshIndicators();
        }
    }
}