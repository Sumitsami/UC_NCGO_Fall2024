using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Random = UnityEngine.Random;

public class ammoSpawner : NetworkBehaviour
{
    public NetworkObject Ammo;
    [SerializeField] private float tickTime = 6f;
    [SerializeField] private float currentTime = 6f;

    public void FixedUpdate()
    {
        currentTime -= Time.deltaTime;
        if (currentTime<= 0) 
        {
            SpawnAmmoRpc();

            currentTime = Random.Range(tickTime, tickTime + 5f);
        }
    }
    [ContextMenu("SpawnAmmo")]
    [Rpc(SendTo.Server)]
    public void SpawnAmmoRpc()
    {
        NetworkObject ammo = NetworkManager.Instantiate(Ammo, transform.position, transform.rotation);
        ammo.Spawn(true);
    }
}
