using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class SettingsPopup : MonoBehaviour {
   
    public void OnSubmitName(string name)
    {
        Debug.Log(name);
    }
    public void OnSpeedValue(float speed)
    {
        Debug.Log($"Speed: {speed}");
    }

    public void Open() {
        gameObject.SetActive(true);
    }
    public void Close() {
        gameObject.SetActive(false);
    }
}
