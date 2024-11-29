using UnityEngine;
using UnityEngine.UI;

/* This script is provided only as a simple tool to preview all of the provided animations on the model */
public class Animation_Demo : MonoBehaviour
{
    public Transform corsairObject;  /* the animated aircraft 3d model */
    public float rotationSpeed = 2.0f;  /* speed at which our 3d model will rotate */
    private bool rotate = true; /* allows us to turn on and off the rotation of the 3d model */
    public Animator propAnimator; /* animator for the propeller of the aircraft */
    public Animator landingGearAnimator; /* animator for the landing gear of the aircraft */
    public Animator rearWheelAnimator; /* animator for the small rear wheel of the aircraft */
    public Animator elevatorAnimator; /* animator for the rear elevator of the aircraft */
    public Animator rudderAnimator; /* animator for the rear rudder of the aircraft */
    public Animator leftAileronAnimator; /* animator for the left aileron of the aircraft */
    public Animator rightAileronAnimator; /* animator for the right aileron of the aircraft */
    public Animator leftFlapAnimator; /* animator for the right flap of the aircraft */
    public Animator rightFlapAnimator; /* animator for the right flap of the aircraft */

    void RotateObject()
    {
        if(rotate)
            corsairObject.Rotate(0f, 10f * rotationSpeed * Time.deltaTime, 0f);
    }
    public void SetRotate(GameObject btn)
    {
        Text text = btn.GetComponentInChildren<Text>(true);

        if (!rotate)
        {
            if (text != null)
            {
                text.text = "Pause Rotation";
            }
                rotate = true;
        }
        else
        {
            if (text != null)
            {
                text.text = "Unpause Rotation";
            }
            rotate = false;
        }
    }
    public void PlayPropIdleAnimation()
    {
        if (propAnimator != null)
        {
            propAnimator.Play("Base Layer.prop_spin_slow", 0, 0);
        }
    }
    public void PlayPropFlightAnimation()
    {
        if (propAnimator != null)
        {
            propAnimator.Play("Base Layer.prop_spin_fast", 0, 0);
        }
    }
    public void PlayLandingGearUpAnimation()
    {
        if (landingGearAnimator != null)
        {
            landingGearAnimator.CrossFade("Base Layer.gear_up", 0.25f, 0, 0);
        }
    }
    public void PlayLandingGearDownAnimation()
    {
        if (landingGearAnimator != null)
        {
            landingGearAnimator.CrossFade("Base Layer.gear_down", 0.25f, 0, 0);
        }
    }
    public void PlayRearWheelUpAnimation()
    {
        if (rearWheelAnimator != null)
        {
            rearWheelAnimator.CrossFade("Base Layer.rear_wheel_up", 0.25f, 0, 0);
        }
    }
    public void PlayRearWheelDownAnimation()
    {
        if (rearWheelAnimator != null)
        {
            rearWheelAnimator.CrossFade("Base Layer.rear_wheel_down", 0.25f, 0, 0);
        }
    }
    public void PlayElevatorUpAnimation()
    {
        if (elevatorAnimator != null)
        {
            elevatorAnimator.CrossFade("Base Layer.elevator_up", 0.25f, 0, 0);
        }
    }
    public void PlayElevatorDownAnimation()
    {
        if (elevatorAnimator != null)
        {
            elevatorAnimator.CrossFade("Base Layer.elevator_down", 0.25f, 0, 0);
        }
    }
    public void PlayRudderLeftAnimation()
    {
        if (rudderAnimator != null)
        {
            rudderAnimator.CrossFade("Base Layer.rudder_left", 0.25f, 0, 0);
        }
    }
    public void PlayRudderRightAnimation()
    {
        if (rudderAnimator != null)
        {
            rudderAnimator.CrossFade("Base Layer.rudder_right", 0.25f, 0, 0);
        }
    }
    public void PlayLeftAileronUpAnimation()
    {
        if (leftAileronAnimator != null)
        {
            leftAileronAnimator.CrossFade("Base Layer.left_aileron_up", 0.25f, 0, 0);
        }
    }
    public void PlayLeftAileronDownAnimation()
    {
        if (leftAileronAnimator != null)
        {
            leftAileronAnimator.CrossFade("Base Layer.left_aileron_down", 0.25f, 0, 0);
        }
    }
    public void PlayRightAileronUpAnimation()
    {
        if (rightAileronAnimator != null)
        {
            rightAileronAnimator.CrossFade("Base Layer.right_aileron_up", 0.25f, 0, 0);
        }
    }
    public void PlayRightAileronDownAnimation()
    {
        if (rightAileronAnimator != null)
        {
            rightAileronAnimator.CrossFade("Base Layer.right_aileron_down", 0.25f, 0, 0);
        }
    }
    public void PlayRightFlapUpAnimation() 
    {
        if (rightFlapAnimator != null)
        {
            rightFlapAnimator.CrossFade("Base Layer.right_flaps_up", 0.25f, 0, 0);
        }
    }
    public void PlayRightFlapDownAnimation()
    {
        if (rightFlapAnimator != null)
        {
            rightFlapAnimator.CrossFade("Base Layer.right_flaps_down", 0.25f, 0, 0);
        }
    }
    public void PlayLeftFlapUpAnimation()
    {
        if (leftFlapAnimator != null)
        {
            leftFlapAnimator.CrossFade("Base Layer.left_flaps_up", 0.25f, 0, 0);
        }
    }
    public void PlayLeftFlapDownAnimation()
    {
        if (leftFlapAnimator != null)
        {
            leftFlapAnimator.CrossFade("Base Layer.left_flaps_down", 0.25f, 0, 0);
        }
    }

    void Update()
    {
        RotateObject();
    }

    /* method to return all of our animations back to a default state */
    public void ResetAnimations()
    {
        var allAnims = FindObjectsOfType<Animator>();

        foreach (var anim in allAnims)
        {
            anim.CrossFade("Base Layer.idle", 0.25f, 0, 0);
        }
    }
}
