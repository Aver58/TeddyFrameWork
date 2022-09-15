using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class GameObjectPool {
    private string bundleName;
    private string assetName;

    private List<GameObject> activeGameObjects;
    private List<GameObject> freeGameObjects;

    public GameObjectPool(string bundleName) {
        Init(bundleName);
    }
    
    private void Init(string bundleName) {
        this.bundleName = bundleName;
        
        activeGameObjects = new List<GameObject>();
        freeGameObjects = new List<GameObject>();
    }
    
    public void GetAsync(string assetName, Action<GameObject> callback) {
        GameObject instance = null;
        if (freeGameObjects.Count == 0) {
            LoadModule.LoadAssetAsync(bundleName + assetName + LoadModule.GetAssetPostfix(typeof(GameObject)), 
                typeof(GameObject), delegate(AssetRequest request) {
                    if (request.asset != null) {
                        instance = Object.Instantiate(request.asset as GameObject, UIModule.GameObjectPoolRoot.transform);
                        activeGameObjects.Add(instance);
                        
                        callback?.Invoke(instance);
                    } else {
                        callback?.Invoke(null);
                    }
                });
        } else {
            var index = freeGameObjects.Count - 1;
            instance = freeGameObjects[index];
            freeGameObjects.RemoveAt(index);
            activeGameObjects.Add(instance);
            
            callback?.Invoke(instance);
        }
    }

    public void Release(GameObject instance) {
        if (instance == null) {
            return;
        }
        
        if (activeGameObjects.Contains(instance)) {
            instance.transform.SetParent(UIModule.GameObjectPoolRoot.transform);
            instance.SetActive(false);
        }
        freeGameObjects.Add(instance);
    }
}