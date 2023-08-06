using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HUDScript : MonoBehaviour
{

    public static HUDScript s_instance { get; private set; }

    public TMPro.TextMeshProUGUI m_nrOfChunksText;
    public TMPro.TextMeshProUGUI m_loadedChunksText;
    public TMPro.TextMeshProUGUI m_culledChunksText;
    public TMPro.TextMeshProUGUI m_nrOfCubesText;
    public TMPro.TextMeshProUGUI m_nrOfFacesText;
    public TMPro.TextMeshProUGUI m_culledFacesText;

    public int m_nrOfChunks;
    public int m_nrOfLoadedChunks;
    public int m_nrOfRenderedChunks;
    public int m_nrOfCubes;
    public int m_nrOfFaces;


    void Awake()
    {
        s_instance = this;
    }

    void Update()
    {
        m_nrOfChunksText.text = "#Loaded Chunks: " + m_nrOfLoadedChunks;
        m_loadedChunksText.text = "%Loaded Chunks: " + m_nrOfLoadedChunks / (float)m_nrOfChunks * 100;
        m_culledChunksText.text = "%Culled Chunks: " + (1 - (m_nrOfRenderedChunks / (float)m_nrOfLoadedChunks)) * 100;

        m_nrOfCubesText.text = "#Cubes: " + m_nrOfCubes;
        m_nrOfFacesText.text = "#Faces: " + m_nrOfFaces;
        m_culledFacesText.text = "%Culled Faces: " + (1-(m_nrOfFaces / (float)(m_nrOfCubes*6)))*100;

        m_nrOfLoadedChunks = 0;
        m_nrOfRenderedChunks = 0;
        m_nrOfCubes = 0;
        m_nrOfFaces = 0;
    }
}
