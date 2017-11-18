using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallGameEnd : MonoBehaviour
{

    public void CallEnd()
    {
        GameController.Instance.EndGame(true, false);
    }
}
