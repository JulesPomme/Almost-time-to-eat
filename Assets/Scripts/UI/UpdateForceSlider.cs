using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateForceSlider : MonoBehaviour
{
    public FloatSO throwingForce;
    public FloatSO throwingForceMin;
    public FloatSO throwingForceMax;
    public RectTransform sliderFront;

    private float maxValue;

    void Start() {
        maxValue = ((RectTransform)transform).sizeDelta.y;
    }

    void Update() {
        float force = throwingForce.value;
        //       (b - a)(x - min)
        //f(x) =  --------------    +a
        //          max - min
        float sliderTopValue = (maxValue) * (force - throwingForceMax.value) / (throwingForceMin.value - throwingForceMax.value);
        sliderFront.offsetMax = new Vector2(sliderFront.offsetMax.x, -sliderTopValue);
    }
}
