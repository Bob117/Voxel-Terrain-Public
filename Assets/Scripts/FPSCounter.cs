using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
    private TMPro.TextMeshProUGUI m_fpsCounterText;
    
    private int m_count;
    private int m_samples = 100;
    private float m_totalTime;


    void Start()
    {
        m_fpsCounterText = GetComponent<TMPro.TextMeshProUGUI>();
    }

    void Update()
    {
        m_count -= 1;
        m_totalTime += Time.deltaTime;

        if (m_count <= 0)
        {
            float fps = m_samples / m_totalTime;
            m_fpsCounterText.text = "FPS: " + fps;
            m_totalTime = 0f;
            m_count = m_samples;
        }
    }
}
