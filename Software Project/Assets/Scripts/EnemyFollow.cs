﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFollow : MonoBehaviour
{
    public int hp = 20;
    public float speed;
    public float normalSpeed;
    public float coldSpeed;
    public float stoppingDistance;
    public float retreatDistance;
    public float attackCooldown;
    public float startAtkCooldown;
    public float moveCooldown;
    public float startMvCooldown;
    public float frozenCooldown;
    float heatTimer = 1f;
    float coldTimer = 1f;
    public bool ranged;
    public bool frozen = false;
    public bool suddenDeath = false;
    public bool dead = false;
    public bool bpSpawn = false;
    public GameObject projectile;
    public GameObject BP;
    public GameObject Corpse;
    private Transform target;
    public GameObject hitEffect;
    public Color normalColor;
    public Color frozenColor;
    public Color deathColor;
    PlayerStat player;
    PlayerMovement playerMove;
    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.Find("Player").GetComponent<Transform>();
        player = GameObject.Find("Player").GetComponent<PlayerStat>();
        playerMove = GameObject.Find("Player").GetComponent<PlayerMovement>();
        //stoppingDistance = Random.Range(25, 31);
        //Physics2D.IgnoreLayerCollision(10, 10, true); 
    }

    // Update is called once per frame
    void Update()
    {
        //Kill Cheat
        if (Input.GetKey(KeyCode.F1))
        {
            hp = 0;
        }
        movement();
        heatstroke();
        coldZone();
        staticShock();
        //Death
        if (hp <= 0){
            Instantiate(Corpse, transform.position, Quaternion.identity);
            Instantiate(BP, transform.position * 1.02f, Quaternion.identity);
            Destroy(gameObject);
        }
        if (moveCooldown > 0)
            moveCooldown -= Time.deltaTime;
        if (frozenCooldown <= 0)
        {
            gameObject.GetComponent<SpriteRenderer>().color = normalColor;
        }
        else if (frozenCooldown > 0 && !dead)
        {
            frozenCooldown -= Time.deltaTime;
            gameObject.GetComponent<SpriteRenderer>().color = frozenColor;
        }
    }
    void movement()
    {
        //Close Range 
        if (!ranged && frozenCooldown <= 0)
        {
            if (Vector2.Distance(transform.position, target.position) > stoppingDistance && moveCooldown <= 0)
            {
                transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
                attackCooldown = startAtkCooldown;
            }
            else if (Vector2.Distance(transform.position, target.position) <= stoppingDistance)
            {
                attackCooldown -= Time.deltaTime;
                moveCooldown = startMvCooldown;
                if (attackCooldown <= 0 && player.hp > 0 && Vector2.Distance(transform.position, target.position) <= stoppingDistance)
                {
                    enemyCloseAtk();

                }
            }
        }
        //Long Range
        if (ranged && frozenCooldown <= 0)
        {
            if (Vector2.Distance(transform.position, target.position) > stoppingDistance && moveCooldown <= 0)
            {
                transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            }
            else if (Vector2.Distance(transform.position, target.position) < stoppingDistance && Vector2.Distance(transform.position, target.position) > retreatDistance)
            {
                transform.position = this.transform.position;
            }
            if (Vector2.Distance(transform.position, target.position) < retreatDistance && moveCooldown <= 0)
            {
                transform.position = Vector2.MoveTowards(transform.position, target.position, -speed * Time.deltaTime);
            }
            enemyRangeAtk();
        }
    }
    void enemyCloseAtk(){
        player.Damage(Random.Range(10, 20));
        attackCooldown = startAtkCooldown;
    }
    void enemyRangeAtk(){
        if (attackCooldown <= 0 && moveCooldown <= 0){
            Instantiate(projectile, transform.position, Quaternion.identity);
            attackCooldown = startAtkCooldown;
            moveCooldown = startMvCooldown;
        }
        else
            attackCooldown -= Time.deltaTime;
    }
    void heatstroke()
    {
        if (player.heat){
            if (heatTimer > 0)
                heatTimer -= Time.deltaTime;
            else if (heatTimer <= 0)
            {
                GameObject hit = Instantiate(hitEffect, transform.position, Quaternion.identity) as GameObject;
                hit.GetComponent<ParticleSystem>().Play();
                heatTimer = 1f;
                hp -= 3;
            }
        }
        if (!player.heat)
            heatTimer = 2f;
    }
    void coldZone()
    {
        if (player.cold)
        {
            speed = coldSpeed;
            if (coldTimer > 0)
                coldTimer -= Time.deltaTime;
            else if (coldTimer <= 0)
                coldTimer = 2f;
        }
        if (!player.cold)
        {
            coldTimer = 1f;
            speed = normalSpeed;
        }          
    }
    void staticShock()
    {
        if (player.shock && player.shockDam && player.shockCoolDown <= 0){
            player.shockCoolDown = 0.2f;
            GameObject hit = Instantiate(hitEffect, transform.position, Quaternion.identity) as GameObject;
            hit.GetComponent<ParticleSystem>().Play();
            hp -= 10;
        }
        
       
    }
    void Damage(int dam)
    {
        GameObject hit = Instantiate(hitEffect, transform.position, Quaternion.identity) as GameObject;
        hit.GetComponent<ParticleSystem>().Play();
        Destroy(hit, 1f);
        if (!suddenDeath)
            hp -= dam;
        else
            hp = 0;
    }
    private void OnTriggerEnter2D(Collider2D other){
        if(other.CompareTag("Bullet")){
            Damage(player.damDict["bulletDam"]);
            Destroy(other.gameObject);              
        }
        if (other.CompareTag("Shell"))
        {
            Damage(player.damDict["shellDam"]);
            Destroy(other.gameObject);
        }
        if (other.CompareTag("Laser"))
        {
            Damage(player.damDict["laserDam"]);
            Destroy(other.gameObject);
        }
        if (other.CompareTag("Fire"))
        {
            Damage(player.damDict["fireDam"]);
        }
        if (other.CompareTag("Melee"))
        {
            Damage(player.damDict["meleeDam"]);
        }
        if (other.CompareTag("Freeze"))
        {
            GameObject hit = Instantiate(hitEffect, transform.position, Quaternion.identity) as GameObject;
            hit.GetComponent<ParticleSystem>().Play();
            Destroy(hit, 1f);
            Damage(player.damDict["freezeDam"]);
            frozenCooldown = 1.5f;
            Destroy(other.gameObject);
        }
        if (other.CompareTag("Pulse"))
        {
            Damage(player.damDict["pulseDam"]);
        }
        if (other.CompareTag("Bolt") && playerMove.isDash)
        {
            Damage(player.damDict["boltDam"]);
        }

    }
}
