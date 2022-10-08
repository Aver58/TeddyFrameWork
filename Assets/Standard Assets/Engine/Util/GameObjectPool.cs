using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class GameObjectPool {
    private static readonly Vector3 hidePoint = new Vector3(-5000, 0, 0);
    private static GameObject root;
    private const string GameObjectPoolRootName = "GameObjectPool";

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

        CreateRoot();
    }
    
    public void GetAsync(string assetName, Action<GameObject> callback) {
        GameObject instance = null;
        if (freeGameObjects.Count == 0) {
            LoadModule.LoadAssetAsync(bundleName + assetName + LoadModule.GetAssetPostfix(typeof(GameObject)), 
                typeof(GameObject), delegate(AssetRequest request) {
                    if (request.asset != null) {
                        instance = Object.Instantiate(request.asset as GameObject, root.transform);
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
            instance.transform.SetParent(root.transform);
            instance.SetActive(false);
        }
        freeGameObjects.Add(instance);
    }
    
    private static void CreateRoot() {
        if (root == null) {
            root = GameObject.Find(GameObjectPoolRootName);
            if (root == null) {
                root = new GameObject("GameObjectPool");
                root.transform.position = hidePoint;
            }
        }
    }
}