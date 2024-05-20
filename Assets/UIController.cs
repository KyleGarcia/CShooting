using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
 
public class UIController : MonoBehaviour {
   [SerializeField] TMP_Text scoreLabel;
   [SerializeField] SettingsPopup settingsPopup;

   void Start ()
    {
        settingsPopup.Close();
    }
 
   void Update() {
      scoreLabel.text = Time.realtimeSinceStartup.ToString();
   }
   public void OnPointerDown()
    {
        Debug.Log("pointer down");
    } 
   public void OnOpenSettings() {
        Debug.Log("open settings");
        settingsPopup.Open();
   }
}
