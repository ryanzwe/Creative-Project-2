using System;
using UnityEngine;
using UnityEngine.UI;

namespace UnityStandardAssets.Utility
{
    [RequireComponent(typeof(Text))]
    public class FPSCounter : MonoBehaviour
    {
        private Text m_Text;
        private float deltaTime;


        private void Start()
        {
            m_Text = GetComponent<Text>();
        }


        private void Update()
        {
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
            float msec = deltaTime * 1000.0f;
            float fps = 1.0f / deltaTime;
            // measure average frames per second
            m_Text.text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        }
    }
}
