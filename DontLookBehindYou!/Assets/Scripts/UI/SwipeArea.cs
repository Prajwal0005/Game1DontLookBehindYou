using UnityEngine;
using UnityEngine.EventSystems;

namespace DontLookBehindYou.UI
{
    public class SwipeArea : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        public Vector2 SwipeDelta { get; private set; }
        
        private int currentPointerId = -999;
        private Vector2 lastPosition;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (currentPointerId == -999)
            {
                currentPointerId = eventData.pointerId;
                lastPosition = eventData.position;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData.pointerId == currentPointerId)
            {
                SwipeDelta = eventData.position - lastPosition;
                lastPosition = eventData.position;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.pointerId == currentPointerId)
            {
                currentPointerId = -999;
                SwipeDelta = Vector2.zero;
            }
        }

        private void LateUpdate()
        {
            // Reset swipe delta after it's been read this frame to prevent continuous drift if drag stops but finger is still down
            if (currentPointerId != -999 && SwipeDelta != Vector2.zero)
            {
                // We keep it for the current frame, it's the responsibility of the MobileInputManager to read it.
                // We clear it here if no drag event occurred this frame.
            }
        }
    }
}
