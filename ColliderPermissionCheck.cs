/*
 * ColliderPermissionChecks
 * Author: AussieGuy92
 * License: Open Source (MIT License)
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 */

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ColliderPermissionCheck : UdonSharpBehaviour
{
    public Collider restrictedCollider;
    public Transform teleportLocation;
    public VRCPlayerApi[] playersWithPermission = new VRCPlayerApi[50]; // Assuming a max of 50 players with permission
    private VRCPlayerApi[] allPlayers = new VRCPlayerApi[80]; // Assuming a max of 80 players in the instance

    private void Start()
    {
        // Add the local player to the array when the script starts
        AddPlayerToArray(Networking.LocalPlayer, allPlayers);
    }

    public override void OnPlayerJoined(VRCPlayerApi player)
    {
        AddPlayerToArray(player, allPlayers);
    }

    public override void OnPlayerLeft(VRCPlayerApi player)
    {
        RemovePlayerFromArray(player, allPlayers);
    }

    private void OnTriggerEnter(Collider other)
    {
        VRCPlayerApi player = other.GetComponent<VRCPlayerApi>();
        if (player == null) return;

        if (!HasPermission(player))
        {
            player.TeleportTo(teleportLocation.position, teleportLocation.rotation);
        }
    }

    private void Update()
    {
        // Check all players inside the collider every frame
        foreach (VRCPlayerApi player in allPlayers)
        {
            if (player == null) continue;
            if (restrictedCollider.bounds.Contains(player.GetPosition()) && !HasPermission(player))
            {
                player.TeleportTo(teleportLocation.position, teleportLocation.rotation);
            }
        }
    }

    public void GrantPermission(VRCPlayerApi player)
    {
        if (!HasPermission(player))
        {
            AddPlayerToArray(player, playersWithPermission);
        }
    }

    private bool HasPermission(VRCPlayerApi player)
    {
        foreach (VRCPlayerApi permittedPlayer in playersWithPermission)
        {
            if (permittedPlayer == player)
            {
                return true;
            }
        }
        return false;
    }

    private void AddPlayerToArray(VRCPlayerApi player, VRCPlayerApi[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == null)
            {
                array[i] = player;
                break;
            }
        }
    }

    private void RemovePlayerFromArray(VRCPlayerApi player, VRCPlayerApi[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == player)
            {
                array[i] = null;
                break;
            }
        }
    }
}
