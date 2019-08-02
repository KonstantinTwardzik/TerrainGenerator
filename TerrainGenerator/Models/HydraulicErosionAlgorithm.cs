using System;

namespace Topographer3D.Models
{
    public class HydraulicErosionAlgorithm
    {
        private int seed;
        private int erosionRadius = 2;
        private float inertia = 0.025f; // At zero, water will instantly change direction to flow downhill. At 1, water will never change direction. 
        private float sedimentCapacityFactor = 5; // Multiplier for how much sediment a droplet can carry
        private float minSedimentCapacity = 0.001f; // Used to prevent carry capacity getting too close to zero on flatter terrain
        private float erodeSpeed = 0.15f;
        private float depositSpeed = 0.15f;
        private float evaporateSpeed = 0.005f;
        private float gravity = 2;
        private int maxDropletLifetime = 15;
        private float[] map;
        private int mapSize;

        private float initialWaterVolume = 1;
        private float initialSpeed = 1;

        // Indices and weights of erosion brush precomputed for every node
        private int[][] erosionBrushIndices;
        private float[][] erosionBrushWeights;
        private Random prng;

        private int currentSeed;
        private int currentErosionRadius;
        private int currentMapSize;

        // Initialization creates a System.Random object and precomputes indices and weights of erosion brush
        internal void Initialize(float[] map, int mapSize, bool resetSeed)
        {
            this.mapSize = mapSize;
            this.map = map;
            if (resetSeed || prng == null || currentSeed != seed)
            {
                prng = new System.Random(seed);
                currentSeed = seed;
            }

            if (erosionBrushIndices == null || currentErosionRadius != erosionRadius || currentMapSize != mapSize)
            {
                InitializeBrushIndices(mapSize, erosionRadius);
                currentErosionRadius = erosionRadius;
                currentMapSize = mapSize;
            }
        }

        internal void UpdateValues(int seed, int erosionRadius, float inertia, float sedimentCapacityFactor, float minSedimentCapacity, float erodeSpeed, float depositSpeed, float evaporateSpeed, float gravity, int maxDropletLifetime)
        {
            this.seed = seed;
            this.erosionRadius = erosionRadius;
            this.inertia = inertia;
            this.sedimentCapacityFactor = sedimentCapacityFactor;
            this.minSedimentCapacity = minSedimentCapacity;
            this.erodeSpeed = erodeSpeed;
            this.depositSpeed = depositSpeed;
            this.evaporateSpeed = evaporateSpeed;
            this.gravity = gravity;
            this.maxDropletLifetime = maxDropletLifetime;
        }

        internal void Erode()
        {


            //for (int iteration = 0; iteration < numIterations; iteration++)
            //{
            // Create water droplet at random point on map
            float posX = prng.Next(0, mapSize - 1);
            float posZ = prng.Next(0, mapSize - 1);
            float dirX = 0;
            float dirY = 0;
            float speed = initialSpeed;
            float water = initialWaterVolume;
            float sediment = 0;

            for (int lifetime = 0; lifetime < maxDropletLifetime; lifetime++)
            {
                int nodeX = Convert.ToInt32(posX);
                int nodeY = Convert.ToInt32(posZ);
                int dropletIndex = nodeY * mapSize + nodeX;
                // Calculate droplet's offset inside the cell (0,0) = at NW node, (1,1) = at SE node
                float cellOffsetX = posX - nodeX;
                float cellOffsetY = posZ - nodeY;


                // Calculate droplet's height and direction of flow with bilinear interpolation of surrounding heights
                HeightAndGradient heightAndGradient = CalculateHeightAndGradient(map, mapSize, posX, posZ);


                // Update the droplet's direction and position (move position 1 unit regardless of speed)
                dirX = (dirX * inertia - heightAndGradient.gradientX * (1 - inertia));
                dirY = (dirY * inertia - heightAndGradient.gradientY * (1 - inertia));

                // Normalize direction
                float len = (float)Math.Sqrt(dirX * dirX + dirY * dirY);
                if (len != 0)
                {
                    dirX /= len;
                    dirY /= len;
                }

                posX += dirX;
                posZ += dirY;

                // Stop simulating droplet if it's not moving or has flowed over edge of map
                if ((dirX == 0 && dirY == 0) || posX < 0 || posX >= mapSize - 1 || posZ < 0 || posZ >= mapSize - 1)
                {
                    break;
                }

                // Find the droplet's new height and calculate the deltaHeight
                float newHeight = CalculateHeightAndGradient(map, mapSize, posX, posZ).height;
                float deltaHeight = newHeight - heightAndGradient.height;

                // Calculate the droplet's sediment capacity (higher when moving fast down a slope and contains lots of water)
                float sedimentCapacity = Math.Max(-deltaHeight * speed * water * sedimentCapacityFactor, minSedimentCapacity);

                // If carrying more sediment than capacity, or if flowing uphill:
                if (sediment > sedimentCapacity || deltaHeight > 0)
                {
                    // If moving uphill (deltaHeight > 0) try fill up to the current height, otherwise deposit a fraction of the excess sediment
                    float amountToDeposit = (deltaHeight > 0) ? Math.Min(deltaHeight, sediment) : (sediment - sedimentCapacity) * depositSpeed;
                    sediment -= amountToDeposit;

                    // Add the sediment to the four nodes of the current cell using bilinear interpolation
                    // Deposition is not distributed over a radius (like erosion) so that it can fill small pits
                    if ((dropletIndex + mapSize + 1) < map.Length)
                    {
                        map[dropletIndex] += amountToDeposit * (1 - cellOffsetX) * (1 - cellOffsetY);
                        map[dropletIndex + 1] += amountToDeposit * cellOffsetX * (1 - cellOffsetY);
                        map[dropletIndex + mapSize] += amountToDeposit * (1 - cellOffsetX) * cellOffsetY;
                        map[dropletIndex + mapSize + 1] += amountToDeposit * cellOffsetX * cellOffsetY;
                    }
                }
                else
                {
                    // Erode a fraction of the droplet's current carry capacity.
                    // Clamp the erosion to the change in height so that it doesn't dig a hole in the terrain behind the droplet
                    float amountToErode = Math.Min((sedimentCapacity - sediment) * erodeSpeed, -deltaHeight);

                    // Use erosion brush to erode from all nodes inside the droplet's erosion radius
                    for (int brushPointIndex = 0; brushPointIndex < erosionBrushIndices[dropletIndex].Length; brushPointIndex++)
                    {
                        int nodeIndex = erosionBrushIndices[dropletIndex][brushPointIndex];
                        float weighedErodeAmount = amountToErode * erosionBrushWeights[dropletIndex][brushPointIndex];
                        float deltaSediment = (map[nodeIndex] < weighedErodeAmount) ? map[nodeIndex] : weighedErodeAmount;
                        map[nodeIndex] -= deltaSediment;
                        sediment += deltaSediment;
                    }
                }

                // Update droplet's speed and water content
                if ((speed * speed + deltaHeight * gravity) < 0)
                {
                    speed = 0;
                }
                else
                {
                    speed = (float)Math.Sqrt(speed * speed + deltaHeight * gravity);
                }

                water *= (1 - evaporateSpeed);


            }


            //}
        }

