using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class GameManager : MonoBehaviour
{

    public enum GameScene{
        MENU,
        SINGLE_PLAYER,
        LOCAL_MULTIPLAYER
    }

    //Linkeado a través del editor
    public GameObject leanTouchPrefab;


    // Singleton implementation
    public GameScene initialScene;
    GameScene currentScene;

    private static GameManager _instance;

    public static GameManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        } else {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
    //--------------------------- 

    void Start(){
        currentScene = initialScene;
        GameObject leanTouch = GameObject.FindWithTag("LeanTouch");
        if (leanTouch == null) leanTouch = Instantiate(leanTouchPrefab);
        DontDestroyOnLoad(leanTouch);

        DOTween.Init();

        SceneManager.sceneLoaded += OnSceneLoaded;
        SetupCurrentScene();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode){
        SetupCurrentScene();
    }

    void SetupCurrentScene(){
        switch(currentScene){
            case GameScene.MENU:
                
            break;
            case GameScene.SINGLE_PLAYER:
                
            break;
            case GameScene.LOCAL_MULTIPLAYER:

            break;
        }
    }

    IEnumerator LoadMenuScene(string scene){

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
