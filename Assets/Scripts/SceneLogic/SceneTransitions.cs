using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SceneTransitions : MonoBehaviour
{
    Image img;

    void Start()
    {
        img = GameObject.Find("TransitionImage").GetComponent<Image>();
        img.DOFade(0,2).SetEase(Ease.OutCubic);
    }

    // Update is called once per frames
    void Update()
    {
        
    }

    public void ChangeScene(string scene)
    {
        img.DOFade(1,2).OnComplete(()=>TransitionCompleted(scene));
    }

    void TransitionCompleted(string scene)
    {
        StartCoroutine(LoadScene(scene));
    }

    IEnumerator LoadScene(string scene){

        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

    }
}
