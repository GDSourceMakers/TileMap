using System;
using UnityEngine;
using UnityEditorInternal;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

using ClipperLib;
using Path = System.Collections.Generic.List<ClipperLib.IntPoint>;
using Paths = System.Collections.Generic.List<System.Collections.Generic.List<ClipperLib.IntPoint>>;

namespace BasicUtility.TileMap
{

    public delegate void UpdateEvent();

    public delegate void StartEvent();

    //[ExecuteInEditMode]
    //[RequireComponent(typeof(MeshFilter))]
    //[RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(PolygonCollider2D))]
    public class TileMap : MonoBehaviour
    {
        #region variables
        //[Header("Size")]
        public int map_x = 1;
        public int map_y = 1;
        //[Space]
        public int chunk_size_x = 64;
        public int chunk_size_y = 64;
        //[Space]
        public int chunk_count_x = 1;
        public int chunk_count_y = 1;
        //[Space]
        public Vector2 tileSize = Vector2.one;

        //[Header("Oriantion")]
        public MapOrientation oriantion;

        //[Header("Texture")]
        //public AtlasArray atlas;
        public Texture2DArray textureArray;
        public List<Texture2D> referenceAtlases;
        public Texture2D missingTexture;
        public int textureSize;

        public static Vector2[] uvDef = { new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1) };

        //[Header("Setting")]
        public bool autoGenMesh;
        public bool autoGenCollision;
        public bool autoUpdateMesh;


        public Material tileMat;

        public List<Layer> layers;

        [HideInInspector]
        public struct Grid
        {

            internal TileMap map;

            public TileBehaviour this[int x, int y, int layer]
            {
                get
                {
                    return map.GetTile(new TilePosition(layer, x, y, map));
                }
                set
                {
                    map.SetTile(value, new TilePosition(layer, x, y, map));
                }
            }

            public Grid(TileMap setMap)
            {
                map = setMap;
            }
        }

        [HideInInspector]
        public Grid grid;

        public UpdateEvent updateEvent;

        public StartEvent startEvent;


        bool firstUpdate = true;

        #endregion

        // Use this for initialization
        void Awake()
        {
            grid = new Grid(this);

            map_x = chunk_size_x * chunk_count_x;
            map_y = chunk_size_y * chunk_count_y;

            for (int i = 0; i < layers.Count; i++)
            {
                layers[i].Instanciate(this);
            }

            BuildAtlas();
        }

        // Update is called once per frame
        void Update()
        {
            if (firstUpdate)
            {
                firstUpdate = false;
                if (startEvent != null)
                {
                    startEvent();
                }
            }
            if (updateEvent != null)
            {
                updateEvent();
            }
        }

        void LateUpdate()
        {
            if (autoUpdateMesh)
            {
                UpdateAll();
            }

            for (int i = 0; i < layers.Count; i++)
            {
                layers[i].RenderLayer();
            }
        }

        #region Updates

        public void UpdateAll()
        {
            for (int i = 0; i < layers.Count; i++)
            {
                layers[i].UpdateLayer();
            }
        }

        public void UpdateLayer(int i)
        {
            layers[i].UpdateLayer();
        }

        public void UpdateChunk(TilePosition pos)
        {
            pos.layer.UpdateChunk(pos);
        }

        public void UpdateTile(TilePosition pos)
        {
            pos.layer.UpdateTile(pos);
        }

        #endregion

        #region Generates

        public void GenerateAllMesh()
        {
            for (int i = 0; i < layers.Count; i++)
            {
                layers[i].GenerateMesh();
            }
        }

        public void GenerateLayerMesh(int i)
        {
            layers[i].GenerateMesh();
        }

        public void GenerateChunkMesh(TilePosition pos)
        {
            pos.layer.GenerateChunkMesh(pos.chunk_x, pos.chunk_y);
        }

        #endregion

