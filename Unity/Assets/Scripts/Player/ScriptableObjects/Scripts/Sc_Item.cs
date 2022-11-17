/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Item", order = 1)]
public class Sc_Item : ScriptableObject
{
    //public float stack;
    public float price;
}
*/

#if UNITY_EDITOR

using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

    // 
    // This is the base-class for all items. It contains a lot of layout using various layout group attributes. 
    // We've also defines a few relevant groups in constant variables, which derived classes can utilize.
    // 
    // Also note that each item deriving from this class, needs to specify which Item types are
    // supported via the SupporteItemTypes property. This is then referenced in ValueDropdown attribute  
    // on the Type field, so that when users only can specify supported item-types.  
    // 

public abstract class Sc_Item : SerializedScriptableObject
{
    protected const string LEFT_VERTICAL_GROUP = "Split/Left";
    protected const string RIGHT_VERTICAL_GROUP = "Split/Right";
    protected const string STATS_BOX_GROUP = "Split/Left/Stats";
    protected const string GENERAL_SETTINGS_VERTICAL_GROUP = "Split/Left/General Settings/Split/Right";

    [HideLabel, PreviewField(55)]
    [VerticalGroup(LEFT_VERTICAL_GROUP)]
    [HorizontalGroup(LEFT_VERTICAL_GROUP + "/General Settings/Split", 55, LabelWidth = 110)]
    public Texture Icon;

    [BoxGroup(LEFT_VERTICAL_GROUP + "/General Settings")]
    [VerticalGroup(GENERAL_SETTINGS_VERTICAL_GROUP)]
    public string Name;

    /*
    //[HorizontalGroup("Split", 0.5f, MarginLeft = 5, LabelWidth = 130)]
    [BoxGroup(RIGHT_VERTICAL_GROUP, false)]
    [TextArea(5, 4)]
    public string Description;
    */

    //[BoxGroup(RIGHT_VERTICAL_GROUP + "/Description")]
    //[VerticalGroup(GENERAL_SETTINGS_VERTICAL_GROUP)]
    //[HideLabel, TextArea(4, 14)]
    //public string Description;

    [HorizontalGroup("Split", 0.8f, MarginLeft = 5, LabelWidth = 130)]
    //[BoxGroup("Split/Right/Notes")]
    //[HideLabel, TextArea(4, 9)]
    //public string Notes;

    [VerticalGroup(GENERAL_SETTINGS_VERTICAL_GROUP)]
    [ValueDropdown("SupportedItemTypes")]
    [ValidateInput("IsSupportedType")]
    //public abstract Sc_itemType Type { get; }
    public Sc_itemType Type;

    //[VerticalGroup("Split/Right")]
    //public StatList Requirements;

    //[AssetsOnly]
    //[VerticalGroup(GENERAL_SETTINGS_VERTICAL_GROUP)]
    //public GameObject Prefab;

    [BoxGroup(STATS_BOX_GROUP)]
    public float price;

    [BoxGroup(STATS_BOX_GROUP)]
    public float ItemRarity;

    [DisableInEditorMode]
    [BoxGroup(STATS_BOX_GROUP)]
    public int ItemStackSize = 999;


    [TabGroup("Crafting")]
    public Sc_itemSlot[,] requirementToCraft = new Sc_itemSlot[5, 1];


    public abstract Sc_itemType[] SupportedItemTypes { get; }

    private bool IsSupportedType(Sc_itemType type)
    {
       return this.SupportedItemTypes.Contains(type);
    }
}

#endif