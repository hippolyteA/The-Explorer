#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Demos.RPGEditor
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    //[CreateAssetMenu(fileName = "Inventory", menuName = "ScriptableObjects/Items/Inventory", order = 1)]
    public class Inventory : SerializedScriptableObject
    {
        [TabGroup("Starting Inventory")]
        public ItemSlot[,] WorkPls = new ItemSlot[12, 6];
    }
}
#endif
