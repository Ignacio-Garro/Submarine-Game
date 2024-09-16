using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] int health = 100;
    bool dead = false;

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0) PlayerDies();
    }

    public void KnockBack(Vector3 direction, float force)
    {

    }

    public void PlayerRevives(int health)
    {
        dead = false;
        this.health = health;
    }


    void PlayerDies()
    {
        dead = true;
    }

    
}
