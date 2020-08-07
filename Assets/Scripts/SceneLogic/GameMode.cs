using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMode : MonoBehaviour
{
    public GameObject leanTouchPrefab; //Prefab linkeado a través del editor

    protected InputManager inputManager;

    protected CustomCamera cam;

    protected GameObject EndGameMenu;

    protected OnGameFightLogic game;

    protected void FocusOnWinner()
    {
        inputManager.GameControllersSetActive(false);

        Transform winner = game.GetWinnerTransform();

        if (winner != null)
        {
            cam.FocusOnPlayer(winner);
        }
        else
        {
            cam.PutOnCenter();
        }
    }

    protected IEnumerator RoundEndedAnim()
    {
        FocusOnWinner();
        yield return new WaitForSeconds(1);
        cam.PutOnCenter();
    }


    // Ejecutadas por los eventos de interfaz (seteado desde el editor)
    public virtual void userAllowedToContinue()
    {
        cam.Restart();
        game.Restart();
    }

    public void BackToMenu()
    {
        StartCoroutine(LoadMenuScene("Menu"));
    }

    IEnumerator LoadMenuScene(string scene)
    {

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
