using UnityEngine;
using UnityGameFramework.Runtime;

public class GameEntry : MonoBehaviour {
    public static UIComponent UI { get; private set; }
    public static DataTableComponent DataTable{ get; private set; }

    private void Start() {
        InitBuiltinComponents();
        InitCustomComponents();
    }

    private void InitBuiltinComponents() {
        UI = UnityGameFramework.Runtime.GameEntry.GetComponent<UIComponent>();
        DataTable = UnityGameFramework.Runtime.GameEntry.GetComponent<DataTableComponent>();
    }

    private void InitCustomComponents() {

    }
}
