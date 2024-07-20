using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayView : ViewBase, IView {

    [Header("GameplayView")]
    [SerializeField]
    private GameObject dicePrefab;

    protected override void OnAwake() {
        base.OnAwake();
    }
    public void OnClickBack() {
        GameManager.Instance.Back();
    }
}
