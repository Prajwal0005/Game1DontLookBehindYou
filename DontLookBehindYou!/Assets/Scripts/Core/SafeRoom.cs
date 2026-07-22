using UnityEngine;
using DontLookBehindYou.Managers;

namespace DontLookBehindYou.Core
{
    [RequireComponent(typeof(Collider))]
    public class SafeRoom : MonoBehaviour
    {
        private void Awake()
        {
            Collider col = GetComponent<Collider>();
            col.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (EventManager.Instance != null)
                {
                    EventManager.Instance.TriggerSafeRoomStateChanged(true);
                    Debug.Log("[SafeRoom] Player entered Safe Room.");
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (EventManager.Instance != null)
                {
                    EventManager.Instance.TriggerSafeRoomStateChanged(false);
                    Debug.Log("[SafeRoom] Player exited Safe Room.");
                }
            }
        }
    }
}
