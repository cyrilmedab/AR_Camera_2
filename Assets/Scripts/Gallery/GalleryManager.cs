using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GalleryManager : MonoBehaviour
{
    List<string> dbImageKeys = new();

    // Start is called before the first frame update
    void Start()
    {
        GetAllKeys();
    }

    private void GetAllKeys()
    {

        //foreach (var image in FirebaseManager.Instance.userDatabaseRef.Child(FirebaseManager.Instance.dbImgName))
      
    }
}