        public void SetTile(TileBehaviour tileClass, TilePosition pos, bool auto = true)
        {
            if (Inside(pos))
            {
                //Debug.Log("Layer: " + pos.layerNumber + " X_chunk: " + pos.chunk_x + " Y_chunk: " + pos.chunk_y + " X: " + pos.x + " Y: " + pos.y);

                Chunk chunk = pos.layer.chunkGrid[pos.chunk_x, pos.chunk_y];

                tileClass.SetupTile(this, pos);

                chunk.grid[pos.chunk_relative_x, pos.chunk_relative_y] = tileClass;

                if (auto)
                {
                    if (!chunk.HasMesh() && autoGenMesh)
                    {
                        chunk.GenerateMesh();
                        chunk.GenerateCollision();
                    }
                    else if (chunk.HasMesh() && autoUpdateMesh)
                    {
                        chunk.UpdateTile(pos);

                    }
                }
            }
        }

        private bool Inside(TilePosition pos)
        {
            return
            pos.chunk_x >= 0 &&
            pos.chunk_x < chunk_count_x &&
            pos.chunk_y >= 0 &&
            pos.chunk_y < chunk_count_y &&
            pos.chunk_relative_x >= 0 &&
            pos.chunk_relative_x < chunk_size_x &&
            pos.chunk_relative_y >= 0 &&
            pos.chunk_relative_y < chunk_size_y;
        }

        public TileBehaviour GetTile(TilePosition pos)
        {
            Chunk chunk = pos.layer.chunkGrid[pos.chunk_x, pos.chunk_y];

            return chunk.grid[pos.chunk_relative_x, pos.chunk_relative_y];
        }


        public Layer GetLayer(string name)
        {
            foreach (var item in layers)
            {
                if (item.name == name)
                {
                    return item;
                }
            }

            return null;
        }



        public void BuildAtlas()
        {
            if (textureSize == 0 || (referenceAtlases == null && referenceAtlases.Count == 0))
                return;

            textureArray = new Texture2DArray(textureSize, textureSize, referenceAtlases.Count, TextureFormat.ARGB32, true);
            textureArray.filterMode = FilterMode.Point;
            for (int i = 0; i < referenceAtlases.Count; i++)
            {
                textureArray.SetPixels(referenceAtlases[i].GetPixels(), i);
            }

            textureArray.Apply(true);

            if (missingTexture == null)
            {
                missingTexture = new Texture2D(1, 1);
                missingTexture.SetPixel(0, 0, new Color(0, 0, 0, 0));
                missingTexture.Apply();
            }
            tileMat = new Material(Shader.Find("TileMap/Shader"));
            //atlas.GenerateArray();
            //Debug.Log(atlas.array.filterMode);
            tileMat.mainTexture = textureArray;
            tileMat.SetTexture("_MissingTex", missingTexture);

        }

        public static List<List<Vector2>> RemoveClosePointsInPolygons(List<List<Vector2>> polygons)
        {
            float proximityLimit = 0.1f;

            List<List<Vector2>> resultPolygons = new List<List<Vector2>>();

            foreach (List<Vector2> polygon in polygons)
            {
                List<Vector2> pointsToTest = polygon;
                List<Vector2> pointsToRemove = new List<Vector2>();

                foreach (Vector2 pointToTest in pointsToTest)
                {
                    foreach (Vector2 point in polygon)
                    {
                        if (point == pointToTest || pointsToRemove.Contains(point)) continue;

                        bool closeInX = Math.Abs(point.x - pointToTest.x) < proximityLimit;
                        bool closeInY = Math.Abs(point.y - pointToTest.y) < proximityLimit;

                        if (closeInX && closeInY)
                        {
                            pointsToRemove.Add(pointToTest);
                            break;
                        }
                    }
                }
                polygon.RemoveAll(x => pointsToRemove.Contains(x));

                if (polygon.Count > 0)
                {
                    resultPolygons.Add(polygon);
                }
            }

            return resultPolygons;
        }

