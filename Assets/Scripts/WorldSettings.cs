using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSettings : MonoBehaviour
{
   [SerializeField] private int m_worldWidth = 3;
   [SerializeField] private int m_nrOfChunksToLoadEachTick = 1;

    public static WorldSettings s_instance { get; private set; }

    private void Awake()
    {
        s_instance = this;
    }

    public int GetWorldWidth()
    {
        return m_worldWidth;
    }

    public int GetNrOfChunksToLoadEachTick()
    {
        return m_nrOfChunksToLoadEachTick;
    }

}
