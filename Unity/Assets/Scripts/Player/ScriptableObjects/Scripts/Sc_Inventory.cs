using Sirenix.OdinInspector;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Inventory", menuName = "ScriptableObjects/Items/Inventory", order = 1)]
public class Sc_Inventory : SerializedScriptableObject
{
    [TabGroup("Consummables")]
    public Sc_itemSlot[,] consummableSlot = new Sc_itemSlot[8, 4];
    [TabGroup("Ressources")]
    public Sc_itemSlot[,] ressourcesSlot = new Sc_itemSlot[12, 6];
}
