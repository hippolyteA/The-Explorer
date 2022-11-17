using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class Sc_inventoryManager : MonoBehaviour
{
    public bool saveInventoryDuringPlay = true;

    public Sc_Inventory inventory;

    //Soul Variables
    public TextMeshProUGUI soulText;

    public int soulRessource;
    int soulToAdd;
    //Soul Variables

    //!!test!!
    //public List<Sc_Potion> Potions = new List<Sc_Potion>();

    private Sc_PoolsManager poolsManager;

    private void Start()
    {
        GetComponent<Sc_healthManager>().OnDeathEvents.Add(letSoulOnDeath);

        soulText.text = soulRessource.ToString();
        poolsManager = GameObject.FindGameObjectWithTag("Pools").GetComponent<Sc_PoolsManager>();

        if (!saveInventoryDuringPlay)
        {
            inventory = Instantiate(inventory);
        }
    }


    #region Soul

    public void GainSouls(int souls)
    {
        StartCoroutine(delayAddSoulRessource(souls));
    }
    IEnumerator delayAddSoulRessource(int souls)
    {
        soulToAdd += souls;

        soulText.text = soulRessource.ToString() + " +" + soulToAdd.ToString();

        yield return new WaitForSeconds(1.75f);

        if (!isAdding) StartCoroutine(AddSoulRessource((int)Mathf.Sign(souls)));
    }

    bool isAdding;
    IEnumerator AddSoulRessource(int sign)
    {
        isAdding = true;
        float delay = 0.1f;
        DOTween.To(() => delay, x => delay = x, delay / 50, 6f).SetEase(Ease.InSine);

        while (soulToAdd != 0)
        {
            soulRessource += 1 * sign;
            soulToAdd -= 1 * sign;

            soulText.text = soulRessource.ToString() + " +" + soulToAdd.ToString();

            yield return new WaitForSeconds(delay);
        }
        isAdding = false;

        soulText.text = soulRessource.ToString();
    }

    void letSoulOnDeath()
    {
        int soulGiven = soulRessource;
        soulRessource = 0;

        GameObject goSoul = poolsManager.ballToSpawn(Sc_PoolsManager.poolType.Soul);
        goSoul.transform.position = transform.position;

        Sc_Loot soul = goSoul.GetComponent<Sc_Loot>();
        soul.moverState = Sc_Loot.moverType.Inactif;
        soul.ressourceGiven = soulGiven;
        soul.WakeUp();
    }

    #endregion Soul


    #region Item

    public void AddItem(Sc_Item item, int Add)
    {
        Sc_itemSlot[,] Slot = null;
        switch (item.Type)
        {
            case (Sc_itemType.Potion):
                {
                    Slot = inventory.consummableSlot;
                    break;
                }
            case (Sc_itemType.Ingredient):
                {
                    Slot = inventory.ressourcesSlot;
                    break;
                }
        }

        for (int y = 0; y <= Slot.GetUpperBound(1); ++y)
        {
            for (int x = 0; x <= Slot.GetUpperBound(0); ++x)
            {
                if (Slot[x, y].Item == null)
                {
                    Slot[x, y].Item = item;
                    Slot[x, y].ItemCount += Add;
                    return;
                }
                else if (Slot[x, y].Item == item)
                {
                    if (Slot[x, y].ItemCount < item.ItemStackSize)
                    {
                        Slot[x, y].ItemCount += Add;
                    }
                    return;
                }
                Debug.Log("Add code to expand inventory (Add Item)");
            }
        }

    }

    public Sc_itemSlot findItemInInventory(Sc_Item item)
    {
        Sc_itemSlot[,] Slot = null;
        switch (item.Type)
        {
            case (Sc_itemType.Potion):
                {
                    Slot = inventory.consummableSlot;
                    break;
                }
            case (Sc_itemType.Ingredient):
                {
                    Slot = inventory.ressourcesSlot;
                    break;
                }
        }

        for (int y = 0; y <= Slot.GetUpperBound(1); ++y)
        {
            for (int x = 0; x <= Slot.GetUpperBound(0); ++x)
            {
                if (Slot[x, y].Item == item)
                {
                    return Slot[x, y];
                }
            }
        }

        return new Sc_itemSlot();
    }
    public int isItemInInventory(Sc_Item item)
    {
        Sc_itemSlot[,] Slot = null;
        switch (item.Type)
        {
            case (Sc_itemType.Potion):
                {
                    Slot = inventory.consummableSlot;
                    break;
                }
            case (Sc_itemType.Ingredient):
                {
                    Slot = inventory.ressourcesSlot;
                    break;
                }
        }

        for (int y = 0; y <= Slot.GetUpperBound(1); ++y)
        {
            for (int x = 0; x <= Slot.GetUpperBound(0); ++x)
            {
                if (Slot[x, y].Item == item)
                {
                    return Slot[x, y].ItemCount;
                }
            }
        }

        return 0;
    }


    #region Consummable



    #endregion Consummable

    #region Ingredient



    #endregion Ingredient


    #endregion Item
}
