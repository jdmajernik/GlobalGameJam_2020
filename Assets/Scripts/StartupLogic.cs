using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartupLogic : MonoBehaviour
{



    void Start()
    {
        Cursor.SetCursor(Resources.Load<Texture2D>("pointer"), new Vector2(22, 6), CursorMode.ForceSoftware);
    }
}
