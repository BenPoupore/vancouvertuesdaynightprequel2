using UnityEngine;

namespace VipExtraction
{
    public sealed class SafehouseMenuController : MonoBehaviour
    {
        [SerializeField] private GameSession session;
        [SerializeField] private GameObject shopPanel;
        [SerializeField] private GameObject missionSelectPanel;
        [SerializeField] private GameObject inventoryPanel;
        [SerializeField] private GameObject nonsenseMarketPanel;

        private void Start()
        {
            if (session != null && session.ConsumeMissionSelectRequest()) ShowMissionSelect();
            else ShowShop();
        }
        public void ShowShop() => SetPanels(shopPanel);
        public void ShowMissionSelect() => SetPanels(missionSelectPanel);
        public void ShowInventory() => SetPanels(inventoryPanel);
        public void ShowNonsenseMarket() => SetPanels(nonsenseMarketPanel);

        private void SetPanels(GameObject activePanel)
        {
            if (shopPanel != null) shopPanel.SetActive(shopPanel == activePanel);
            if (missionSelectPanel != null) missionSelectPanel.SetActive(missionSelectPanel == activePanel);
            if (inventoryPanel != null) inventoryPanel.SetActive(inventoryPanel == activePanel);
            if (nonsenseMarketPanel != null) nonsenseMarketPanel.SetActive(nonsenseMarketPanel == activePanel);
        }
    }
}
