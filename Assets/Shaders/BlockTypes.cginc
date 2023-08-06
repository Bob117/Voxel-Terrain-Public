static const uint BLOCK_AIR = 0;
static const uint BLOCK_TEST = 1;
static const uint BLOCK_STONE = 2;
static const uint BLOCK_DIRT = 3;
static const uint BLOCK_DIRT_GRASS = 4;

static const uint NR_OF_BLOCK_TYPES = 4; //Dont count air
static const uint NR_OF_SPRITES_IN_ATLAS = 7;

//Texture index per block type and face
static const uint CUBE_TYPE_ATLAS_INDICES[NR_OF_BLOCK_TYPES *6] = {
    2, //Test front
    0, //Test left
    0, //Test right
    1, //Test top
    1, //Test bottom
    2,  //Test back

    3, //Stone front
    3, //Stone left
    3, //Stone right
    3, //Stone top
    3, //Stone bottom
    3, //Stone back

    4, //Dirt front
    4, //Dirt left
    4, //Dirt right
    4, //Dirt top
    4, //Dirt bottom
    4,  //Dirt back

    5, //Dirt_Grass front
    5, //Dirt_Grass left
    5, //Dirt_Grass right
    6, //Dirt_Grass top
    4, //Dirt_Grass bottom
    5  //Dirt_Grass back
};