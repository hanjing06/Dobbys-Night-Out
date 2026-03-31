using System.Collections;
using UnityEngine;

public class CatSmackAbility: MonoBehaviour
{
    [Header("Unlock")]
    public bool isUnlocked = false;

    [Header("Input")]
    [SerializeField] private KeyCode smackKey = KeyCode.C;
    [SerializeField] private Animator animator;

    [Header("Attack Stats")]
    [SerializeField] private int damage = 1;
    [SerializeField] private float cooldown = 0.75f;
    [SerializeField] private float hitboxDuration = 0.12f;

    [Header("Hitbox")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private Vector2 hitboxSize = new Vector2(1.25f, 1.0f);
    [SerializeField] private LayerMask enemyLayers;

    [Header("Optional Knockback")]
    [SerializeField] private bool applyKnockback = false;
    [SerializeField] private float knockbackForce = 5f;

    private bool isOnCooldown = false;
    
    public int amtEquipped; //the number of times a certain power up is equipped from the inventory

    private void Update()
    {
        if (!isUnlocked)
        {
            return;
        }

        if (Input.GetKeyDown(smackKey) && !isOnCooldown)
        {
            animator.SetTrigger("smack");
            StartCoroutine(PerformCatSmack());
            
            //lock the ability if all equipped are used 
            amtEquipped--;
            if (amtEquipped <= 0)
            {
                LockAbility();
                Debug.Log("Cat Smack is unavailable");
            }
        }
    }

    private IEnumerator PerformCatSmack()
    {
        Debug.Log("smacked");
        isOnCooldown = true;

        DealDamage();

        yield return new WaitForSeconds(cooldown);

        isOnCooldown = false;
    }

    private void DealDamage()
    {
        if (attackPoint == null)
        {
            Debug.LogWarning("CatSmackAbility: No attackPoint assigned.");
            return;
        }

        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(
            attackPoint.position,
            hitboxSize,
            0f,
            enemyLayers
        );
        
        Debug.Log("Enemies hit: " + hitEnemies.Length);

        foreach (Collider2D enemy in hitEnemies)
        {
            IDamageable damageable = enemy.GetComponent<IDamageable>();

            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }

            if (applyKnockback)
            {
                Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();

                if (enemyRb != null)
                {
                    Vector2 direction = (enemy.transform.position - transform.position).normalized;
                    enemyRb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
                }
            }
        }
    }

    public void UnlockAbility()
    {
        isUnlocked = true;
    }

    public void LockAbility()
    {
        isUnlocked = false;
    }

    public bool IsUnlocked()
    {
        return isUnlocked;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
        {
            return;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackPoint.position, hitboxSize);
    }
}