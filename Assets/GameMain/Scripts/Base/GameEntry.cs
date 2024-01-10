using UnityEngine;
using UnityGameFramework.Runtime;

public class GameEntry : MonoBehaviour {
    public static UIComponent UI { get; private set; }
    public static DataTableComponent DataTable{ get; private set; }
    public static EventComponent Event{ get; private set; }

    private void Start() {
        InitBuiltinComponents();
        InitCustomComponents();
    }

    private void InitBuiltinComponents() {
        UI = UnityGameFramework.Runtime.GameEntry.GetComponent<UIComponent>();
        Event = UnityGameFramework.Runtime.GameEntry.GetComponent<EventComponent>();
        DataTable = UnityGameFramework.Runtime.GameEntry.GetComponent<DataTableComponent>();
    }

    private void InitCustomComponents() {

    }
}
