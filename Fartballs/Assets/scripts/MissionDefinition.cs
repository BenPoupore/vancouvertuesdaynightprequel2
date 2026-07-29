using UnityEngine;

namespace VipExtraction
{
    [CreateAssetMenu(menuName = "VIP Extraction/Mission", fileName = "NewMission")]
    public sealed class MissionDefinition : ScriptableObject
    {
        [SerializeField] private string id = "mission_01";
        [SerializeField] private string displayName = "The First Contract";
        [TextArea(3, 8), SerializeField] private string briefing;
        [SerializeField] private string gameplayScene = "Mission01";
        [Min(0), SerializeField] private int baseReward = 4000;
        [Min(0), SerializeField] private int extractionBonus = 1000;

        public string Id => id;
        public string DisplayName => displayName;
        public string Briefing => briefing;
        public string GameplayScene => gameplayScene;
        public int BaseReward => baseReward;
        public int ExtractionBonus => extractionBonus;

        private void OnValidate()
        {
            id = id == null ? string.Empty : id.Trim();
            gameplayScene = gameplayScene == null ? string.Empty : gameplayScene.Trim();
            baseReward = Mathf.Max(0, baseReward);
            extractionBonus = Mathf.Max(0, extractionBonus);
        }
    }
}
