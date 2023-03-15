using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    public Animator _animator;
    private int currentAttackCounter;
    private bool isMelee;
    public Collider swordCollider;
    public Collider gunCollider;
    private bool sword = true;

    void Awake()
    {
        _animator.SetBool("holdingSword", true);
    }

    void Update()
    {
        WeaponChange();

        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }
    }

    private void WeaponChange()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            sword = true;
            _animator.SetBool("holdingSword", true);

            gunCollider.gameObject.SetActive(false);
            swordCollider.gameObject.SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            sword = false;
            _animator.SetBool("holdingSword", false);

            swordCollider.gameObject.SetActive(false);
            gunCollider.gameObject.SetActive(true);
        }
    }

    private void Attack()
    {
        if (sword)
        {
            SwordAttack();
        }
        else
        {
            MusketAttack();
        }
    }

    private async void SwordAttack()
    {
        _animator.SetBool("attacking", true);

        await Task.Delay(1000);
        _animator.SetBool("attacking", false);
    }

    private async void MusketAttack()
    {
        _animator.SetBool("attacking", true);

        await Task.Delay(1000);
        _animator.SetBool("attacking", false);
    }
}
