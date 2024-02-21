using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventScript : MonoBehaviour
{
    public void AnimationEnd()
    {
        Destroy(this.transform.parent.transform.parent.gameObject);
    }
}
