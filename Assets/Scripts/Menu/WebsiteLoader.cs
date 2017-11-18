using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebsiteLoader : MonoBehaviour
{

    public void LoadSite(string websiteName = "http://")
    {
        Application.OpenURL("http://" + websiteName);
    }
}
