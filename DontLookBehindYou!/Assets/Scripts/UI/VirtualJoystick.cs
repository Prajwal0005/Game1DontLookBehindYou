using UnityEngine;
using UnityEngine.EventSystems;

namespace DontLookBehindYou.UI
{
    public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [Header("Settings")]
        public float movementRange = 100f;

        public RectTransform backgroundRect;
        public RectTransform handleRect;

        public Vector2 InputValue { get; private set; }

        private Vector2 defaultHandlePosition;

        private void Start()
        {
            if (handleRect != null)
            {
                defaultHandlePosition = handleRect.anchoredPosition;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (backgroundRect == null || handleRect == null) return;

            Vector2 position;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                backgroundRect, 
                eventData.position, 
                eventData.pressEventCamera, 
                out position);

            // Calculate offset relative to center
            Vector2 offset = position;
            offset = Vector2.ClampMagnitude(offset, movementRange);

            handleRect.anchoredPosition = defaultHandlePosition + offset;
            
            // Normalize value between -1 and 1
            InputValue = offset / movementRange;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            InputValue = Vector2.zero;
            if (handleRect != null)
            {
                handleRect.anchoredPosition = defaultHandlePosition;
            }
        }
    }
}
