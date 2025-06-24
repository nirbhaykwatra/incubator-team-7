using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace Internment.Digging.Terrain
{
    public class Marching : MonoBehaviour
    {

        public enum TerrainType { Dirt = 0, Rock = 1 }

        public struct Voxel
        {
            public TerrainType type;
            public int health;
            public float density;  // for marching cubes thresholding
        }

        [Header("Terrain Settings")] 
        [SerializeField]
        private bool isSmoothTerrain;
        [SerializeField] 
        private bool isFlatShaded;
        [SerializeField] 
        public float terrainSurface = 0.5f;
        [SerializeField] 
        public int width = 32;
        [SerializeField] 
        public int height = 8;
        [SerializeField]
        public int length = 8;
        [SerializeField]
        public TerrainType terrainType = TerrainType.Dirt;

        [Header("Materials")]
        public Material dirtMaterial, rockMaterial;
        public int dirtHealth = 1;
        public int rockHealth = 5;

        [Header("Voxel Resolution")]
        [SerializeField]
        [Min(1)] public int resolution = 1;

        private float voxelSize;
        private int W, H, D;

        private MeshFilter meshFilter;
        private MeshCollider meshCollider;
        public Voxel[,,] voxels;

        private void Start()
        {
            var mr = GetComponent<MeshRenderer>();

            mr.sharedMaterials = new[]
            {
                dirtMaterial,
                rockMaterial
            };
        }

        void OnEnable()
        {
            voxelSize = 1f / resolution;
            W = width * resolution + 1;
            H = height * resolution + 1;
            D = length * resolution + 1;

            voxels = new Voxel[W, H, D];
            PopulateVoxels_AsCube();

            meshFilter = GetComponent<MeshFilter>();
            meshCollider = GetComponent<MeshCollider>();

            UpdateBlockyMesh();

            Physics.SyncTransforms();
        }

        Mesh BuildBlockyMeshFromVoxels()
        {
            
            var verts = new List<Vector3>();
            var tris0 = new List<int>();
            var tris1 = new List<int>();
            var uvs = new List<Vector2>();

            int cx = W, cy = H, cz = D;

            // directions & corner offsets (unchanged)
            Vector3[] norms = { Vector3.up, Vector3.down, Vector3.left,
                        Vector3.right, Vector3.forward, Vector3.back };
            Vector3[,] corners = {
      { new Vector3(0,1,0), new Vector3(1,1,0), new Vector3(1,1,1), new Vector3(0,1,1) }, // up
      { new Vector3(0,0,0), new Vector3(0,0,1), new Vector3(1,0,1), new Vector3(1,0,0) }, // down
      { new Vector3(0,0,0), new Vector3(0,1,0), new Vector3(0,1,1), new Vector3(0,0,1) }, // left
      { new Vector3(1,0,0), new Vector3(1,0,1), new Vector3(1,1,1), new Vector3(1,1,0) }, // right
      { new Vector3(0,0,1), new Vector3(0,1,1), new Vector3(1,1,1), new Vector3(1,0,1) }, // forward
      { new Vector3(0,0,0), new Vector3(1,0,0), new Vector3(1,1,0), new Vector3(0,1,0) }, // back
    };
            Vector2[] faceUVs = {
                new Vector2(0,0),
                new Vector2(1,0),
                new Vector2(1,1),
                new Vector2(0,1),
            };

            bool IsSolid(int x, int y, int z) =>
              x >= 0 && y >= 0 && z >= 0 && x < cx && y < cy && z < cz && voxels[x, y, z].density <= 0f;

            // 1) Generate the *outer* faces exactly as before
            for (int x = 0; x < cx; x++)
                for (int y = 0; y < cy; y++)
                    for (int z = 0; z < cz; z++)
                    {
                        if (!IsSolid(x, y, z)) continue;
                        int typeIndex = (int)voxels[x, y, z].type;

                        for (int f = 0; f < 6; f++)
                        {
                            int nx = x + (int)norms[f].x;
                            int ny = y + (int)norms[f].y;
                            int nz = z + (int)norms[f].z;
                            if (!IsSolid(nx, ny, nz))
                            {
                                int b = verts.Count;
                                for (int i = 0; i < 4; i++)
                                {
                                    verts.Add((new Vector3(x, y, z) + corners[f, i]) * voxelSize);
                                    uvs.Add(faceUVs[i]);
                                }
                                var target = (typeIndex == 1) ? tris1 : tris0;
                                // add two triangles (wound outward)
                                target.AddRange(new[] { b, b + 1, b + 2, b, b + 2, b + 3 });
                            }
                        }
                    }

            // 2) Remember how many verts & tris we have before mirroring
            int outerVertCount = verts.Count;
            var outer0 = tris0.ToArray();
            var outer1 = tris1.ToArray();

            // 3) Duplicate vertices so inner faces have their own normals
            verts.AddRange(verts.Take(outerVertCount));
            uvs.AddRange(uvs.Take(outerVertCount));

            // 4) Build the *inner* triangles by reversing each outer triangle
            var inner0 = new List<int>();
            for (int i = 0; i < outer0.Length; i += 3)
            {
                int a = outer0[i], b = outer0[i + 1], c = outer0[i + 2];
                // add the mirrored triangle on the duplicated verts
                inner0.AddRange(new[]{ c + outerVertCount,
                               b + outerVertCount,
                               a + outerVertCount });
            }
            var inner1 = new List<int>();
            for (int i = 0; i < outer1.Length; i += 3)
            {
                int a = outer1[i], b = outer1[i + 1], c = outer1[i + 2];
                inner1.AddRange(new[]{ c + outerVertCount,
                               b + outerVertCount,
                               a + outerVertCount });
            }

            // 5) Append inner triangles to each submesh
            tris0.AddRange(inner0);
            tris1.AddRange(inner1);

            // 6) Create the mesh with two submeshes
            var mesh = new Mesh
            {
                indexFormat = IndexFormat.UInt32,
                subMeshCount = 2,
                vertices = verts.ToArray(),
                uv = uvs.ToArray()
            };
            mesh.SetTriangles(tris0, 0);
            mesh.SetTriangles(tris1, 1);

            // 7) Recalculate normals so outer normals point out and inner point in
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
            return mesh;
        }

        private void UpdateBlockyMesh()
        {
            var mesh = BuildBlockyMeshFromVoxels();
            meshFilter.sharedMesh = mesh;
            meshCollider.sharedMesh = mesh;
        }

        public void PopulateVoxels_AsCube()
        {

            for (int x = 0; x < W; x++)
            for (int y = 0; y < H; y++)
            for (int z = 0; z < D; z++)
            {
                // Decide if (x,y,z) is inside the cube you want
                bool inside = true;
                
                float density = inside ? -1f : +1f;


                // Store it
                voxels[x, y, z] = new Voxel
                {
                    density = density,
                    type = terrainType,
                    health = (terrainType == TerrainType.Rock) ? rockHealth : dirtHealth
                };
            }
        }

        public void PlaceTerrain(Vector3 worldPos)
        {
            Vector3 localPos = transform.InverseTransformPoint(worldPos);
            int xi = Mathf.FloorToInt(localPos.x);
            int yi = Mathf.FloorToInt(localPos.y);
            int zi = Mathf.FloorToInt(localPos.z);

            if (!IsInBounds(xi, yi, zi))
            {
                return;
            }

            voxels[xi, yi, zi].density = 0f;

            UpdateBlockyMesh();
        }

        public void RemoveTerrain(Vector3 worldPos, int radiusInVoxels = 1)
        {
            Vector3 local = transform.InverseTransformPoint(worldPos) / voxelSize;
            int cx = Mathf.FloorToInt(local.x);
            int cy = Mathf.FloorToInt(local.y);
            int cz = Mathf.FloorToInt(local.z);

            // 2) Loop over a cube of side (2*radiusInVoxels+1)
            for (int dx = -radiusInVoxels; dx <= radiusInVoxels; dx++)
            for (int dy = -radiusInVoxels; dy <= radiusInVoxels; dy++)
            for (int dz = -radiusInVoxels; dz <= radiusInVoxels; dz++)
            {
                int x = cx + dx, y = cy + dy, z = cz + dz;

                // 3) Bound check against your actual array dims W,H,D
                if (x < 0 || x >= W ||
                    y < 0 || y >= H ||
                    z < 0 || z >= D)
                    continue;

                // 4) Keep it spherical
                if (dx * dx + dy * dy + dz * dz > radiusInVoxels * radiusInVoxels)
                    continue;

                // 5) Decrement health, only clear density when it hits zero
                ref Voxel v = ref voxels[x, y, z];
                if (v.density <= 0f && v.health > 0)
                {
                    v.health--;
                    if (v.health <= 0)
                        v.density = +1f;   // carve it out
                }
            }

            // 6) Rebuild the exact same blocky mesh from the mutated voxel grid
            UpdateBlockyMesh();
        }

        private bool IsInBounds(int x, int y, int z) =>
            x >= 0 && x < W && y >= 0 && y < H && z >= 0 && z < D;
    }
}