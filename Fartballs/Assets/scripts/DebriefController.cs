using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace VipExtraction
{
    public sealed class DebriefController : MonoBehaviour
    {
        [SerializeField] private GameSession session;
        [SerializeField] private TMP_Text missionNameText;
        [SerializeField] private TMP_Text baseRewardText;
        [SerializeField] private TMP_Text extractionBonusText;
        [SerializeField] private TMP_Text totalRewardText;
        [SerializeField] private TMP_Text balanceText;
        [SerializeField] private Button continueButton;
        [SerializeField] private string safehouseScene = "Safehouse";

        private void OnEnable()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void Start()
        {
            if (session == null ||
                missionNameText == null ||
                baseRewardText == null ||
                extractionBonusText == null ||
                totalRewardText == null ||
                balanceText == null ||
                continueButton == null)
            {
                Debug.LogError(
                    "DebriefController has missing references.",
                    this);
                enabled = false;
                return;
            }

            PlayerProfile profile = session.Profile;
            missionNameText.text = profile.lastMissionName;
            baseRewardText.text =
                $"Contract: ${profile.lastBaseReward}";
            extractionBonusText.text =
                $"Extraction bonus: ${profile.lastExtractionBonus}";
            totalRewardText.text =
                $"Total earned: ${profile.lastTotalReward}";
            balanceText.text = $"New balance: ${profile.money}";

            continueButton.onClick.AddListener(
                ReturnToSafehouse);
        }

        private void OnDestroy()
        {
            if (continueButton != null)
            {
                continueButton.onClick.RemoveListener(
                    ReturnToSafehouse);
            }
        }

        private void ReturnToSafehouse()
        {
            session.RequestMissionSelectAtSafehouse();
            SceneManager.LoadScene(safehouseScene);
        }
    }
}
