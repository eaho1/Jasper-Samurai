﻿using UnityEngine;
using System.Collections;

public class SpiderBoss : Enemy {
    private int currentPhase = 1;
    private Vector2 leftSpawn, rightSpawn;
    private Player _player;
    private Animator _animator;
    /// Phase 1 Variables
    public GameObject projectilePrefab;
    public float projectileDamage = 5f;
    public float projectileForce = 400f;
    float shotFrequency = 1f;
    private float shotCooldown = 0f;
    private bool highShot = false;

    /// Phase 2 Variables    
    float smashFrequency = 2f;
    float smashSpeed = 1f;
    private float smashCooldown = 0f;


    void Awake()
    {
        base.Awake();
        leftSpawn = this.transform.FindChild("LeftSpawn").position;
        rightSpawn = this.transform.FindChild("RightSpawn").position;
        _player = GameObject.Find("Player").GetComponent<Player>();
        _animator = this.GetComponent<Animator>();
    }

    public void Initialize()
    {
        this.gameObject.SetActive(true);
        this.currentHealth = 1000;
        this.currentPhase = 1;
        this.smashCooldown = 0f;
    }

    public void Despawn()
    {
        this.gameObject.SetActive(false);
    }
		
	void Update () {
        base.Update();

        if (this.currentHealth <= 500)
        {
            this.currentPhase = 2;
        }

        if (currentPhase == 1)
        {
            PhaseOne();
        }
        else if (currentPhase == 2){
            PhaseTwo();
        }
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Projectile"))
        {
            ProjectileManager pjScript = other.GetComponent<ProjectileManager>();
            if (pjScript.projectileOwner == "Player")
            {                
                base.MakeInvulnerable(0.1f);
                _animator.Play("spiderBossParry");
            }
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("Player"))
        {
            this.DealDamage(_player, 10f);
        }
    }

    private void PhaseOne()
    {
        Shoot();
        FlipSides();
    }

    private void PhaseTwo()
    {
        WebSmash();
        Debug.Log(this.GetComponent<Rigidbody2D>().velocity.y);
        if (this.GetComponent<Rigidbody2D>().velocity.y < 0)
        {
            _animator.Play("spiderBossSmash");
        }
        //else
        //{
        //    _animator.Play("spiderBossSmash");
        //}
    }

    private void Shoot()
    {
        if (shotCooldown <= 0)
        {
            GameObject projectile = null;
            if (highShot)
            {
                projectile = (GameObject)Instantiate(projectilePrefab, transform.position + new Vector3(0, 1f, 0), Quaternion.identity);                
            }
            else
            {
                projectile = (GameObject)Instantiate(projectilePrefab, transform.position - new Vector3(0, 1f, 0), Quaternion.identity);
            }
            highShot = !highShot;

            projectile.GetComponent<ProjectileManager>().Initialize(this.tag, this.currentDirection, projectileDamage, projectileForce);

            Physics2D.IgnoreCollision(projectile.GetComponent<Collider2D>(), this.GetComponent<Collider2D>());
           
            shotCooldown = shotFrequency;
        }
        else
        {
            shotCooldown -= Time.deltaTime;
        }
    }

    private void WebSmash()
    {
        if (smashCooldown <= 0)
        {
            _animator.Play("spiderBossAngryIdle");
            this.transform.position = _player.transform.position + (Vector3.up * 5f);
            smashCooldown = smashFrequency;
        }
        else
        {
            smashCooldown -= Time.deltaTime;
        }
    }


    void FlipSides()
    {
        if ((int)this.currentHealth % 200 == 0)
        {
            this.transform.position = rightSpawn;
            this.currentDirection = Direction.Left;
        }
        else
        {
            this.transform.position = leftSpawn;
            this.currentDirection = Direction.Right;
        }
    }
}
