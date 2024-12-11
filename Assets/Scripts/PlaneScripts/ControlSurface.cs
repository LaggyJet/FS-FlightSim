using UnityEngine;

public class ControlSurface : MonoBehaviour, AnimationInterface
{
    Animator anim;
    [Tooltip("Relevant Animation Variable")]
    [SerializeField] string fname = string.Empty;
    [SerializeField] string bname = string.Empty;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
     anim = this.GetComponent<Animator>();
    }

    void AnimationInterface.SetValue(float val, bool large) { anim.SetFloat(fname, map(val, large)); }

    void AnimationInterface.SetBool(bool val) { anim.SetBool(bname, val); }

    float map(float s, bool large)
    {
        if (large) { return 0 + (s - 0) * (1 - 0) / (100 - 0); }
        else { return 0 + (s - -1) * (1 - 0) / (1 - -1); }
    }
}
