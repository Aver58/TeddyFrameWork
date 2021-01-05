using UnityEngine;

public class InputManager : MonoBehaviour {

    private ETCInput input;

    public void Init(HeroActor PlayerActor)
    {
        input = transform.GetComponent<ETCInput>();
    }
}
