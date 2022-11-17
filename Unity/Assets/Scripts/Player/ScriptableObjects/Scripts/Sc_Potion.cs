
#if UNITY_EDITOR

using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Potion", menuName = "ScriptableObjects/Items/Potion", order = 2)]
public class Sc_Potion : Sc_Item
{       
    public enum SizeType { Small, Medium, Big };
    public enum PotionEffectType { Heal, Buff }

    [ShowIf("Type", Sc_itemType.Potion)]
    [BoxGroup(LEFT_VERTICAL_GROUP + "/General Settings")]
    [VerticalGroup(GENERAL_SETTINGS_VERTICAL_GROUP)]
    public PotionEffectType Effect;

    [ShowIf("Type", Sc_itemType.Potion)]
    [OnValueChanged("setType")]
    //[BoxGroup(LEFT_VERTICAL_GROUP + "/General Settings")]
    //[VerticalGroup(GENERAL_SETTINGS_VERTICAL_GROUP)]
    [BoxGroup(STATS_BOX_GROUP)]
    //[HorizontalGroup("/Delay")]
    //[LabelWidth(20)]
    public SizeType Size;
    [OnInspectorInit]
    void setType()
    {
        switch (Size)
        {
            case SizeType.Small:
                ConsumeTime = 0.6f;
                //ItemStackSize = 20;
                break;
            case SizeType.Medium:
                ConsumeTime = 1f;
                //ItemStackSize = 10;
                break;
            case SizeType.Big:
                ConsumeTime = 1.8f;
                //ItemStackSize = 5;
                break;
        }
    }

    [HideLabel]
    [DisableInEditorMode]
    //[BoxGroup(LEFT_VERTICAL_GROUP + "/General Settings")]
    //[VerticalGroup(GENERAL_SETTINGS_VERTICAL_GROUP)]
    [BoxGroup(STATS_BOX_GROUP)]
    //[HorizontalGroup("/Delay")]
    [SuffixLabel("Consumme time in seconds ", true)]
    //[LabelWidth(30)]
    public float ConsumeTime;

    [ShowIf("Effect", PotionEffectType.Heal)]
    [BoxGroup(STATS_BOX_GROUP)]
    //[HorizontalGroup(STATS_BOX_GROUP)]
    public int heal;


    //[SuffixLabel("seconds ", true)]
    //[BoxGroup(STATS_BOX_GROUP)]
    //public float Cooldown;

    //[HorizontalGroup(STATS_BOX_GROUP + "/Dur")]
    //public bool ConsumeOverTime;

    //[VerticalGroup(LEFT_VERTICAL_GROUP)]
    //public StatList Modifiers;

    public override Sc_itemType[] SupportedItemTypes
    {
       get
       {
           return new Sc_itemType[]
           {
               Sc_itemType.Potion,
               Sc_itemType.Flask
           };

       }
    }
}

#endif
