using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // פונקציה לטעינת סצנה
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
