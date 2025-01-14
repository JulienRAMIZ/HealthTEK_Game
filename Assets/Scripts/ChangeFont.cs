using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class ChangeFont : MonoBehaviour
{
    public TMP_FontAsset tahomaFont;
    private TextMeshPro[] txtMP;
    // Start is called before the first frame update
    void Start()
    {
        GetAllTextMeshPro();

    }

    // Update is called once per frame
    void Update()
    {
    }

    void GetAllObjectsOnlyInScene()
    {
        //List<TMP_FontAsset> objectsInScene = new List<TMP_FontAsset>();

        //foreach (TMP_FontAsset go in Resources.FindObjectsOfTypeAll(typeof(TMP_FontAsset)) as TMP_FontAsset[])
        //{
        //    //if (!EditorUtility.IsPersistent(go.transform.root.gameObject) && !(go.hideFlags == HideFlags.NotEditable || go.hideFlags == HideFlags.HideAndDontSave))
        //    //    objectsInScene.Add(go);
        //    FontStyle style = go.GetComponent<FontStyle>();
        //    TMP_FontAsset = tahomaFont;
        //}
        //objectsInScene = tahomaFont;
        

        

    }

    void GetAllTextMeshPro()
    {
        //txtMP = Resources.FindObjectsOfTypeAll(typeof(TextMeshPro));

        var object2TMP = Resources.FindObjectsOfTypeAll(typeof(TextMeshPro));
        //txtMP = object2TMP;

        for (int i = 0; i < txtMP.Length; i++)
        {
            txtMP[i].font = tahomaFont;
        }

        
    }
}
