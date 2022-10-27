using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionTrigger : MonoBehaviour
{
    [SerializeField] string sceneName;
    public void Transition()
    {
        Fade.instance.FadeOutTranstion(sceneName);
    }
}
