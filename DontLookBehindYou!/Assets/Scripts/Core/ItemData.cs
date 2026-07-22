using UnityEngine;

namespace DontLookBehindYou.Core
{
    [CreateAssetMenu(fileName = "NewItem", menuName = "DontLookBehindYou/Item Data")]
    public class ItemData : ScriptableObject
    {
        public string id;
        public string itemName;
        [TextArea]
        public string description;
        public Sprite icon;
        public bool isConsumable;
    }
}
