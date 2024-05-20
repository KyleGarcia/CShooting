using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class ReactiveTarget : MonoBehaviour
{
    private SceneController sceneController;

    public void Start()
    {
        sceneController = FindObjectOfType<SceneController>();
    }

    public void ReactToHit()
    {
        Debug.Log("Target hit");
        StartCoroutine(Die());
    }

    private IEnumerator Die()
    {
        this.transform.Rotate(-75, 0, 0);
        yield return new WaitForSeconds(1.5f);

        if (sceneController != null)
        {
            sceneController.OnSkeetDestroyed(this.gameObject);
        }
        Destroy(this.gameObject);
    }
}
