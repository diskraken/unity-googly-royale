using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceButton : MonoBehaviour {

    private int _id = 0;

    public void Init(int id) {
        _id = id;
    }

    public void OnClick() {
        GameManager.Instance.DiceButtonOnClick(_id);
    }
}
