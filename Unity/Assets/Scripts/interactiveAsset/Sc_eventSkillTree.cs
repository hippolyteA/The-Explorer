using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class Sc_eventSkillTree : MonoBehaviour
{
    public enum TreeType { Competences, Consommables, Meal}

    public class SkillTreeData
    {
        GameObject owner;

        public GameObject player;
        public Sc_inventoryManager playerInventory;

        public GameObject UI;

        public int soulsNeed = 10;

        public void ResetValue(GameObject Owner)
        {
            owner = Owner;
            player = GameObject.FindGameObjectWithTag("Player");
            playerInventory = player.GetComponentInChildren<Sc_inventoryManager>();

            UI = GameObject.Find("SkillTreeUI");

            //UI.SetActive(false);
            //showSkillTree(false);
        }


        public void showSkillTree(bool activate)
        {
            UI.SetActive(activate);
        }


        public bool canAffordSkill()
        {
            if (soulsNeed <= playerInventory.soulRessource)
            {
                return true;
            }
            return false;
        }
    }
    [HideInInspector]
    public SkillTreeData Data = new SkillTreeData();

    // get button EventSystem.current.currentSelectedGameObject.
    public void moreDamage()
    {
        if (!Data.canAffordSkill())
        {
            return;
        }

        Data.playerInventory.soulRessource -= Data.soulsNeed;
        Data.soulsNeed = (int)(Data.soulsNeed * 1.5f);

        Sc_Shooter shooter = Data.player.GetComponent<Sc_Shooter>();
        shooter.weaponShotGun.shootDamage = (int)(shooter.weaponShotGun.shootDamage * 1.5f);
        shooter.weaponRevolver.shootDamage = (int)(shooter.weaponRevolver.shootDamage * 1.5f);
    }
    public void moreHealth()
    {
        if (!Data.canAffordSkill())
        {
            return;
        }

        Data.playerInventory.soulRessource -= Data.soulsNeed;
        Data.soulsNeed = (int)(Data.soulsNeed * 1.5f);

        Data.player.GetComponent<Sc_healthManager>().maxHealthPoint += 5;
    }

    public void craft(Sc_Item itemTocraft)
    {
        Sc_itemSlot[,] slotRequirement = itemTocraft.requirementToCraft;


        for (int y = 0; y <= slotRequirement.GetUpperBound(1); ++y)
        {
            for (int x = 0; x <= slotRequirement.GetUpperBound(0); ++x)
            {
                if (slotRequirement[x, y].Item == null)
                {
                    //Debug.Log("craft");
                    break;
                }
                else
                {
                    Sc_itemSlot itemNeeded = Data.playerInventory.findItemInInventory(slotRequirement[x, y].Item);
                    print(itemNeeded.ItemCount);
                    if (itemNeeded.ItemCount < slotRequirement[x, y].ItemCount)
                    {
                        Debug.Log("Don't Craft");
                        return;
                    }
                }

            }
        }

        //ultra moche mais j'ai pas trouvé mieux, l'opti c'est nul 
        for (int y = 0; y <= slotRequirement.GetUpperBound(1); ++y)
        {
            for (int x = 0; x <= slotRequirement.GetUpperBound(0); ++x)
            {
                if (slotRequirement[x, y].Item != null)
                {
                    Data.playerInventory.AddItem(slotRequirement[x, y].Item, -slotRequirement[x, y].ItemCount);
                }
            }
        }
        Data.playerInventory.AddItem(itemTocraft, 1);
        Debug.Log("craft");
    }
}
