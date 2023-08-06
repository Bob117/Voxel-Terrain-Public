using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkPlacer : MonoBehaviour
{
    public GameObject m_chunkGO;
    public GameObject m_player;

    private int m_width;

    private GameObject[] m_chunks;
    private int m_nrOfChunks;

    public static ChunkPlacer s_instance { get; private set; }

    private void Awake()
    {
        s_instance = this;
    }

    void Start()
    {
        m_width = WorldSettings.s_instance.GetWorldWidth();
        m_chunks = new GameObject[m_width * m_width * m_width];

        ChunkSyncer.s_instance.Init();
        CreateChunks();
    }

    void CreateChunks()
    {
        int radius = m_width * ChunkConstants.CHUNK_DIMENSION / 2;
        Vector3Int midPoint = new Vector3Int(radius, radius, radius);

        int i = 0;
        int j = 0;
        int k = 0;

        for (float x = (-m_width * 0.5f); x < (m_width * 0.5f); x++)
        {
            j = 0;

            for (float y = (-m_width * 0.5f); y < (m_width * 0.5f); y++)
            {
                k = 0;

                for (float z = (-m_width * 0.5f); z < (m_width * 0.5f); z++)
                {


                    Vector3 pos = new Vector3(x * ChunkConstants.CHUNK_DIMENSION, y * ChunkConstants.CHUNK_DIMENSION, z * ChunkConstants.CHUNK_DIMENSION) + m_player.transform.position;
                    float distanceToMid = (pos + new Vector3(16, 16, 16) - m_player.transform.position).magnitude;
                    if (distanceToMid <= radius)
                    {


                        m_chunks[m_nrOfChunks] = Instantiate(m_chunkGO, pos, Quaternion.identity, transform);
                        m_chunks[m_nrOfChunks].SetActive(true);

                        m_chunks[m_nrOfChunks].GetComponent<Chunk>().Init(new Vector3((int)(x - m_width) + m_width, (int)(y - m_width) + m_width, (int)(z - m_width) + m_width) * ChunkConstants.CHUNK_DIMENSION);
     
                        ChunkSyncer.s_instance.AddChunk(m_chunks[m_nrOfChunks].GetComponent<Chunk>());


                        m_nrOfChunks++;

                    }

                    k++;

                }
                j++;

            }
            i++;
        }
        HUDScript.s_instance.m_nrOfChunks = m_nrOfChunks;
    }

    public int GetWorldWidth()
    {
        return m_width;
    }
}
