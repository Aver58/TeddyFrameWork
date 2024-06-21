using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ResourceManager : Singleton<ResourceManager> {
    private Dictionary<string, AsyncOperationHandle> loadedAssets = new Dictionary<string, AsyncOperationHandle>();

    public void LoadResourceAsync<T>(string address, Action<T> onSuccess, Action<string> onFailure = null) where T : UnityEngine.Object {
        if (loadedAssets.ContainsKey(address)) {
            onSuccess?.Invoke(loadedAssets[address].Result as T);
            return;
        }

        var handle = Addressables.LoadAssetAsync<T>(address);
        handle.Completed += (operation) => {
            if (operation.Status == AsyncOperationStatus.Succeeded) {
                loadedAssets[address] = operation;
                onSuccess?.Invoke(operation.Result);
            } else {
                onFailure?.Invoke($"Failed to load asset at {address}");
            }
        };
    }

    public void UnloadResource(string address) {
        if (loadedAssets.ContainsKey(address)) {
            Addressables.Release(loadedAssets[address]);
            loadedAssets.Remove(address);
        }
    }

    public void InstantiateResourceAsync(string address, Vector3 position, Quaternion rotation, Action<GameObject> onSuccess, Action<string> onFailure = null) {
        LoadResourceAsync<GameObject>(address, prefab => {
            if (prefab != null) {
                var instance = Instantiate(prefab, position, rotation);
                onSuccess?.Invoke(instance);
            } else {
                onFailure?.Invoke($"Failed to instantiate asset at {address}");
            }
        }, onFailure);
    }

    public void ReleaseInstance(GameObject instance) {
        Addressables.ReleaseInstance(instance);
    }
}