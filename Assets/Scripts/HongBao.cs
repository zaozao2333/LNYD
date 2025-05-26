using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HongBao : MonoBehaviour
{
    BoundsCheck boundsCheck;
    // Start is called before the first frame update
    void Awake()
    {
        boundsCheck = GetComponent<BoundsCheck>();

        Collider2D objectCollider = GetComponent<Collider2D>();
        Collider2D firstGroundCollider = GameObject.Find("Ground/Ground_1").GetComponent<Collider2D>();
        print(firstGroundCollider);
        Physics2D.IgnoreCollision(objectCollider, firstGroundCollider, true);

    }

    // Update is called once per frame
    void Update()
    {
        if (boundsCheck != null)
        {
            if (!boundsCheck.isOnScreen && !boundsCheck.keepOnScreen)
            {
                Destroy(gameObject);
            }
        }
    }
}
