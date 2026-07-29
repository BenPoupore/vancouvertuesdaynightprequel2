using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace VipExtraction
{
    public sealed class BriefingController : MonoBehaviour
    {
        [SerializeField] private GameSession session;
        [SerializeField] private MissionCatalog missionCatalog;
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text briefingText;
        [SerializeField] private TMP_Text rewardText;
        [SerializeField] private Button startMissionButton;
        [SerializeField] private Button backButton;
        [SerializeField] private string safehouseScene = "Safehouse";

        private MissionDefinition selectedMission;

        private void Start()
        {
            if (session == null || missionCatalog == null || titleText == null ||
                briefingText == null || rewardText == null || startMissionButton == null)
            {
                Debug.LogError("BriefingController has missing references.", this);
                enabled = false;
                return;
            }

            if (!missionCatalog.TryGet(session.Profile.selectedMissionId, out selectedMission))
            {
                titleText.text = "No contract selected";
                briefingText.text = "Return to the safehouse and select a mission.";
                rewardText.text = string.Empty;
                startMissionButton.interactable = false;
            }
            else
            {
                titleText.text = selectedMission.DisplayName;
                briefingText.text = selectedMission.Briefing;
                rewardText.text = $"Maximum payout: ${selectedMission.BaseReward + selectedMission.ExtractionBonus}";
                startMissionButton.onClick.AddListener(StartMission);
            }

            if (backButton != null) backButton.onClick.AddListener(ReturnToSafehouse);
        }

        private void OnDestroy()
        {
            if (startMissionButton != null) startMissionButton.onClick.RemoveListener(StartMission);
            if (backButton != null) backButton.onClick.RemoveListener(ReturnToSafehouse);
        }

        private void StartMission()
        {
            if (selectedMission != null && !string.IsNullOrWhiteSpace(selectedMission.GameplayScene))
                SceneManager.LoadScene(selectedMission.GameplayScene);
        }

        private void ReturnToSafehouse() => SceneManager.LoadScene(safehouseScene);
    }
}
