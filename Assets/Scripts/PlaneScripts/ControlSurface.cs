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

    void AnimationInterface.SetValue(float val) { anim.SetFloat(fname, val); }

    void AnimationInterface.SetBool(bool val) { anim.SetBool(bname, val); }
}
