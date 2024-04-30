using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu]
public class LootTable : ScriptableObject
{
    [Serializable]
    public class Drop
    {
        public Loot LootToDrop;
        public int probability;
    }

    public List<Drop> table;

    public Loot GetDrop()
    {
        int roll = UnityEngine.Random.Range(0, 100);

        for(int i = 0; i < table.Count; i++)
        {
            roll -= table[i].probability;
            if (roll <0)
            {
                return table[i].LootToDrop;
            }
        }

        return table[0].LootToDrop;
    }
}

// loot number 0 in the table is 0..49
// loot number 1 in the table is 50..99
// if we roll 40 cycle will calculate 40 minus probality of the item number 0
// wich will be -40 because ours probability is 80
// that will return item number 0
