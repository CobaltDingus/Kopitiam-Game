using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] private Animator animations;

    public void trigger_transition()
    {
        if (animations != null)
        {
            animations.SetTrigger("go_to_second");
        }
    }
}