        void OnDrawGizmos()
        {
            if (layers == null || layers.Count == 0)
            {
                return;
            }

            int grid_x;
            int grid_y;

            float grid_size_x;
            float grid_size_y;

            Camera sceenCam = SceneView.lastActiveSceneView.camera;

            for (int i = 0; i < layers.Count; i++)
            {
                if (layers[i].showGrid)
                {
                    Vector3 position_p = layers[i].position;
                    Quaternion rotation_p = layers[i].rotation;

                    Color col = layers[i].color;

                    // set colours
                    Color dimColor = new Color(col.r, col.g, col.b, 0.45f * col.a);
                    Color brightColor = Color.Lerp(Color.white, col, 0.75f);

                    Plane plane = new Plane(position_p, position_p + rotation_p * new Vector3(0, 1, 0), position_p + rotation_p * new Vector3(1, 0, 0));

                    if (plane.GetDistanceToPoint(sceenCam.transform.position) > chunk_size_x * tileSize.x * 7)
                    {
                        grid_x = chunk_count_x;
                        grid_y = chunk_count_y;

                        grid_size_x = chunk_size_x;
                        grid_size_y = chunk_size_y;
                    }
                    else
                    {
                        grid_x = map_x;
                        grid_y = map_y;

                        grid_size_x = tileSize.x;
                        grid_size_y = tileSize.y;

                        DrawGrid(chunk_size_x, chunk_size_y, chunk_count_x, chunk_count_y, position_p, rotation_p, oriantion, brightColor);
                    }

                    DrawGrid(grid_size_x, grid_size_y, grid_x, grid_y, position_p, rotation_p, oriantion, dimColor);
                }

            }
        }

        void DrawGrid(float size_x, float size_y, int count_x, int count_y, Vector3 pos, Quaternion rot, MapOrientation type, Color c)
        {
            float h = count_y * size_y;
            float w = count_x * size_x;

            Gizmos.color = c;

            Vector3 trans;
            if (oriantion == MapOrientation.xSwaped)
            {
                trans = new Vector3(-1, 1, 1);
            }
            else if (oriantion == MapOrientation.ySwaped)
            {
                trans = new Vector3(1, -1, 1);
            }
            else if (oriantion == MapOrientation.xySwaped)
            {
                trans = new Vector3(-1, -1, 1);
            }
            else
            {
                trans = new Vector3(1, 1, 1);
            }

            for (int i = 0; i < count_x + 1; i++)
            {
                float x = (i * size_x);

                Vector3 start = rot * Vector3.Scale(
                    new Vector3(
                        x,
                        0,
                        0
                    ), trans);
                Vector3 end = rot * Vector3.Scale(
                    new Vector3(
                        x,
                        h,
                        0
                    ), trans);
                Gizmos.DrawLine(start + pos, end + pos);
            }

            for (int i = 0; i < count_y + 1; i++)
            {
                float y = (i * size_y);

                Vector3 start = rot * Vector3.Scale(
                        new Vector3(
                            0,
                            y,
                            0
                        ), trans);
                Vector3 end = rot * Vector3.Scale(
                    new Vector3(
                         w,
                        y,
                        0
                    ), trans);
                Gizmos.DrawLine(start + pos, end + pos);
            }

            Gizmos.color = Color.white;
        }

