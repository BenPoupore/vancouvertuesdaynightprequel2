using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace VipExtraction
{
    public sealed class MissionSelectController : MonoBehaviour
    {
        [SerializeField] private GameSession session;
        [SerializeField] private MissionCatalog missionCatalog;
        [SerializeField] private TMP_Dropdown missionDropdown;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private Button briefingButton;
        [SerializeField] private string briefingScene = "Briefing";

        private readonly List<MissionDefinition> shownMissions = new List<MissionDefinition>();

        private void Start()
        {
            if (session == null || missionCatalog == null || missionDropdown == null ||
                descriptionText == null || briefingButton == null)
            {
                Debug.LogError("MissionSelectController has missing references.", this);
                enabled = false;
                return;
            }

            foreach (MissionDefinition mission in missionCatalog.Missions)
            {
                if (mission != null) shownMissions.Add(mission);
            }

            var names = new List<string>();
            foreach (MissionDefinition mission in shownMissions) names.Add(mission.DisplayName);
            missionDropdown.ClearOptions();
            missionDropdown.AddOptions(names);
            missionDropdown.onValueChanged.AddListener(ShowSelection);
            briefingButton.onClick.AddListener(OpenBriefing);
            ShowSelection(missionDropdown.value);
        }

        private void OnDestroy()
        {
            if (missionDropdown != null) missionDropdown.onValueChanged.RemoveListener(ShowSelection);
            if (briefingButton != null) briefingButton.onClick.RemoveListener(OpenBriefing);
        }

        private MissionDefinition Selected()
        {
            int index = missionDropdown.value;
            return index >= 0 && index < shownMissions.Count ? shownMissions[index] : null;
        }

        private void ShowSelection(int _)
        {
            MissionDefinition mission = Selected();
            descriptionText.text = mission == null
                ? "No contracts available."
                : $"{mission.Briefing}\n\nReward: ${mission.BaseReward + mission.ExtractionBonus}";
            briefingButton.interactable = mission != null;
        }

        private void OpenBriefing()
        {
            MissionDefinition mission = Selected();
            if (mission == null || string.IsNullOrWhiteSpace(briefingScene)) return;
            session.SelectMission(mission.Id);
            SceneManager.LoadScene(briefingScene);
        }
    }
}
