using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoSingleton<UIManager> {
    private GameObject currentPanel;
    private Dictionary<string, GameObject> uiPanels = new Dictionary<string, GameObject>();
    private Dictionary<string, Coroutine> loadingCoroutines = new Dictionary<string, Coroutine>();

    public void LoadPanel(string panelName, Action<GameObject> onLoaded = null) {
        if (uiPanels.ContainsKey(panelName)) {
            onLoaded?.Invoke(uiPanels[panelName]);
            return;
        }

        if (loadingCoroutines.ContainsKey(panelName)) {
            StartCoroutine(WaitForPanelLoad(panelName, onLoaded));
            return;
        }

        loadingCoroutines[panelName] = StartCoroutine(LoadPanelAsync(panelName, onLoaded));
    }

    private IEnumerator LoadPanelAsync(string panelName, Action<GameObject> onLoaded) {
        bool isLoaded = false;
        GameObject panelPrefab = null;

        ResourceManager.Instance.LoadAssetAsync<GameObject>(panelName, (asset) => {
            isLoaded = true;
            panelPrefab = asset;
        });

        yield return new WaitUntil(() => isLoaded);

        if (panelPrefab != null) {
            GameObject panelInstance = Instantiate(panelPrefab);
            panelInstance.SetActive(false);
            uiPanels[panelName] = panelInstance;
            onLoaded?.Invoke(panelInstance);
        }

        loadingCoroutines.Remove(panelName);
    }

    private IEnumerator WaitForPanelLoad(string panelName, Action<GameObject> onLoaded) {
        while (loadingCoroutines.ContainsKey(panelName)) {
            yield return null;
        }

        if (uiPanels.ContainsKey(panelName)) {
            onLoaded?.Invoke(uiPanels[panelName]);
        }
    }

    public void ShowPanel(string panelName) {
        LoadPanel(panelName, (panel) => {
            if (currentPanel != null) {
                currentPanel.SetActive(false);
            }

            currentPanel = panel;
            currentPanel.SetActive(true);
        });
    }

    public void HidePanel(string panelName) {
        if (uiPanels.ContainsKey(panelName)) {
            uiPanels[panelName].SetActive(false);
        }
    }

    public void UnloadPanel(string panelName) {
        if (uiPanels.ContainsKey(panelName)) {
            Destroy(uiPanels[panelName]);
            uiPanels.Remove(panelName);
        }
    }
}