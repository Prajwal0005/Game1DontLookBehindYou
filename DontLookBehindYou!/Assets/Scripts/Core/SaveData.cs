using System;
using System.Collections.Generic;
using UnityEngine;

namespace DontLookBehindYou.Core
{
    [Serializable]
    public class SaveData
    {
        public string lastSavedScene;
        public float playerHealth;
        public float playerStamina;
        public float flashlightBattery;
        public float stressLevel;
        
        public float[] playerPosition = new float[3];
        public float[] playerRotation = new float[4];

        public List<string> inventoryItemIDs = new List<string>();
        public List<string> solvedPuzzleIDs = new List<string>();
        public List<string> unlockedDoorIDs = new List<string>();
    }
}
