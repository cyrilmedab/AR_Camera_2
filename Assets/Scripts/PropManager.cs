using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class PropManager : MonoBehaviour
{
    public static PropManager Instance { get; private set; }

    public static int CurrentProp;

    public HashSet<GameObject> ARFaces = new HashSet<GameObject>();

    public int PropCount;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    //private void FindFaces()
    //{
    //    ARFaces = GameObject.FindGameObjectsWithTag("AR_Face");
    //    if (ARFaces.Length == 0) return;

    //    _propCount = ARFaces[0].transform.childCount;
    //}

    public void NextProp(int direction)
    {
        SetPropInactive();

        CurrentProp += direction;
        CurrentProp = Utils.RealMod(CurrentProp, PropCount);

        SetPropActive();
    }

    private void SetPropActive()
    {
        foreach (GameObject face in ARFaces)
        {
            face.transform.GetChild(CurrentProp).gameObject.SetActive(true);
        }
    }

    private void SetPropInactive()
    {
        foreach (GameObject face in ARFaces)
        {
            face.transform.GetChild(CurrentProp).gameObject.SetActive(false);
        }
    }
}
