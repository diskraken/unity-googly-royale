using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoresView : ViewBase, IView {

    [Header("ScoresView")]
    [SerializeField] private Transform menuContainer;


    public void OnClickBack() {
        GameManager.Instance.Back();
    }
}