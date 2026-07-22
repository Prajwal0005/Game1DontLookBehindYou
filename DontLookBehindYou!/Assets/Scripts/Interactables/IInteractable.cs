using UnityEngine;

namespace DontLookBehindYou.Interactables
{
    public interface IInteractable
    {
        string GetInteractText();
        void Interact();
    }
}
