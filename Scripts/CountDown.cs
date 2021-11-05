using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountDown : MonoBehaviour
{
    public TMPro.TMP_Text text;
    public Animation animation;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetText(string t)
    {
        text.text = t;
    }
    
    public void CountDownStart()
    {
        animation.Play();
    }
}
