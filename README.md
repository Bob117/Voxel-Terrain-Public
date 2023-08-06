# Voxel-Terrain-Public

Use Unity 2021.3.5f1

Summary how it works:
1. Places chunks in a spherical pattern around the player. Each chunk is 32x32x32 blocks.
2. Each chunk generates the blocks with a cumputeshader, at this step only air and stone.
3. Once the shape of the terrain is done another compute shader calculates the type of each block.
4. A third compute shader takes the voxel data and calculates which faces of the terrain that needs to be rendered.
5. Lastly the indices of the faces are sent to a custom vertex shader that renders the wanted faces with the texture for that type of block.
