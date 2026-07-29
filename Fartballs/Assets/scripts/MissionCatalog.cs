using System.Collections.Generic;
using UnityEngine;

namespace VipExtraction
{
    [CreateAssetMenu(menuName = "VIP Extraction/Mission Catalog", fileName = "MissionCatalog")]
    public sealed class MissionCatalog : ScriptableObject
    {
        [SerializeField] private List<MissionDefinition> missions = new List<MissionDefinition>();
        public IReadOnlyList<MissionDefinition> Missions => missions;

        public bool TryGet(string missionId, out MissionDefinition mission)
        {
            mission = missions.Find(candidate => candidate != null && candidate.Id == missionId);
            return mission != null;
        }
    }
}
