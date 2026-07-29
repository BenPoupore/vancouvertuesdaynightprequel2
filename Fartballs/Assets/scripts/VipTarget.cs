using UnityEngine;

namespace VipExtraction
{
    [RequireComponent(typeof(global::Health))]
    public sealed class VipTarget : MonoBehaviour
    {
        [SerializeField] private MissionDirector director;
        private global::Health health;

        private void Awake()
        {
            health = GetComponent<global::Health>();
        }

        private void OnEnable()
        {
            health.onDeath.AddListener(NotifyVipKilled);
        }

        private void OnDisable()
        {
            health.onDeath.RemoveListener(NotifyVipKilled);
        }

        public void NotifyVipKilled()
        {
            if (director == null)
            {
                Debug.LogError("VIP has no MissionDirector reference.", this);
                return;
            }

            director.ReportVipKilled();
        }
    }
}
