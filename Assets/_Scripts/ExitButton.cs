using UnityEngine;
using System.Collections;

public class ExitButton : MonoBehaviour
{

    /// <summary>
    /// When called exits application.
    /// (Post Build only)
    /// </summary>
   public void onButton()
    {
        Application.Quit();
    }

}
