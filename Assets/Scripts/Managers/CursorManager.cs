using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public Texture2D AimCursorText;
    
    // Start is called before the first frame update
    void Start()
    {
        AimCursor(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AimCursor(bool active)
    {
        if (active)
        {
            Vector2 hotSpot = new Vector2(AimCursorText.width / 2f, AimCursorText.height / 2f);
            Cursor.SetCursor(AimCursorText, hotSpot, CursorMode.ForceSoftware);
            //Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
