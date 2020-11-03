using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;

public class SquishySwimRandomStart : MonoBehaviour
{
    // Start is called before the first frame update
    void OnEnable()
    {
        Animator animator = GetComponent <Animator> ();
        float randomFrame = Random.Range(0f, 1f);
        for (int i = 0; i < 6; i++) {
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(i);
            animator.Play(state.fullPathHash, -1, randomFrame);
        }
    }
}
