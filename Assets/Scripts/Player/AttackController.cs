/**
 * Copyright 2022 Connor Easterbrook
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
*/

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
