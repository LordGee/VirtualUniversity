﻿using System;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

[CreateAssetMenu(menuName="uMMORPG Item/Monster Scroll", order=999)]
public class MonsterScrollItemTemplate : UsableItemTemplate {
    [Serializable]
    public struct SpawnInfo {
        public Monster monster;
        public int amount;
        public float distanceMultiplier;
    }

    [Header("Spawn")]
    public SpawnInfo[] spawns;

    public override void Use(Player player, int inventoryIndex) {
        foreach (SpawnInfo spawn in spawns) {
            if (spawn.monster != null) {
                for (int i = 0; i < spawn.amount; ++i) {
                    // summon in random circle position around the player
                    Vector2 circle2D = UnityEngine.Random.insideUnitCircle * spawn.distanceMultiplier;
                    Vector3 position = player.transform.position + new Vector3(circle2D.x, 0, circle2D.y);
                    GameObject go = Instantiate(spawn.monster.gameObject, position, Quaternion.identity);
                    go.name = spawn.monster.name; // avoid "(Clone)"
                    NetworkServer.Spawn(go);
                }
            }
        }

        // decrease amount
        ItemSlot slot = player.inventory[inventoryIndex];
        slot.DecreaseAmount(1);
        player.inventory[inventoryIndex] = slot;
    }
}