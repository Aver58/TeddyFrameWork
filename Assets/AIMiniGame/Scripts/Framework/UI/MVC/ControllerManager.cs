using System.Collections.Generic;
using AIMiniGame.Scripts.Framework.UI;

public class ControllerManager : Singleton<ControllerManager> {
    private List<ControllerBase> controllers = new ();
    public void OpenAsync<T>() where T : ControllerBase, new(){
        var controller = Get<T>();
        if (controller != null && !controller.IsOpen) {
            controller.OpenAsync();
        }
    }

    public T Get<T>() where T : ControllerBase, new() {
        var controller = Find<T>();
        if (controller == null) {
            controller = new T();
            var typeName = typeof(T).Name;
            var functionName = "";
            if (typeName.EndsWith("Controller")) {
                functionName = typeName.Remove(typeName.Length - 10);
            }

            controller.Init(functionName);
            controllers.Add(controller);
        }

        return controller;
    }

    public T Find<T>() where T : ControllerBase, new() {
        for (var i = 0; i < controllers.Count; i++) {
            var controller = controllers[i];
            if (controller.IsClear) {
                continue;
            }

            if (controller is T) {
                return controller as T;
            }
        }

        return null;
    }

    public void Close<T>() where T : ControllerBase, new() {
        var controller = Get<T>();
        if (controller != null) {
            controller.Close();
        }
    }
}