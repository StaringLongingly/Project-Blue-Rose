using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightGameObject : MonoBehaviour
{
    public bool isHighlighted, isHighlightedFromLeftHand, isHighlightedFromRightHand;
    public float highlightState = 0f;
    private float highlightStatePreviousFrame;
    public float highlightRateUp = 0.1f;
    public float highlightRateDown = 0.2f;
    private MaterialPropertyBlock propertyBlock;
    private Renderer renderer;

    void Start()
    {
        renderer = gameObject.GetComponent<Renderer>();
        propertyBlock = new MaterialPropertyBlock();
        propertyBlock.SetFloat("_Highlight", isHighlighted ? 1 : 0);
        renderer.SetPropertyBlock(propertyBlock);
    }

    void FixedUpdate()
    {
        isHighlighted = isHighlightedFromLeftHand || isHighlightedFromRightHand;
        if ((highlightState == 0 && !isHighlighted) || (highlightState == 1 && isHighlighted)) return;

        if (isHighlighted) highlightState += highlightRateUp;
        else highlightState -= highlightRateDown;

        highlightState = Mathf.Clamp(highlightState, 0f, 1f);

        if (Mathf.Abs(highlightStatePreviousFrame - highlightState) > Mathf.Epsilon)
        {
            //Debug.Log("Updated Highlight", gameObject);
            propertyBlock.SetFloat("_Highlight", Mathf.Sqrt(highlightState));
            renderer.SetPropertyBlock(propertyBlock);
        }

        highlightStatePreviousFrame = highlightState;
    }
}