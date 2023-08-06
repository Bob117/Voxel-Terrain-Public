using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkSyncer : MonoBehaviour
{
    public GameObject m_player;

    private Vector3Int m_currentPlayerChunkPos;
    private Vector3Int m_lastPlayerChunkPos;


    private Chunk[] m_chunks;
    private int m_nrOfChunks;
    private List<ChunkSortingData> m_outOfPlayChunkIDs;


    public static ChunkSyncer s_instance { get; private set; }

    struct ChunkSortingData
    {
        public int chunkIndex;
        public float distanceToPlayer;
    }

    private void Awake()
    {
        s_instance = this;
    }

    public void Init()
    {
        int width = WorldSettings.s_instance.GetWorldWidth();
        m_chunks = new Chunk[width * width * width];
        m_outOfPlayChunkIDs = new List<ChunkSortingData>();
        m_nrOfChunks = 0;
    }

    public Vector3Int WorldPosToChunkPos(Vector3 pos)
    {    
        pos.x = pos.x / ChunkConstants.CHUNK_DIMENSION;
        pos.y = pos.y / ChunkConstants.CHUNK_DIMENSION;
        pos.z = pos.z / ChunkConstants.CHUNK_DIMENSION;

        Vector3Int chunk3DID = Vector3Int.FloorToInt(pos);

        return chunk3DID;
    }

    void Update()
    {
        if(m_outOfPlayChunkIDs.Count == 0) //A temporary limitation, will be removed once a related bug is fixed
        {
            Vector3Int playerChunkPos = WorldPosToChunkPos(m_player.transform.position);

            if (m_currentPlayerChunkPos != playerChunkPos)
            {
                m_currentPlayerChunkPos = playerChunkPos;

                m_outOfPlayChunkIDs.Clear();

                FindAllOutOfPlayChunksRenderers();
                SyncChunks(playerChunkPos * ChunkConstants.CHUNK_DIMENSION);

                //Debug.Log("Player is in chunk: " + playerChunkPos);
                m_lastPlayerChunkPos = m_currentPlayerChunkPos;
            }
        }

        SyncOutOfPlayChunk();
    }

    public void AddChunk(Chunk chunk)
    {
        m_chunks[m_nrOfChunks] = chunk;
        m_nrOfChunks++;
    }

    void SyncChunks(Vector3Int syncPos)
    {
        for (int i = 0; i < m_nrOfChunks; i++)
        {
            m_chunks[i].SyncPos(syncPos);
            if (ChunkRendererManager.s_instance.DoesChunkRendererExistsForPos(m_chunks[i].transform.position))
            {
                m_chunks[i].SyncRender();
            }
            else
            {
                Vector3 chunkPos = m_chunks[i].transform.position;
                Vector3 playerChunkPos = WorldPosToChunkPos(m_player.transform.position) * ChunkConstants.CHUNK_DIMENSION;

                float distanceToPlayer = (chunkPos - playerChunkPos).magnitude;

                ChunkSortingData chunkSortingData = new ChunkSortingData();
                chunkSortingData.chunkIndex = i;
                chunkSortingData.distanceToPlayer = distanceToPlayer;
                m_outOfPlayChunkIDs.Add(chunkSortingData);
                m_chunks[i].SetOutOfPlay();
            }
        }

        m_outOfPlayChunkIDs.Sort(SortChunksByDistance);
    }

    void SyncOutOfPlayChunk()
    {
        for (int i = 0; i < WorldSettings.s_instance.GetNrOfChunksToLoadEachTick(); i++)
        {
            if (m_outOfPlayChunkIDs.Count > 0)
            {
                m_chunks[m_outOfPlayChunkIDs[0].chunkIndex].SyncRender();
                m_outOfPlayChunkIDs.RemoveAt(0);
            }
        } 
    }

    static int SortChunksByDistance(ChunkSortingData c1, ChunkSortingData c2)
    {
        return c1.distanceToPlayer.CompareTo(c2.distanceToPlayer);
    }

    public void FindAllOutOfPlayChunksRenderers()
    {
        ChunkRendererManager.s_instance.ClearOutOfPlayChunkRendererList();

        int radius = WorldSettings.s_instance.GetWorldWidth() * ChunkConstants.CHUNK_DIMENSION / 2;

        for (int i = 0; i < m_nrOfChunks; i++)
        {
            Vector3 chunkPos = m_chunks[i].transform.position;
            Vector3 playerChunkPos = WorldPosToChunkPos(m_player.transform.position) * ChunkConstants.CHUNK_DIMENSION;

            float distanceToPlayer = (chunkPos - playerChunkPos).magnitude;

            if (distanceToPlayer >= radius && m_chunks[i].GetChunkRenderer() != null)
            {
                ChunkRendererManager.s_instance.RegisterChunkAsOutOfPlay(m_chunks[i].GetChunkRenderer().GetID());
            }
        }
    }
}
