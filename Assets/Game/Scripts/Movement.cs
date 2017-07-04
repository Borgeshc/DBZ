using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float moveSpeed;

    float minYClamp = -3.23f;
    float maxYClamp = 4.55f;

    float minXClamp = -9.4f;
    float maxXClamp = 9.4f;

    Rigidbody2D rb;
    SoundManager soundManager;
    PlayerManager playerManager;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        soundManager = GetComponent<SoundManager>();
        playerManager = GetComponent<PlayerManager>();
    }

    private void Update()
    {
        transform.position = new Vector2(Mathf.Clamp(transform.position.x, minXClamp, maxXClamp), Mathf.Clamp(transform.position.y, minYClamp, maxYClamp));
    }

    public void Move(float horizontal, float vertical)
    {
        Vector2 move = new Vector2(horizontal, vertical);
        move *= moveSpeed;

        rb.velocity = move;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Ground")
        {
            if (playerManager.isDead)
                soundManager.CmdSoundEffect("Death");
            else
                soundManager.CmdSoundEffect("Land");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Ground")
        {
            soundManager.CmdSoundEffect("TakeOff");
        }
    }
}
