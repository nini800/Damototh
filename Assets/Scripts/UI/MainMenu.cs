using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGameButton()
    {
        StartCoroutine("StartGame");
    }

    IEnumerator StartGame()
    {
        AsyncOperation async =  SceneManager.LoadSceneAsync(1);

        while (!async.isDone)
        {
             yield return new WaitForEndOfFrame();
        }
    }
}
