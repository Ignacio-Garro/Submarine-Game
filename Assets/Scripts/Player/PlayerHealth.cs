using Unity.Netcode;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    [SerializeField] RagdollController ragdollController;
    [SerializeField] int health = 100;
    bool dead = false;

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0) PlayerDies();
    }

    public void KnockBack(Vector3 direction, float force)
    {
        if (dead) return;
        GetComponent<Rigidbody>()?.AddForce(direction * force, ForceMode.Impulse);
    }

    public void PlayerRevives(int health)
    {
        dead = false;
        this.health = health;
        if (IsOwner)
        {
            ragdollController.Revive();
        }
    }


    void PlayerDies()
    {
        dead = true;
        ragdollController.Die(); 
    }

    
}
