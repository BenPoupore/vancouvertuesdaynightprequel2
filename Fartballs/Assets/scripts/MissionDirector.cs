using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace VipExtraction
{
    public enum MissionPhase
    {
        Briefing,
        Infiltration,
        Lockdown,
        Extraction,
        Complete
    }

    public sealed class MissionDirector : MonoBehaviour
    {
        [SerializeField] private GameSession session;
        [SerializeField] private MissionCatalog missionCatalog;
        [Min(0), SerializeField] private int baseReward = 4000;
        [Min(0), SerializeField] private int extractionBonus = 1000;
        [SerializeField] private string debriefScene = "Debrief";
        [SerializeField] private TMP_Text objectiveText;
        [SerializeField] private TMP_Text statusText;
        [SerializeField] private UnityEvent onLockdown;
        [SerializeField] private UnityEvent onMissionComplete;

        public event Action<MissionPhase> PhaseChanged;
        public MissionPhase Phase { get; private set; } = MissionPhase.Briefing;
        public bool CanExtract => Phase == MissionPhase.Extraction;
        private MissionDefinition activeMission;

        private void Start()
        {
            if (session == null)
            {
                Debug.LogError("MissionDirector requires a GameSession reference.", this);
                enabled = false;
                return;
            }

            if (missionCatalog != null)
            {
                missionCatalog.TryGet(session.Profile.selectedMissionId, out activeMission);
            }

            SetPhase(MissionPhase.Briefing, "Review the contract, then begin.");
            BeginMission();
        }

        public void BeginMission()
        {
            if (Phase != MissionPhase.Briefing)
            {
                return;
            }

            SetPhase(MissionPhase.Infiltration, "Find and eliminate the VIP.");
        }

        public void ReportVipKilled()
        {
            if (Phase != MissionPhase.Infiltration)
            {
                return;
            }

            SetPhase(MissionPhase.Lockdown, "VIP eliminated. Security lockdown initiated.");
            onLockdown?.Invoke();
            SetPhase(MissionPhase.Extraction, "Reach an extraction point.");
        }

        public bool TryExtract()
        {
            if (!CanExtract)
            {
                if (statusText != null)
                {
                    statusText.text = "Extraction unavailable: eliminate the VIP first.";
                }

                return false;
            }

            int earnedBase = activeMission != null ? activeMission.BaseReward : baseReward;
            int earnedExtraction = activeMission != null ? activeMission.ExtractionBonus : extractionBonus;
            int reward = earnedBase + earnedExtraction;
            string missionName = activeMission != null ? activeMission.DisplayName : "Completed Contract";
            session.RecordMissionResult(missionName, earnedBase, earnedExtraction);
            SetPhase(MissionPhase.Complete, $"Mission complete. Earned ${reward}.");
            onMissionComplete?.Invoke();
            if (string.IsNullOrWhiteSpace(debriefScene))
            {
                Debug.LogError("Debrief scene name is empty.", this);
                return false;
            }

            SceneManager.LoadScene(debriefScene);
            return true;
        }

        private void SetPhase(MissionPhase next, string message)
        {
            Phase = next;
            if (objectiveText != null) objectiveText.text = message;
            if (statusText != null) statusText.text = next.ToString();
            PhaseChanged?.Invoke(next);
        }
    }
}
