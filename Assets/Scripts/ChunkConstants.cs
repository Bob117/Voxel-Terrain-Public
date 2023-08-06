using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkConstants : MonoBehaviour
{

    [SerializeField] private Material m_material;
    [SerializeField] private ComputeShader m_faceGeneratorComputeShader;
    [SerializeField] private ComputeShader m_terrainGeneratorComputeShader;
    [SerializeField] private ComputeShader m_terrainPainterComputeShader;
 
    public static readonly int CHUNK_DIMENSION = 32;
    public static readonly int NR_OF_FACES_PER_CUBE = 6;
    public static readonly int NR_OF_VERTICES_PER_FACE = 6;
    public static ChunkConstants s_instance { get; private set; }

    private void Awake()
    {
        s_instance = this;
    }

    public Material GetMaterial()
    {
        return m_material;
    }

    public ComputeShader GetFaceGeneratorComputeShader()
    {
        return m_faceGeneratorComputeShader;
    }

    public ComputeShader GetTerrainGeneratorComputeShader()
    {
        return m_terrainGeneratorComputeShader;
    }

    public ComputeShader GetTerrainPainterComputeShader()
    {
        return m_terrainPainterComputeShader;
    }  
}
