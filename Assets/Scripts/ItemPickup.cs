using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * ItemPickup
 *
 * The purpose of ItemPickup is to represent an item
 * in the game world, ready to be picked up on a tile.
 * It is a GameObject which owns an Item, and if the
 * player triggers the pickup, the Item is added to their
 * inventory and the ItemPickup is destroyed.
 */
public class ItemPickup : MonoBehaviour
{
    // Settings for creating the item
    public string name;
    public string description;
    public Sprite icon;

    public string statName1;
    public int statValue1;
    
    public string statName2;
    public int statValue2;
    
    public string flagName1;
    public bool flagValue1;
    
    private Item _item;
    // Start is called before the first frame update
    void Start()
    {
        _item = new Item(name, description, icon);
        
        _item.Stats[statName1] = statValue1;
        _item.Stats[statName2] = statValue2;
        _item.Flags[flagName1] = flagValue1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
