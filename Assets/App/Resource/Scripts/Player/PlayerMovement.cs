using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace App.Resource.Scripts.Player
{
    public class PlayerMovement : NetworkBehaviour
    {


        // Update is called once per frame
        private void Update()
        {
            if (!IsOwner) return;
            
                Vector3 moveDirection = new Vector3(x: 0, y: 0, z: 0);
                if (Input.GetKey(KeyCode.W)) moveDirection.z = +1f;
                if (Input.GetKey(KeyCode.S)) moveDirection.z = -1f;
                if (Input.GetKey(KeyCode.A)) moveDirection.x = -1f;
                if (Input.GetKey(KeyCode.D)) moveDirection.x = +1f;

            float moveSpeed = 3f;
            transform.position += moveDirection * (moveSpeed * Time.deltaTime);
        }
    }
}
