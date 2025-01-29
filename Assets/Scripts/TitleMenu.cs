using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleMenu : MonoBehaviour
{
    /// <summary>
    /// When the user presses the 'Start Game' button,
    /// we need to load the MainGame scene.
    /// </summary>
    public void onPressStartGameBtn()
    {
        SceneManager.LoadScene("SampleScene");
    }
}