        #region Simple Quad
        /*
		public void BuildMeshSimple()
		{

			// Generate the mesh data
			Vector3[] vertices = new Vector3[4];
			Vector3[] normals = new Vector3[4];
			Vector2[] uv = new Vector2[4];

			int[] triangles = new int[2 * 3];


			int a = 0;
			vertices[0] = new Vector3(0, 0, a);
			vertices[1] = new Vector3((size_x * tileSize), 0, a);
			vertices[2] = new Vector3(0, (size_y * tileSize), a);
			vertices[3] = new Vector3((size_x * tileSize), (size_y * tileSize), a);


			//normals[y * verteciesSize_x + x+j] = Vector3.up;

			uv[0] = new Vector2(0, 0);
			uv[1] = new Vector2(1, 0);
			uv[2] = new Vector2(0, 1);
			uv[3] = new Vector2(1, 1);


			triangles[0] = 0;
			triangles[1] = 3;
			triangles[2] = 1;

			triangles[3] = 0;
			triangles[4] = 2;
			triangles[5] = 3;



			// Create a new Mesh and populate with the data
			Mesh mesh = new Mesh();
			mesh.vertices = vertices;
			mesh.triangles = triangles;
			mesh.normals = normals;
			mesh.uv = uv;

			// Assign our mesh to our filter/renderer/collider
			MeshFilter mesh_filter = GetComponent<MeshFilter>();
			MeshCollider mesh_collider = GetComponent<MeshCollider>();

			//mesh_filter.mesh = mesh;
			//mesh_collider.sharedMesh = mesh;
			tileMesh = mesh;
			Debug.Log("Done Mesh!");

			BuildTexture();
		}
		*/
        #endregion

    }

    [Serializable]
    public class Layer
    {
        [HideInInspector]
        public bool isExpanded;
        public string name = "New Layer";
        public int number;

        public bool showGrid = true;
        public Color color = Color.red;

        public Vector3 position;
        public Quaternion rotation;

        public Chunk[,] chunkGrid;

        public TileMap map;

        public int chunkSize_x;
        public int chunkSize_y;

        public int chunkCount_x;
        public int chunkCount_y;

        public bool[,] updateMap;

        public Layer(TileMap t_map, int num)
        {
            map = t_map;
            number = num;

            chunkSize_x = map.chunk_size_x;
            chunkSize_y = map.chunk_size_y;

            chunkCount_x = map.chunk_count_x;
            chunkCount_y = map.chunk_count_y;

            chunkGrid = new Chunk[chunkCount_x, chunkCount_y];
            updateMap = new bool[chunkCount_x, chunkCount_y];

            rotation = Quaternion.identity;

            for (int x = 0; x < chunkCount_x; x++)
            {
                for (int y = 0; y < chunkCount_y; y++)
                {
                    chunkGrid[x, y] = new Chunk(this, x, y);

                }
            }

        }

        public void Instanciate(TileMap t_map)
        {
            map = t_map;

            chunkSize_x = map.chunk_size_x;
            chunkSize_y = map.chunk_size_y;

            chunkCount_x = map.chunk_count_x;
            chunkCount_y = map.chunk_count_y;

            chunkGrid = new Chunk[chunkCount_x, chunkCount_y];
            updateMap = new bool[chunkCount_x, chunkCount_y];

            rotation = Quaternion.identity;

            for (int x = 0; x < chunkCount_x; x++)
            {
                for (int y = 0; y < chunkCount_y; y++)
                {
                    chunkGrid[x, y] = new Chunk(this, x, y);

                }
            }
        }

        public void GenerateMesh()
        {
            for (int x = 0; x < map.map_x / map.chunk_size_x; x++)
            {
                for (int y = 0; y < map.map_y / map.chunk_size_y; y++)
                {
                    chunkGrid[x, y].GenerateMesh();
                }
            }
        }

        public void GenerateChunkMesh(int x, int y)
        {
            chunkGrid[x, y].GenerateMesh();
        }


        public void UpdateLayer()
        {
            for (int x = 0; x < chunkCount_x; x++)
            {
                for (int y = 0; y < chunkCount_y; y++)
                {
                    if (updateMap != null && updateMap[x, y])
                    {
                        chunkGrid[x, y].UpdateChunk();
                    }
                }
            }
        }

        public void UpdateChunk(TilePosition pos)
        {
            if (updateMap[pos.chunk_x, pos.chunk_y])
            {
                chunkGrid[pos.chunk_x, pos.chunk_y].UpdateChunk();
            }
        }

        public void UpdateTile(TilePosition pos)
        {
            chunkGrid[pos.chunk_x, pos.chunk_y].UpdateTile(pos);
        }


        public void UpdateChunkCollider(TilePosition pos)
        {
            chunkGrid[pos.chunk_x, pos.chunk_y].GenerateCollision();
        }

        public void RenderLayer()
        {
            for (int x = 0; x < chunkCount_x; x++)
            {
                for (int y = 0; y < chunkCount_y; y++)
                {
                    if (chunkGrid != null && chunkGrid[x, y] != null && chunkGrid[x, y].HasMesh())
                    {
                        int x_cord = ((map.oriantion == MapOrientation.xSwaped) || (map.oriantion == MapOrientation.xySwaped)) ? -x : x;
                        int y_cord = ((map.oriantion == MapOrientation.ySwaped) || (map.oriantion == MapOrientation.xySwaped)) ? -y : y;

                        //Debug.Log(((map.oriantion == TileVectorTypes.ySwaped) && (map.oriantion == TileVectorTypes.xySwaped)) ? -y : y);

                        Graphics.DrawMesh(chunkGrid[x, y].tileMesh, position + new Vector3(chunkSize_x * x_cord, chunkSize_y * y_cord), rotation, map.tileMat, 0);
                    }
                }
            }
        }


    }

    [Serializable]
    public class Chunk
    {
        internal Mesh tileMesh;
        public Layer layer;
        public int chunkPos_x;
        public int chunkPos_y;


        public TileBehaviour[,] grid;

        Vector2[] uvChanges;
        Vector2[] textureLayerChanges;
        Color[] colorChanges;

        internal bool[,] collisionChanges;
        internal bool collisionChanged;

        internal bool[,] tileChanges;

        //internal bool[,] colorChanges;
        //internal bool colorChanged;

        public bool HasMesh()
        {
            return tileMesh != null;
        }

        public Chunk(Layer t_layer, int x, int y)
        {
            layer = t_layer;

            int numberTiles = layer.chunkSize_x * layer.chunkSize_y;
            int verteciesCount = numberTiles * 4;

            layer = t_layer;

            chunkPos_x = x;
            chunkPos_y = y;

            grid = new TileBehaviour[layer.chunkSize_x, layer.chunkSize_y];

            tileChanges = new bool[layer.chunkSize_x, layer.chunkSize_y];
            collisionChanges = new bool[layer.chunkSize_x, layer.chunkSize_y];
            //colorChanges = new bool[layer.chunkSize_x, layer.chunkSize_y];

            uvChanges = new Vector2[verteciesCount];
            textureLayerChanges = new Vector2[verteciesCount];
            colorChanges = new Color[verteciesCount];
        }


        public void GenerateMesh()
        {
            int numberTiles = layer.chunkSize_x * layer.chunkSize_y;
            int verteciesCount = numberTiles * 4;

            // Generate the mesh data
            Vector3[] vertices = new Vector3[verteciesCount];
            Vector3[] normals = new Vector3[verteciesCount];

            int[] triangles = new int[numberTiles * 2 * 3];

            Vector2 tileSize = layer.map.tileSize;

            for (int x = 0; x < layer.chunkSize_x; x++)
            {
                for (int y = 0; y < layer.chunkSize_y; y++)
                {
                    int x_cord = ((layer.map.oriantion == MapOrientation.xSwaped) || (layer.map.oriantion == MapOrientation.xySwaped)) ? -(x + 1) : x;
                    int y_cord = ((layer.map.oriantion == MapOrientation.ySwaped) || (layer.map.oriantion == MapOrientation.xySwaped)) ? -(y + 1) : y;

                    
                    int tile = (layer.chunkSize_x * y * 4) + (x * 4);
                    int a = 0;
                    vertices[tile + 2] = new Vector3(x_cord * tileSize.x, y_cord * tileSize.y, a);
                    vertices[tile + 3] = new Vector3((x_cord * tileSize.x) + tileSize.x, y_cord * tileSize.y, a);
                    vertices[tile + 0] = new Vector3(x_cord * tileSize.x, (y_cord * tileSize.y) + tileSize.y, a);
                    vertices[tile + 1] = new Vector3((x_cord * tileSize.x) + tileSize.x, (y_cord * tileSize.y) + tileSize.y, a);


                    GenUvData(x, y);
                    //uv = uvChanges;
                    //uv2 = textureLayer;
                    

                    int squareIndex = y * layer.chunkSize_x + x;
                    int triOffset = squareIndex * 6;

                    triangles[triOffset + 0] = tile + 0;
                    triangles[triOffset + 1] = tile + 3;
                    triangles[triOffset + 2] = tile + 1;

                    triangles[triOffset + 3] = tile + 0;
                    triangles[triOffset + 4] = tile + 2;
                    triangles[triOffset + 5] = tile + 3;

                }
            }

            // Create a new Mesh and populate with the data
            Mesh mesh = new Mesh();

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.normals = normals;

            mesh.uv = uvChanges;
            mesh.uv2 = textureLayerChanges;
            mesh.colors = colorChanges;

            mesh.name = "Chunk " + chunkPos_x + ", " + chunkPos_y + "";

            tileMesh = mesh;

            //Debug.Log("Done Mesh!");
        }

        public void GenerateCollision()
        {
            //List<List<Vector2>> polygons = new List<List<Vector2>>();

            List<List<Vector2>> unitedPolygons = new List<List<Vector2>>();
            Clipper clipper = new Clipper();

            int scalingFactor = 10000;

            for (int x = 0; x < layer.chunkSize_x; x++)
            {
                for (int y = 0; y < layer.chunkSize_y; y++)
                {

                    if (grid[x, y] != null && grid[x, y].colider)
                    {
                        Path newPoligons = new Path(4);

                        newPoligons.Add(new IntPoint(Mathf.Floor((0 + x) * scalingFactor), Mathf.Floor((0 + y) * scalingFactor)));
                        newPoligons.Add(new IntPoint(Mathf.Floor((0 + x) * scalingFactor), Mathf.Floor((1 + y) * scalingFactor)));
                        newPoligons.Add(new IntPoint(Mathf.Floor((1 + x) * scalingFactor), Mathf.Floor((1 + y) * scalingFactor)));
                        newPoligons.Add(new IntPoint(Mathf.Floor((1 + x) * scalingFactor), Mathf.Floor((0 + y) * scalingFactor)));

                        clipper.AddPath(newPoligons, PolyType.ptSubject, true);
                    }
                }
            }

            Paths solution = new Paths();

            clipper.Execute(ClipType.ctUnion, solution);

            ClipperOffset offset = new ClipperOffset();
            offset.AddPaths(solution, JoinType.jtMiter, EndType.etClosedPolygon);
            offset.Execute(ref solution, 5f);

            foreach (Path path in solution)
            {
                List<Vector2> unitedPolygon = new List<Vector2>();
                foreach (IntPoint point in path)
                {
                    unitedPolygon.Add(new Vector2(point.X / (float)scalingFactor, point.Y / (float)scalingFactor));
                }
                unitedPolygons.Add(unitedPolygon);
            }

            unitedPolygons = TileMap.RemoveClosePointsInPolygons(unitedPolygons);

            PolygonCollider2D collider = layer.map.gameObject.GetComponent<PolygonCollider2D>();

            collider.pathCount = unitedPolygons.Count;

            for (int i = 0; i < unitedPolygons.Count; i++)
            {
                Vector2[] points = unitedPolygons[i].ToArray();

                collider.SetPath(i, points);
            }
        }


        public void UpdateTile(TilePosition pos)
        {
            //Vector2[] uvChanges = new Vector2[verteciesCount];
            //Vector2[] textureLayer = new Vector2[verteciesCount];

            if (pos.chunk_x != chunkPos_x || pos.chunk_y != chunkPos_y)
            {
                Debug.Log("Passed values are not for this chunk: Chunk: " + chunkPos_x + "," + chunkPos_y + " Passed: " + pos.chunk_x + "," + pos.chunk_y);
                return;
            }

            if (tileChanges[pos.chunk_relative_x, pos.chunk_relative_y])
            {
                GenUvData(pos.chunk_relative_x, pos.chunk_relative_y);

                ApplyUpdates();

                tileChanges[pos.chunk_relative_x, pos.chunk_relative_y] = false;
            }
            if (collisionChanges[pos.chunk_relative_x, pos.chunk_relative_y])
            {
                GenerateCollision();
            }
        }

        public void UpdateChunk()
        {
            for (int x = 0; x < layer.chunkSize_x; x++)
            {
                for (int y = 0; y < layer.chunkSize_y; y++)
                {
                    if (tileChanges[x, y])
                    {

                        GenUvData(x, y);

                        tileChanges[x, y] = false;
                    }
                }
            }

            ApplyUpdates();

            if (collisionChanged)
            {
                GenerateCollision();
            }

            layer.updateMap[chunkPos_x, chunkPos_y] = false;
        }



        //public bool a = true;

        public void GenUvData(int x, int y)
        {
            int tile = (layer.chunkSize_x * y * 4) + (x * 4);

            Vector2[] tileUV = null;
            int atlasLayer = 0;
            Color tileColor = Color.white;

            bool empty = false;

            Texture texture = null;
            TileBehaviour currentTile = grid[x, y];

            if (currentTile != null && (currentTile.sprite != null || (currentTile is IAdvancedSprite && ((IAdvancedSprite)currentTile).GetSprite(x, y, layer) != null)))
            {

                if (grid[x, y] is IAdvancedSprite)
                {
                    texture = ((IAdvancedSprite)currentTile).GetSprite(x, y, layer).texture;
                    tileUV = ((IAdvancedSprite)currentTile).GetSprite(x, y, layer).uv;
                    //TODO: Change when IAdvancedSprite has color
                    tileColor = Color.white;
                }
                else
                {
                    texture = currentTile.sprite.texture;
                    tileUV = currentTile.sprite.uv;
                    tileColor = currentTile.color;
                }

                for (int k = 0; k < layer.map.referenceAtlases.Count; k++)
                {
                    if (layer.map.referenceAtlases[k] == texture)
                    {
                        atlasLayer = k;
                    }
                }
            }
            else
            {
                tileUV = TileMap.uvDef;
                empty = true;

            }

            for (int i = 0; i < 4; i++)
            {
                uvChanges[tile + i] = tileUV[i];
                colorChanges[tile + i] = tileColor;


                if (!empty)
                {
                    textureLayerChanges[tile + i] = new Vector2(atlasLayer, 0);
                }
                else
                {
                    textureLayerChanges[tile + i] = new Vector2(255, 255);
                }

                //Debug.Log("Layer: " + textureLayer[tile + i] + ", uv: " + uvChanges[tile + i]);
            }
        }

		public void ApplyUpdates()
		{
            tileMesh.uv = uvChanges;
            tileMesh.uv2 = textureLayerChanges;
            tileMesh.colors = colorChanges;
        }
		
        /*
		[Obsolete()]
		public void UpdateChunkMesh()
		{
			for (int x = 0; x < layer.chunkSize_x; x++)
			{
				for (int y = 0; y < layer.chunkSize_y; y++)
				{
					int tile = (layer.chunkSize_x * y * 4) + (x * 4);

					for (int i = 0; i < 4; i++)
					{
						if (grid[x, y] != null)
						{
							uvChanges[tile + i] = grid[x, y].sprite.uv[3 - i];

							tileMesh.colors32[tile + i] = new Color32(0, 255, 255, 255);
						}
					}
				}
			}

			layer.updateMap[chunkPos_x, chunkPos_y] = true;
			hasChanged = true;

		}
		*/
    }

    public class TileBehaviour
    {
        TileMap map;
        Layer layer;
        Chunk chunk;

        public TilePosition position { private set; get; }

        Sprite _sprite;
        public Sprite sprite
        {
            get
            {
                return _sprite;
            }
            set
            {
                _sprite = value;
                if (map != null)
                {
                    MarkDirty();
                }
            }
        }

        Color _color = Color.white;
        public Color color
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
                if (map != null)
                {
                    MarkDirty();
                }
            }
        }

        GameObject _gameobject;
        public GameObject gameobject
        {
            get
            {
                return _gameobject;
            }
            private set
            {
                if (value != null)
                {
                    _gameobject = value;

                    //TODO:fix
                    //_gameobject.transform.position = position;
                }
            }
        }

        public bool _colider;
        public bool colider
        {
            get
            {
                return _colider;
            }
            set
            {
                if (map != null && _colider != value)
                {
                    chunk.collisionChanged = true;
                }
                _colider = value;
            }
        }

        bool hStart;
        bool hUpdate;

        TileBehaviour obj;

        internal void SetupTile(TileMap setMap, TilePosition setPos)
        {
            map = setMap;
            layer = setPos.layer;
            chunk = layer.chunkGrid[setPos.chunk_x, setPos.chunk_y];
            position = setPos;

            MarkDirty();

            //Thread a = new Thread(SetupEvents);
            //a.Start();
        }

        public void MarkDirty ()
        {
            chunk.tileChanges[position.chunk_relative_x, position.chunk_relative_y] = true;
            layer.updateMap[position.chunk_x, position.chunk_y] = true;

        }

        /*
        public void SetupEvents()
        {
            if (hStart)
            {
                map.startEvent += obj.Start;
            }

            if (hUpdate)
            {
                map.updateEvent += obj.Update;
            }
        }
        */

        public void TileSetup<T>(bool hasUpdate, bool hasStart, T objAdd) where T : TileBehaviour
        {
            this.obj = objAdd;
            //Debug.Log(typeof(T));

            hStart = hasStart;
            hUpdate = hasUpdate;
        }

        //public virtual void Update() { }

        //public virtual void Start() { }

    }

    public class TilePosition
    {
        public Layer layer;

        public int x;
        public int y;

        public int chunk_x;
        public int chunk_y;

        public int chunk_relative_x;
        public int chunk_relative_y;

        TileMap mapGrid;

        //public static TileVector zero = new TileVector(0, 0);

        public TilePosition(int layer, int ChunkX, int ChunkY, int posX, int posY, TileMap map)
        {
            Init(layer, ChunkX, ChunkY, posX, posY, map);
        }

        public TilePosition(Layer layer, int ChunkX, int ChunkY, int posX, int posY)
        {


            Init(layer.number, ChunkX, ChunkY, posX, posY, layer.map);
        }

        public TilePosition(Layer layer, int posX, int posY)
        {


            Init(layer.number, posX, posY, layer.map);
        }

        public TilePosition(int layer, int posX, int posY, TileMap map)
        {
            this.Init(layer, posX, posY, map);
        }

        void Init(int layer, int posX, int posY, TileMap map)
        {
            mapGrid = map;

            this.layer = map.layers[layer];

            chunk_x = posX / mapGrid.chunk_size_x;
            chunk_y = posY / mapGrid.chunk_size_y;


            chunk_relative_x = posX % mapGrid.chunk_size_x;
            chunk_relative_y = posY % mapGrid.chunk_size_y;

            x = posX;
            y = posY;
        }

        void Init(int layer, int ChunkX, int ChunkY, int posX, int posY, TileMap map)
        {
            mapGrid = map;

            this.layer = map.layers[layer];

            chunk_x = ChunkX;
            chunk_y = ChunkY;

            chunk_relative_x = posX;
            chunk_relative_y = posY;

            x = chunk_x * map.chunk_size_x + chunk_relative_x;
            y = chunk_y * map.chunk_size_y + chunk_relative_y;
        }

        /// <summary>
        /// Will use the layer of the first parameter
        /// </summary>
        /// <param name="l"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        static public TilePosition operator +(TilePosition l, TilePosition r)
        {
            return new TilePosition(l.layer, l.x + r.x, l.y + r.y);
        }

        static public TilePosition operator -(TilePosition l, TilePosition r)
        {
            return new TilePosition(l.layer, l.x - r.x, l.y - r.y);
        }

        /*
        static public implicit operator TilePosition(Vector2 v)
        {
            return new TilePosition(null,(int)v.x, (int)v.y);
        }

        static public implicit operator TilePosition(Vector3 v)
        {
            return new TilePosition(v.x, v.y);
        }
        */

        //TODO: Do the math so the convertion is good
        /*
        static public implicit operator Vector2(TilePosition v)
        {
            if (v.mapGrid.oriantion == MapOrientation.xSwaped)
            {
                return new Vector2(-v.relative_x, v.relative_y)+new Vector2(v.layer.position.x,v.layer.position.y);
            }
            else if (v.mapGrid.oriantion == MapOrientation.xySwaped)
            {
                return new Vector2(-v.relative_x, -v.relative_y) + new Vector2(v.layer.position.x, v.layer.position.y);
            }
            else if (v.mapGrid.oriantion == MapOrientation.ySwaped)
            {
                return new Vector2(v.relative_x, -v.relative_y) + new Vector2(v.layer.position.x, v.layer.position.y);
            }
            return new Vector2(v.relative_x, v.relative_y) + new Vector2(v.layer.position.x, v.layer.position.y);
        }

        static public implicit operator Vector3(TilePosition v)
        {
            if (v.swaping == MapOrientation.xSwaped)
            {
                return new Vector3(-v.relative_x, v.relative_y);
            }
            else if (v.swaping == MapOrientation.xySwaped)
            {
                return new Vector3(-v.relative_x, -v.relative_y);
            }
            else if (v.swaping == MapOrientation.ySwaped)
            {
                return new Vector3(v.relative_x, -v.relative_y);
            }
            return new Vector3(v.relative_x, v.relative_y);
        }
        */
    }

    public interface IAdvancedSprite
    {
        Sprite GetSprite(int x, int y, Layer l);
    }


    public enum MapOrientation
    {
        xSwaped,
        ySwaped,
        xySwaped,
        none
    }

}

