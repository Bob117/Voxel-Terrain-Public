using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ChunkRenderer
{
    [SerializeField] private bool m_render = true;
    [SerializeField] private int m_LOD = 0;

    private Material m_uniqueMaterial;

    [SerializeField] private GameObject m_debugSquare;

    private Bounds m_bounds;

    private GraphicsBuffer m_blockBuffer; 
    private GraphicsBuffer m_faceIndexBuffer;
    private GraphicsBuffer m_positionBuffer; 
    private GraphicsBuffer m_LODBuffer;

    private uint[] m_blocks;
    private int[] m_faceIndices;

    private int m_nrOfFaces;
    private int m_nrOfCubes;

    private static int s_IDCounter;

    private int m_uniqueID;
    private Vector3Int m_chunkPos;

    public void Init(Vector3 startPos)
    {
        m_uniqueID = s_IDCounter;
        s_IDCounter++;

        m_blocks = new uint[ChunkConstants.CHUNK_DIMENSION * ChunkConstants.CHUNK_DIMENSION * ChunkConstants.CHUNK_DIMENSION];
        m_faceIndices = new int[ChunkConstants.CHUNK_DIMENSION * ChunkConstants.CHUNK_DIMENSION * ChunkConstants.CHUNK_DIMENSION * ChunkConstants.NR_OF_FACES_PER_CUBE /2];
        m_uniqueMaterial = new Material(ChunkConstants.s_instance.GetMaterial());

        CreateGPUBuffers();
        RecreateChunk(startPos);
    }

    public void Destroy()
    {
        m_blockBuffer.Release();
        m_faceIndexBuffer.Release();
        m_positionBuffer.Release();
        m_LODBuffer.Release();
    }

    public void RecreateChunk(Vector3 newPos)
    {
        Vector3 vectorToCenter = new Vector3(16, 16, 16);
        Vector3 center = newPos + vectorToCenter;
        m_bounds = new Bounds(center, vectorToCenter*2);

        UpdateChunkPosition(newPos);
        CreateBlocks();
        CreateFaces();
    
    }

    void CreateGPUBuffers()
    {
        m_blockBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, m_blocks.Length, sizeof(uint));
        m_blockBuffer.name = "m_blockBuffer";

        m_faceIndexBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, m_faceIndices.Length, sizeof(int));
        m_faceIndexBuffer.name = "m_faceIndexBuffer";

        m_positionBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Constant, 3, sizeof(float));
        m_positionBuffer.name = "m_positionBuffer";

        m_LODBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Constant, 1, sizeof(int));
        m_LODBuffer.name = "m_LODBuffer";
    }

    void CreateBlocks()
    {
        ChunkConstants.s_instance.GetTerrainGeneratorComputeShader().SetBuffer(0, "_BlockBuffer", m_blockBuffer);
        ChunkConstants.s_instance.GetTerrainGeneratorComputeShader().SetConstantBuffer("_PositionBuffer", m_positionBuffer, 0, sizeof(float) * 4);
        ChunkConstants.s_instance.GetTerrainGeneratorComputeShader().SetConstantBuffer("_LODBuffer", m_LODBuffer, 0, sizeof(int) * 4);

        ChunkConstants.s_instance.GetTerrainGeneratorComputeShader().Dispatch(0, ChunkConstants.CHUNK_DIMENSION, 1, 1);

        ChunkConstants.s_instance.GetTerrainPainterComputeShader().SetBuffer(0, "_BlockBuffer", m_blockBuffer);
        ChunkConstants.s_instance.GetTerrainPainterComputeShader().SetConstantBuffer("_PositionBuffer", m_positionBuffer, 0, sizeof(float) * 4);

        ChunkConstants.s_instance.GetTerrainPainterComputeShader().Dispatch(0, 1, 1, 1);

        m_blockBuffer.GetData(m_blocks);

        for (int i = 0; i < m_blocks.Length; i++)
        {
            if (m_blocks[i] != 0)
            {
                HUDScript.s_instance.m_nrOfCubes++;
                m_nrOfCubes++;
            }
        }
    }

    void CreateFaces()
    {
        ChunkConstants.s_instance.GetFaceGeneratorComputeShader().SetBuffer(0, "_BlockBuffer", m_blockBuffer);
        ChunkConstants.s_instance.GetFaceGeneratorComputeShader().SetBuffer(0, "_FaceIndexBuffer", ChunkRendererManager.s_instance.GetTempFaceIndexBuffer());

        ChunkConstants.s_instance.GetFaceGeneratorComputeShader().Dispatch(0, ChunkConstants.CHUNK_DIMENSION, 1, 1);

        CompressFaceData();
    }

    void CompressFaceData()
    {
        int[] tempArray = ChunkRendererManager.s_instance.GetTempFaceIndexArray();
        ChunkRendererManager.s_instance.GetTempFaceIndexBuffer().GetData(tempArray);

        m_nrOfFaces = 0;

        for (int i = 0; i < tempArray.Length; i++)
        {
            if (tempArray[i] != -1)
            {
                m_faceIndices[m_nrOfFaces] = tempArray[i];
                m_nrOfFaces++;
            }
        }
        m_faceIndexBuffer.SetData(m_faceIndices);
    }

    void UpdateChunkPosition(Vector3 newPos)
    {
        m_chunkPos = Vector3Int.RoundToInt(newPos);
        m_positionBuffer.SetData(new float[] { newPos.x, newPos.y, newPos.z });
    }


    public void Update()
    {
        RenderChunk();
    }

    void RenderChunk()
    {
        HUDScript.s_instance.m_nrOfLoadedChunks++;

        if (m_nrOfFaces > 0)
        {
            HUDScript.s_instance.m_nrOfRenderedChunks++;
            HUDScript.s_instance.m_nrOfCubes += m_nrOfCubes;
            HUDScript.s_instance.m_nrOfFaces += m_nrOfFaces;

            m_uniqueMaterial.SetBuffer("FaceIndexBuffer", m_faceIndexBuffer);
            m_uniqueMaterial.SetBuffer("_BlockBuffer", m_blockBuffer);
            m_uniqueMaterial.SetConstantBuffer("PositionBuffer", m_positionBuffer, 0, sizeof(float) * 4);
            m_uniqueMaterial.SetConstantBuffer("LODBuffer", m_LODBuffer, 0, sizeof(int) * 4);

            Graphics.DrawProcedural(m_uniqueMaterial, m_bounds, MeshTopology.Triangles, m_nrOfFaces * ChunkConstants.NR_OF_VERTICES_PER_FACE, 1, Camera.main, null, ShadowCastingMode.On, true);
        }
    }

    public Vector3 GetChunkPos()
    {
        return m_chunkPos;
    }

    public int GetID()
    {
        return m_uniqueID;
    }
}
