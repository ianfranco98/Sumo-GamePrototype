using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lean.Touch;

public class SearchingDisplayBehaviour : MonoBehaviour
{
    Text infoText;//,buttonText;
    LeanSelectable confirmButtonInteraction;
    Image confirmButtonImg;

    Color green;

    bool hostFindedSignalReceived;
    bool failedSignalReceived;

    void Start()
    {
        green = new Color(100,233,126);
        infoText = GameObject.Find("SearchingText").GetComponent<Text>();
        confirmButtonImg = GameObject.Find("ConfirmButton").GetComponent<Image>();
        confirmButtonInteraction = confirmButtonImg.gameObject.GetComponent<LeanSelectable>();
        confirmButtonInteraction.enabled = false;
    }

    void Update()
    {
        if (hostFindedSignalReceived){
            confirmButtonImg.color = green;
            infoText.text = "Finded";
            confirmButtonInteraction.enabled = true;
            hostFindedSignalReceived = false;
        }

        if (failedSignalReceived){
            infoText.text = "Host not finded, trying again";
            StartCoroutine(FindingHostTryAgain());
            failedSignalReceived = false;
        }
    }

    public void HostFinded()
    {
        hostFindedSignalReceived = true;
        
    }

    public void FindingHostFailed()
    {
        failedSignalReceived = true;
    }


    public IEnumerator FindingHostTryAgain()
    {
        yield return new WaitForSeconds(2.5f);
        GameObject.Find("NetworkingManager").GetComponent<UDPClient>().StartClient();
    }

}
