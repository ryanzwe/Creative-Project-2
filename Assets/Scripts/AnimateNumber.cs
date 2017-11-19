using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

class AnimateNumber : MonoBehaviour
{
    private Text animatedText;
    private float StartingNum;
    private float EndingNum;
    private float ChangeAMT;
    public void Setup(ref Text animatedText, float StartingNum, float EndingNum, float LerpTime)
    {
        this.animatedText = animatedText;
        this.StartingNum = StartingNum;
        this.EndingNum = EndingNum;
        ChangeAMT = (EndingNum - StartingNum) / LerpTime;
        StartCoroutine(AnimateNum());
    }
    public IEnumerator AnimateNum()
    {
        Debug.Log("A");
        while (StartingNum < EndingNum)
        {
            StartingNum += ChangeAMT * Time.deltaTime;
            if (StartingNum > EndingNum) StartingNum = EndingNum;
            animatedText.text = Mathf.RoundToInt(StartingNum).ToString();
            yield return null;
        }
        Destroy(this);
    }
}
