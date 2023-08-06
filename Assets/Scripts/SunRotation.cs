using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunRotation : MonoBehaviour
{
    [SerializeField] private float m_rotationSpeed = 0.5f;
    private float m_currentIntesity;
    private Vector3 m_currentRotation;
    private Light m_sun;

    void Start()
    {
        m_sun = transform.GetComponent<Light>();
    }

    void Update()
    {
        transform.Rotate(new Vector3(m_rotationSpeed * Time.deltaTime, m_rotationSpeed * Time.deltaTime, 0));
        m_currentRotation = transform.rotation.eulerAngles;
        if(m_currentRotation.x > 0 && m_currentRotation.x < 180)
        {
            m_currentIntesity = Mathf.Min(m_currentRotation.x / 5.0f, 1);
        }
        else
        {
            m_currentIntesity = 0;
        }

        m_sun.intensity = m_currentIntesity;

    }
}