        private HeightAndGradient CalculateHeightAndGradient(float[] nodes, int mapSize, float posX, float posZ)
        {
            int coordX = (int)posX;
            int coordZ = (int)posZ;

            // Calculate droplet's offset inside the cell (0,0) = at NW node, (1,1) = at SE node
            float x = posX - coordX;
            float y = posZ - coordZ;

            // Calculate heights of the four nodes of the droplet's cell
            int nodeIndexNW = coordZ * mapSize + coordX;
            float heightNE = nodes[nodeIndexNW + 1];
            float heightSW = nodes[nodeIndexNW + mapSize];
            float heightNW = nodes[nodeIndexNW];
            float heightSE = nodes[nodeIndexNW + mapSize + 1];

            // Calculate droplet's direction of flow with bilinear interpolation of height difference along the edges

            float gradientX = (heightNE - heightNW) * (1 - y) + (heightSE - heightSW) * y;
            float gradientY = (heightSW - heightNW) * (1 - x) + (heightSE - heightNE) * x;


            // Calculate height with bilinear interpolation of the heights of the nodes of the cell
            float height = heightNW * (1 - x) * (1 - y) + heightNE * x * (1 - y) + heightSW * (1 - x) * y + heightSE * x * y;


            return new HeightAndGradient() { height = height, gradientX = gradientX, gradientY = gradientY };
        }

        private void InitializeBrushIndices(int mapSize, int radius)
        {
            erosionBrushIndices = new int[mapSize * mapSize][];
            erosionBrushWeights = new float[mapSize * mapSize][];

            int[] xOffsets = new int[radius * radius * 4];
            int[] yOffsets = new int[radius * radius * 4];
            float[] weights = new float[radius * radius * 4];
            float weightSum = 0;
            int addIndex = 0;

            for (int i = 0; i < erosionBrushIndices.GetLength(0); i++)
            {
                int centreX = i % mapSize;
                int centreY = i / mapSize;

                if (centreY <= radius || centreY >= mapSize - radius || centreX <= radius + 1 || centreX >= mapSize - radius)
                {
                    weightSum = 0;
                    addIndex = 0;
                    for (int y = -radius; y <= radius; y++)
                    {
                        for (int x = -radius; x <= radius; x++)
                        {
                            float sqrDst = x * x + y * y;
                            if (sqrDst < radius * radius)
                            {
                                int coordX = centreX + x;
                                int coordY = centreY + y;

                                if (coordX >= 0 && coordX < mapSize && coordY >= 0 && coordY < mapSize)
                                {
                                    float weight = 1.0f - (float)Math.Sqrt(sqrDst) / radius;
                                    weightSum += weight;
                                    weights[addIndex] = weight;
                                    xOffsets[addIndex] = x;
                                    yOffsets[addIndex] = y;
                                    addIndex++;
                                }
                            }
                        }
                    }
                }

                int numEntries = addIndex;
                erosionBrushIndices[i] = new int[numEntries];
                erosionBrushWeights[i] = new float[numEntries];

                for (int j = 0; j < numEntries; j++)
                {
                    erosionBrushIndices[i][j] = (yOffsets[j] + centreY) * mapSize + xOffsets[j] + centreX;
                    erosionBrushWeights[i][j] = weights[j] / weightSum;
                }
            }
        }

        struct HeightAndGradient
        {
            public float height;
            public float gradientX;
            public float gradientY;
        }
    }
}
