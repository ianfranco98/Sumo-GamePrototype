using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CustomCamera : MonoBehaviour
{

    public Transform playerOne, playerTwo;

    [SerializeField]
    bool smoothEnabled;

    Vector3 originalPosition;
    Vector3 originalRotation;
    Vector3 smoothedPosition;
    Vector3 finalPosition;
    float originalZoom;


    [Range(0, 1)]
    [SerializeField]
    float smoothVel, zoomVel;

    float upperLimit, lowerLimit;
    float minSize, maxSize;
    float zoom;

    bool followModeEnabled;

    Camera mainCamera;

    void SetDefaultParams()
    {
        originalPosition = transform.position;
        minSize = 8f;
        maxSize = 11;
        smoothEnabled = true;
        smoothVel = 0.1f;
        zoomVel = 0.05f;
        originalRotation = transform.eulerAngles;
        if (mainCamera == null) mainCamera = Camera.main;
        originalZoom = mainCamera.orthographicSize;
        zoom = originalZoom;

    }

    void Start()
    {
        //SetDefaultParams();
    }

    public void Setup(Transform p1, Transform p2)
    {
        playerOne = p1;
        playerTwo = p2;
        SetDefaultParams();
    }

    public void Disconnect()
    {
        playerOne = null;
        playerTwo = null;
    }

    public void Restart()
    {
        smoothedPosition = originalPosition;
        finalPosition = originalPosition;
        transform.position = originalPosition;
        transform.eulerAngles = originalRotation;
        mainCamera.orthographicSize = originalZoom;
        zoom = originalZoom;
        followModeEnabled = false;
    }

    void FixedUpdate()
    {
        if (playerOne != null && playerTwo != null)
        {
            if (followModeEnabled)
            {
                smoothedPosition = (playerOne.position + playerTwo.position) / 2;
                float dist = Vector3.Distance(playerOne.position, playerTwo.position);
                zoom = map(dist, 4.5f, 12, minSize, maxSize);
                //Debug.Log(zoom);
                zoom = Mathf.Clamp(zoom, minSize, maxSize);
                mainCamera.orthographicSize = smoothEnabled ? Mathf.Lerp(mainCamera.orthographicSize, zoom, zoomVel) : zoom;
            }

            if (smoothEnabled)
            {
                finalPosition.x = Mathf.Lerp(transform.position.x, smoothedPosition.x, smoothVel);
                finalPosition.y = Mathf.Lerp(transform.position.y, smoothedPosition.y, smoothVel);
                finalPosition.z = Mathf.Lerp(transform.position.z, smoothedPosition.z, smoothVel);
            }
            else
            {
                finalPosition = smoothedPosition;

            }

            transform.position = finalPosition;

        }

    }

    //---------------------------------------------

    public void SwingAroundCenter()
    {
        followModeEnabled = false;
        transform.DORotate(new Vector3(0, 360, 0), 3.8f, RotateMode.FastBeyond360).SetDelay(0.1f).SetEase(Ease.InOutCubic).OnComplete(SwingAroundCenterCompleted);
        mainCamera.DOOrthoSize(zoom - 5, 2f).SetDelay(0.1f).SetEase(Ease.InOutCubic);
    }

    public void FollowPlayers()
    {
        followModeEnabled = true;
        smoothedPosition = transform.position;
        finalPosition = transform.position;
    }

    public void FocusOnPlayer(Transform playerTransform)
    {
        followModeEnabled = false;
        //transform.DOMove(playerTransform.position, focusOnPlayerDuration);//.OnComplete(FocusOnPlayerCompleted);
        smoothedPosition = playerTransform.position;
    }

    public void PutOnCenter()
    {
        transform.DOMove(Vector3.zero, 1.6f);
        smoothedPosition = Vector3.zero;
        finalPosition = Vector3.zero;
    }

    //---------------------------------------------

    void FocusOnPlayerCompleted()
    {
        //followModeEnabled = false;
        //transform.DOMove(Vector3.zero, 2).OnComplete(FocusOnPlayerCompleted);
    }

    void SwingAroundCenterCompleted()
    {
        //Ojo con este boleano, puede desfazarse con el Gamestate 
        followModeEnabled = true;
        transform.eulerAngles = Vector3.zero;
        zoom = mainCamera.orthographicSize;
    }




    float map(float value, float start1, float stop1, float start2, float stop2)
    {
        if (stop1 - start1 == 0)
        {
            Debug.LogWarning("Capo, no podés dividir por cero. Sos pelotudo?");
            return 0;
        }
        return start2 + (stop2 - start2) * ((value - start1) / (stop1 - start1));
    }



}
