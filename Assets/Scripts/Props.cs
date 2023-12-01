using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Props : MonoBehaviour
{
    private void Awake()
    {
        this.transform.GetChild(PropManager.CurrentProp).gameObject.SetActive(true);
        PropManager.Instance.ARFaces.Add(this.gameObject);
        PropManager.Instance.PropCount = this.transform.childCount;
    }

    private void OnDestroy() => PropManager.Instance.ARFaces.Remove(this.gameObject);

}
