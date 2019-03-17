using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObjects : MonoBehaviour {

	public void ResetMapObjects() {
        gameObject.BroadcastMessage("ResetSelf");
    }
}
