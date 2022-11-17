#if UNITY_EDITOR

using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Ingredient", menuName = "ScriptableObjects/Items/Ingredient", order = 3)]
public class Sc_Ingredient : Sc_Item
{
    public enum IngredientType { Fruit }

    [BoxGroup(LEFT_VERTICAL_GROUP + "/General Settings")]
    [VerticalGroup(GENERAL_SETTINGS_VERTICAL_GROUP)]
    public IngredientType IngredientState;

    /*
    [OnInspectorInit]
    void setType()
    {
        switch (IngredientState)
        {
            case IngredientType.Fruit:
                ItemStackSize = 999;
                break;
        }
    }
    */

    public override Sc_itemType[] SupportedItemTypes
    {
        get
        {
            return new Sc_itemType[]
            {
               Sc_itemType.Ingredient
            };

        }
    }
}

#endif
