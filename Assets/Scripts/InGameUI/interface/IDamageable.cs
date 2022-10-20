using UnityEngine;
public interface IDamageable
{
    public void takeDamage(float damage);
    public bool isDead();
    public string dropItem();
}
