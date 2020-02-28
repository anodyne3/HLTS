using Utils;

namespace Core.MainSlotMachine
{
    public class CoinTray : Singleton<CoinTray>
    {
        public int CoinTrayCounter { get; private set; }

        private void Start()
        {
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.coinCreatedEvent, AddCoinToTray);
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.coinLoadEvent, DeductCoinFromTray);
            EventManager.NewEventSubscription(gameObject, Constants.GameEvents.coinDroppedEvent, DeductCoinFromTray);
        }

        private void DeductCoinFromTray()
        {
            CoinTrayCounter -= 1;
            EventManager.generateCoin.Raise();
        }

        private void AddCoinToTray()
        {
            CoinTrayCounter += 1;
        }
    }
}