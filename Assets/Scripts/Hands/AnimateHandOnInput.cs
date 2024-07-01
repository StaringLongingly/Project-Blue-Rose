using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimateHandOnInput : MonoBehaviour
{
    public InputActionProperty pinchAnimationAction;
    public InputActionProperty gripAnimationAction;
    public Animator handAnimator;
    public bool grabOverride, pinchOverride;
    private bool wasGrabbingLastFrame, wasPinchingLastFrame;
    public bool isCalibrationLevel;

    void FixedUpdate()
    {
        float triggerValue = pinchAnimationAction.action.ReadValue<float>();
        handAnimator.SetFloat("Trigger", triggerValue);
        bool isPinching = triggerValue > 0.5f || pinchOverride;

        if (isCalibrationLevel)
        {
            gameObject.GetComponentInParent<UpdateHandModelTransform>().enabled = !isPinching;
            if (isPinching)
            {
                if (wasPinchingLastFrame) gameObject.GetComponent<HandCalibrator>().CalibratePositionAndRotationOffset();
                else gameObject.GetComponent<HandCalibrator>().getControllerTransformBeforePinch();
            }

            if (!isPinching && wasPinchingLastFrame) gameObject.GetComponent<HandCalibrator>().updateHandOffset();
        }


        float gripValue = gripAnimationAction.action.ReadValue<float>();
        handAnimator.SetFloat("Grip", gripValue);

        bool isGrabbing = gripValue > 0.5f || grabOverride;

        if (isCalibrationLevel && isGrabbing && !wasGrabbingLastFrame) gameObject.GetComponent<HandCalibrator>().CalibrateGrabbingDirection();
        if (!isCalibrationLevel) gameObject.GetComponentInParent<ForceGrabObject>().isGrabbing = isGrabbing;

        wasGrabbingLastFrame = isGrabbing;
        wasPinchingLastFrame = isPinching;
    }
}
