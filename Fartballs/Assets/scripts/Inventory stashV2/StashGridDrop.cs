using UnityEngine;
using UnityEngine.EventSystems;

namespace VipExtraction
{
    public sealed class StashGridDrop : MonoBehaviour, IDropHandler
    {
        [SerializeField] private StashService stash;
        [SerializeField] private RectTransform gridRoot;
        [SerializeField] private StashGridController controller;

        public void OnDrop(PointerEventData eventData)
        {
            StashItemDrag drag =
                eventData.pointerDrag == null
                    ? null
                    : eventData.pointerDrag.GetComponent<StashItemDrag>();

            if (drag == null || drag.Instance == null)
            {
                return;
            }

            if (controller == null)
            {
                controller =
                    GetComponentInParent<StashGridController>();
                if (controller == null)
                {
                    controller =
                        FindAnyObjectByType<StashGridController>();
                }
            }

            if (stash == null ||
                gridRoot == null ||
                controller == null ||
                controller.CellSize <= 0f)
            {
                Debug.LogError(
                    "StashGridDrop has missing Inspector references.",
                    this);
                return;
            }

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                gridRoot,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 local);

            float cellSize = controller.CellSize;
            int x = Mathf.FloorToInt(
                (local.x +
                 gridRoot.rect.width * gridRoot.pivot.x) /
                cellSize);
            int y = Mathf.FloorToInt(
                (gridRoot.rect.height *
                 (1f - gridRoot.pivot.y) -
                 local.y) /
                cellSize);

            if (!stash.TryMoveToStash(
                    drag.Instance,
                    x,
                    y,
                    drag.Instance.rotated,
                    out string message))
            {
                Debug.Log(message, this);
            }
        }
    }
}
