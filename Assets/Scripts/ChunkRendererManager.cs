using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkRendererManager : MonoBehaviour
{
    public GameObject m_player;

    private Dictionary<int, ChunkRenderer> m_chunkRendersByPos;
    private Dictionary<int, ChunkRenderer> m_chunkRendersByID; //Never modify the data in this
    private List<int> m_outOfPlayChunkKeys;
    private static GraphicsBuffer m_faceIndexBuffer;
    private int[] m_faceIndices;


    public static ChunkRendererManager s_instance { get; private set; }


    private void Awake()
    {
        s_instance = this;

        m_chunkRendersByPos = new Dictionary<int, ChunkRenderer>();
        m_chunkRendersByID = new Dictionary<int, ChunkRenderer>();
        m_outOfPlayChunkKeys = new List<int>();
    }

    private void Start()
    {

        int faceBufferSize = ChunkConstants.CHUNK_DIMENSION * ChunkConstants.CHUNK_DIMENSION * ChunkConstants.CHUNK_DIMENSION * ChunkConstants.NR_OF_FACES_PER_CUBE;
        m_faceIndices = new int[faceBufferSize];
        m_faceIndexBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, faceBufferSize, sizeof(int));
        m_faceIndexBuffer.name = "m_faceIndexBuffer";
    }

    private void OnDestroy()
    {
        foreach (KeyValuePair<int, ChunkRenderer> chunkRenderer in m_chunkRendersByID)
        {
            chunkRenderer.Value.Destroy();
        }

        m_faceIndexBuffer.Release();
    }

    int ConvertPosToIndex(Vector3Int chunkPos)
    {
        return chunkPos.x + ChunkConstants.CHUNK_DIMENSION * (chunkPos.y + ChunkConstants.CHUNK_DIMENSION * chunkPos.z);
    }

    public ChunkRenderer GetChunkRenderer(Vector3 chunkPos)
    {
        int chunkKey = ConvertPosToIndex(Vector3Int.FloorToInt(chunkPos));

        ChunkRenderer chunkRenderer = null;

        if (m_chunkRendersByPos.TryGetValue(chunkKey, out chunkRenderer) == false)
        {
            chunkRenderer = GetOutOfPlayChunkRenderer();
            chunkRenderer.RecreateChunk(chunkPos);
            m_chunkRendersByPos.Add(chunkKey, chunkRenderer);
        }


        return chunkRenderer;
    }

    public bool DoesChunkRendererExistsForPos(Vector3 chunkPos)
    {
        int chunkKey = ConvertPosToIndex(Vector3Int.FloorToInt(chunkPos));
        return m_chunkRendersByPos.ContainsKey(chunkKey);
    }

    public ChunkRenderer CreateChunkRenderer(Vector3 pos)
    {
        int chunkIndex = ConvertPosToIndex(Vector3Int.FloorToInt(pos));

        ChunkRenderer chunkRenderer = new ChunkRenderer();
        chunkRenderer.Init(pos);

        m_chunkRendersByPos.Add(chunkIndex, chunkRenderer);
        m_chunkRendersByID.Add(chunkRenderer.GetID(), chunkRenderer);

        return chunkRenderer;
    }

    ChunkRenderer GetOutOfPlayChunkRenderer()
    {
        ChunkRenderer chunkRenderer = null;
        if (m_outOfPlayChunkKeys.Count > 0)
        {
            if(m_chunkRendersByID.TryGetValue(m_outOfPlayChunkKeys[0], out chunkRenderer))
            {
                int chunkIndex = ConvertPosToIndex(Vector3Int.FloorToInt(chunkRenderer.GetChunkPos()));

                m_chunkRendersByPos.Remove(chunkIndex);
                m_outOfPlayChunkKeys.RemoveAt(0);
            }
            else
            {
                Debug.Assert(false, "Could not find out of play chunk renderer");
            }
        }
        else
        {
            Debug.Assert(false, "There are no free chunk renderer");
        }
        return chunkRenderer;
    }

    public void RegisterChunkAsOutOfPlay(int chunkRenderID)
    {
        Debug.Log("RegisterChunkAsOutOfPlay");
        m_outOfPlayChunkKeys.Add(chunkRenderID);
    }

    public void ClearOutOfPlayChunkRendererList()
    {
        Debug.Log("ClearOutOfPlayChunkRendererList");
        m_outOfPlayChunkKeys.Clear();
    }

    public GraphicsBuffer GetTempFaceIndexBuffer()
    {
        return m_faceIndexBuffer;
    }

    public int[] GetTempFaceIndexArray()
    {
        return m_faceIndices;
    }
}

