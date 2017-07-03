using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Projectile : NetworkBehaviour
{
    public float speed;
    public float damage;
    bool isPlayerOne;
    bool dealingDamage;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(2);
        gameObject.SetActive(false);
    }

    public void SetPlayer(bool result)
    {
        isPlayerOne = result;
    }

    private void Update()
    {
        if(isPlayerOne)
            transform.Translate(Vector2.left * speed * Time.deltaTime);
        else
            transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    [ServerCallback]
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag.Equals("Player") && !dealingDamage)
        {
            dealingDamage = true;
            PlayerWrangler.GetPlayer(collision.name).GetComponent<Health>().RpcTookDamage(damage);
        }
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(1);
        dealingDamage = false;
    }
}
