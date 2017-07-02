using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed;
    bool isPlayerOne;

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
}
