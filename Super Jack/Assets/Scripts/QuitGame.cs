using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class QuitGame : MonoBehaviour
{
    public void EscapeGame(InputAction.CallbackContext context)
    {
        Application.Quit();
    }
}
