using System;
using UnityEngine;

public class PuzzleTemplateItem : MonoBehaviour {
    private int index;
    public Action<int> onDrop;

    #region Public

    public void Init(int index, Action<int> onDrop) {
        this.index = index;
        this.onDrop = onDrop;
    }

    #endregion
    
    #region Private

    
    
    #endregion
}
