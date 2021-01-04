using UnityEngine;

public class InputController : MonoBehaviour {

    private ETCInput input;

    public void Init(HeroActor PlayerActor)
    {
        input = transform.GetComponent<ETCInput>();
    }
}
