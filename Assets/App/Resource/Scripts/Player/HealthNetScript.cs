using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
namespace App.Resource.Scripts.Player
{
    public class HealthNetScript : NetworkBehaviour
    {
        [SerializeField] public float _startingHealth = 100f;
        [SerializeField] public float cooldown = 1.5f;
        [SerializeField] private bool _canDamage = true;
        [SerializeField] private NetworkVariable<float> _Health = new NetworkVariable<float>(100);
        public Image _healthBar;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            _Health.Value = _startingHealth;
            _Health.OnValueChanged += UpdateHealth;
        }
        private void UpdateHealth(float previousvalue, float newvalue) 
        {
            if (_healthBar != null)
            {
                _healthBar.fillAmount = newvalue / _startingHealth;
            }

            if (IsOwner)
            {
                if (newvalue < 0f)
                {
                    //FindObjectOfType<GameScript>().PlayerDeathRpc();
                    HasDiedRpc();
                }
            }
        }
        [Rpc(SendTo.Server)]
        public void HasDiedRpc()
        {
            NetworkObject.Despawn();
        }

        [Rpc(SendTo.Server, RequireOwnership =false)]
        public void DamageObjRpc(float dmg)
        {
            if (!_canDamage) return;
            _Health.Value -= dmg;
            StartCoroutine(nameof(DamageCooldown));
            Debug.Log($"Damage recieved: {dmg}");
        }

        private IEnumerable DamageCooldown()
        {
            _canDamage = false;
            yield return new WaitForSeconds(cooldown);
            _canDamage = true;
        }
    }
}
