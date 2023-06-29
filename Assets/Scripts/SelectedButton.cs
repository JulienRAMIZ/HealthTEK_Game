using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectedButton : MonoBehaviour, IPointerClickHandler
{
    // public bool buttonCliked;
    public Button button1, button2, button3, button4;
    public string buttonName;
   // PointerEventData pointerEventData;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Detect if a click occurs
    public void OnPointerClick (PointerEventData pointerEventData)
    {
        Debug.Log("coucou");
        //Use this to tell when the user right-clicks on the Button
        if (pointerEventData.button == PointerEventData.InputButton.Right || pointerEventData.button == PointerEventData.InputButton.Left)
        {
            //Output to console the clicked GameObject's name and the following message. You can replace this with your own actions for when clicking the GameObject.
            Debug.Log(name + " Game Object Right Clicked!");
            //buttonCliked = true;
            buttonName = name;
         
            //button.Select();
        }
    }
}
