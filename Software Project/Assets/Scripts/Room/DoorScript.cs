﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public GameObject newPos;
    public bool locked = false;
    bool blocked;
    SpriteRenderer door;
    RoomTypes room;
    Countdown timeCountdown;
    public Sprite open, closed;
    PlayerStat player;
    // Start is called before the first frame update
    void Start()
    {
        door = GetComponentInChildren<SpriteRenderer>();
        room = GetComponentInParent<RoomTypes>();
        timeCountdown = GameObject.Find("Global").GetComponent<Countdown>();
        player = GameObject.Find("Player").GetComponent<PlayerStat>();

    }

    // Update is called once per frame
    void Update(){
        if (room.time && timeCountdown.timeRemaining > 0)
            locked = true;
        else if (room.enemyOn)
            locked = true;
        else if (blocked)
            Destroy(gameObject);
        else
            locked = false;
        //if (!locked)
        //    door.color = Color.green;
        //else
        //    door.color = Color.red;
        door.sprite = (locked) ? closed : open;

    }
    private void OnTriggerEnter2D(Collider2D other){
        if(other.name == "Player" && !locked)
        {
            other.transform.position = newPos.transform.position;
            player.GetComponent<AudioSource>().PlayOneShot(player.doorSound);
        }
        if (other.tag == "Blocked")
            blocked = true;
    }
    private void OnTriggerStay2D(Collider2D other){
        
    }
}
