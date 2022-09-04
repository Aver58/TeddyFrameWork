using UnityEngine;
using UnityEngine.UI;

public class HpSliderActor : MonoBehaviour {
    public Slider slider;

    public void SetValue(float value) {
        slider.value = value;
    }
}
