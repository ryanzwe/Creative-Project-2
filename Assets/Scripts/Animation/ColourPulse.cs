using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class ColourPulse : MonoBehaviour
{
    public bool Pulse = true;
    public bool ScrollUV = false;
    public float ScrollUVSpeed = 0.15f;
    public Color[] Colours = new Color[2];
    private Image pulseImage;
    private RawImage ScrollImage;

    private void Start()
    {
        if (Pulse)
        {
            pulseImage = GetComponent<Image>();
            Material m = Instantiate(pulseImage.material);
            pulseImage.material = m;
            // StartCoroutine(PulseAnim());
        }
        if (ScrollUV)
        {
            ScrollImage = GetComponent<RawImage>();
            StartCoroutine(ScrollingUVs());
        }
    }

    private void Update()
    {
        if (Pulse)
        {
            if (t < 0 || t > 1)
            {
                i = !i;
                //Debug.Log("Inverted");

            }
            t += i ? 1 * Time.deltaTime : -1 * Time.deltaTime;
            pulseImage.material.color = Color.Lerp(Colours[0], Colours[1], t);
        }
    }

    public bool i = true;
    public float t;
    //private IEnumerator PulseAnim()
    ////{
    //Material m = Instantiate(pulseImage.material);
    //pulseImage.material = m;
    //    while (true)
    //    {
    //        if (t < 0 || t > 1)
    //        {
    //            i = !i;
    //            Debug.Log("Inverted");
                
    //        }
    //        t += i ? 0.1f * Time.deltaTime : -0.1f * Time.deltaTime;
    //        pulseImage.material.color = Color.Lerp(Colours[0], Colours[1], t);
    //        yield return null;
    //    }
    //}

    private IEnumerator ScrollingUVs()
    {
        while (true)
        {
            //Debug.Log("Scrolling");
           ScrollImage.uvRect = new Rect(Time.time * ScrollUVSpeed,0,1,1);
            yield return null;
        }
    }
}
