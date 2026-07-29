using UnityEngine;

namespace VipExtraction
{
    [RequireComponent(typeof(Collider))]
    public sealed class ExtractionZone : MonoBehaviour
    {
        [SerializeField] private MissionDirector director;

        private void Reset()
        {
            Collider zoneCollider = GetComponent<Collider>();
            zoneCollider.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (director == null)
            {
                Debug.LogError("ExtractionZone requires a MissionDirector reference.", this);
                return;
            }

            if (other.GetComponentInParent<PlayerMarker>() != null)
            {
                director.TryExtract();
            }
        }
    }
}
