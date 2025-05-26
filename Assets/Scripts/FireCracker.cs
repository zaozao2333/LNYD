using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireCracker : MonoBehaviour
{
    BoundsCheck boundsCheck;
    // Start is called before the first frame update
    void Start()
    {
        boundsCheck = GetComponent<BoundsCheck>();
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
