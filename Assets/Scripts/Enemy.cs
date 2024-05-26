using CoolDawn.Player;
using UnityEngine;

namespace CoolDawn
{
    public class Enemy : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer != LayerMask.NameToLayer("Player")) return;
            other.GetComponent<PlayerController>().Respawn();
        }
    }
}
