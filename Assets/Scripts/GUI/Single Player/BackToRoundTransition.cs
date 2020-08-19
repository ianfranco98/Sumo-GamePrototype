using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BackToRoundTransition : MonoBehaviour
{
    // Start is called before the first frame update
    Image img;

    void Start()
    {
        img = GameObject.Find("TransitionImage").GetComponent<Image>();
    }

    // Update is called once per frames
    void Update()
    {
        
    }

    public void FadeIn()
    {
        img.DOFade(0,0.4f);
    }

    public void FadeOut()
    {
        img.DOFade(1,0.4f);
    }
}
