using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Chunk : MonoBehaviour
{
    ChunkRenderer m_chunkRenderer;

    Vector3 m_relativePos;

    public void Init(Vector3 pos)
    {
        m_chunkRenderer = ChunkRendererManager.s_instance.CreateChunkRenderer(pos);
        m_relativePos = pos;
    }

    public ChunkRenderer GetChunkRenderer()
    {
        return m_chunkRenderer;
    }

    public void SyncPos(Vector3Int syncPos)
    {
        transform.position = syncPos + m_relativePos;

    }

    public void SyncRender()
    {
        m_chunkRenderer = ChunkRendererManager.s_instance.GetChunkRenderer(transform.position);
    }

    public void SetOutOfPlay()
    {
         m_chunkRenderer = null;
    }

    private void Update()
    {
        if( m_chunkRenderer != null)
        {
            m_chunkRenderer.Update();
        }
    }
}
