using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Utils;
using MagicTower.EditorData;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace MagicTower.Editor
{
    public class TileMapWindow : EditorWindow
    {
        public enum EEditMode
        {
            Select = 0,
            Modify = 1,
            Erase = 2,
        }

        EEditMode mEditMode = EEditMode.Select;
        ETileMapLayer mEditLayer = ETileMapLayer.Floor;
        int mEditRange = 1;

		uint mMapLevel = 1;

        EditorData.TileMap mTilemap;

        MonsterList mMonsterList;
        ItemList mItemList;
        TerrainList mTerrainList;
        PortalList mPortalList;
        DoorList mDoorList;

        [MenuItem("Window/TileMapWindow")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(TileMapWindow));
        }

        void OnEnable()
        {
            var tile_map = GameObject.FindGameObjectWithTag("TileMap");
            if (tile_map != null)
            {
                mTilemap = tile_map.GetComponent<EditorData.TileMap>();
            }

            SpriteSheetManager.Instance.Load("tile");

            mMonsterList = new MonsterList();
            for (uint i = 1; i <= 31; ++i)
            {
                mMonsterList.Add(i);
            }

            mItemList = new ItemList();
            for (uint i = 1; i <= 7; ++i)
            {
                mItemList.Add(i);
            }

            mTerrainList = new TerrainList();
            for (uint i = 1; i <= 4; ++i)
            {
                mTerrainList.Add(i);
            }

            mPortalList = new PortalList();
            mPortalList.Add(0);
            mPortalList.Add(1);

            mDoorList = new DoorList();
            mDoorList.Add(0);
            mDoorList.Add(1);
            mDoorList.Add(2);
            mDoorList.Add(3);
            mDoorList.Add(4);

            SceneView.onSceneGUIDelegate += TileMapUpdate;
        }

        bool GetTilePositionByMousePosition(Vector2 mouse_position, ref int row, ref int col)
        {
            Ray r = Camera.current.ScreenPointToRay(
                                    new Vector3(mouse_position.x, -mouse_position.y + Camera.current.pixelHeight));

            var mouse_pos = r.origin;

            var local_pos = mTilemap.gameObject.transform.InverseTransformPoint(mouse_pos);
            local_pos.x = Mathf.Floor(local_pos.x + 0.0f);
            local_pos.y = Mathf.Floor(local_pos.y + 0.0f);
            local_pos.z = 0.0f;

            if (local_pos.x < 0 || local_pos.x >= mTilemap.Width || local_pos.y < 0 || local_pos.y >= mTilemap.Height)
                return false;

            row = (int)local_pos.y;
            col = (int)local_pos.x;

            return true;
        }

        void TileMapUpdate(SceneView scene_view)
        {
            if (mTilemap == null)
                return;

            if (mEditMode == EEditMode.Modify)
            {
                if (Selecting.SelectedList == null)
                    return;

                Event e = Event.current;
                if (e.type == EventType.MouseDown)
                {
                    int row = 0;
                    int col = 0;
                    if (GetTilePositionByMousePosition(e.mousePosition, ref row, ref col))
                    {
                        var range = mEditRange - 1;
                        for (int r = row - range; r <= row + range; ++r)
                        {
                            for (int c = col - range; c <= col + range; ++c)
                            {
                                if (c < 0 || c >= mTilemap.Width || r < 0 || r >= mTilemap.Height)
                                    continue;

                                var obj = Selecting.SelectedList.CreateTile(Selecting.SelectedIndex);

                                mTilemap.SetTile(r, c, obj, mEditLayer);
                            }
                        }
                    }
                }
            }
            else if (mEditMode == EEditMode.Erase)
            {
                Event e = Event.current;
                if (e.type == EventType.MouseDown)
                {
                    int row = 0;
                    int col = 0;
                    if (GetTilePositionByMousePosition(e.mousePosition, ref row, ref col))
                    {
                        var range = mEditRange - 1;
                        for (int r = row - range; r <= row + range; ++r)
                        {
                            for (int c = col - range; c <= col + range; ++c)
                            {
                                if (c < 0 || c >= mTilemap.Width || r < 0 || r >= mTilemap.Height)
                                    continue;

                                mTilemap.RemoveTile(r, c, mEditLayer);
                            }
                        }
                    }
                }
            }
            else if (mEditMode == EEditMode.Select)
            {

            }
        }

        void OnGUI()
        {
            if (Application.isPlaying)
            {
                EditorGUILayout.LabelField("Please exit the play mode.");
                return;
            }

            if (mTilemap == null)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Level");
				mMapLevel = (uint)EditorGUILayout.IntField((int)mMapLevel);
				EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("TileMap Width:");
                var width = (uint)EditorGUILayout.IntField(11);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("TileMap Height");
                var height = (uint)EditorGUILayout.IntField(11);
                EditorGUILayout.EndHorizontal();

                if (GUILayout.Button("Generate TileMap"))
                {
                    var tile_map_obj = new GameObject("TileMap");
                    mTilemap = tile_map_obj.AddComponent<EditorData.TileMap>();
                    tile_map_obj.tag = "TileMap";

					mTilemap.Level = mMapLevel;
					mTilemap.Init(width, height);
                }
                
                if (GUILayout.Button("Load TileMap"))
                {
					var asset = Resources.Load("level" + mMapLevel) as TextAsset;
                    Data.TileMapData tile_map_data = null;
                    //using (var stream = new MemoryStream(asset.bytes))
                    //{
                    //    var formatter = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(Data.TileMapData));
                    //    tile_map_data = formatter.ReadObject(stream) as Data.TileMapData;
                    //    //tile_map_data = formatter.Deserialize(stream) as Data.TileMapData;
                    //}

                    //tile_map_data = Newtonsoft.JsonConvert.DeserializeObject<Data.TileMapData>(asset.bytes.ToString());
                    var content = System.Text.Encoding.Default.GetString(asset.bytes);
                    tile_map_data = LitJson.JsonMapper.ToObject<Data.TileMapData>(content);
                    
                    Resources.UnloadAsset(asset);

                    //var tile_map_data = AssetDatabase.LoadAssetAtPath("Assets/test.asset", typeof(Data.TileMapData)) as Data.TileMapData;

                    var tile_map_obj = new GameObject("TileMap");
                    mTilemap = tile_map_obj.AddComponent<EditorData.TileMap>();
                    tile_map_obj.tag = "TileMap";

                    mTilemap.Level = tile_map_data.Level;
                    mTilemap.Init(tile_map_data.Width, tile_map_data.Height);

                    var floor_layer = tile_map_data.FloorLayer;
                    for (int r = 0; r < floor_layer.GetLength(0); ++r)
                    {
                        for (int c = 0; c < floor_layer.GetLength(1); ++c)
                        {
                            var tile = TerrainList.CreateTile((Logic.Tile.EType)floor_layer[r, c]);
                            mTilemap.SetTile(r, c, tile, ETileMapLayer.Floor);
                        }
                    }

                    foreach (var monster_data in tile_map_data.MonsterDatas)
                    {
                        var monster = MonsterList.CreateTile(monster_data.Id);
                        mTilemap.SetTile((int)monster_data.Pos.Row, (int)monster_data.Pos.Col, monster, ETileMapLayer.Collide);
                    }

					foreach (var portal_data in tile_map_data.PortalDatas)
					{
						var portal = PortalList.CreateTile(portal_data.PortalType, portal_data.DestinationLevel, portal_data.DestinationPosition);
						mTilemap.SetTile((int)portal_data.Pos.Row, (int)portal_data.Pos.Col, portal, ETileMapLayer.Collide);
					}

                    foreach (var item_data in tile_map_data.ItemDatas)
                    {
                        var item = ItemList.CreateTile(item_data.Id);
                        mTilemap.SetTile((int)item_data.Pos.Row, (int)item_data.Pos.Col, item, ETileMapLayer.Collide);
                    }

                    foreach (var door_data in tile_map_data.DoorDatas)
                    {
                        var door = DoorList.CreateTile(door_data.DoorType);
                        mTilemap.SetTile((int)door_data.Pos.Row, (int)door_data.Pos.Col, door, ETileMapLayer.Collide);
                    }
                }

                return;
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Level");
            mTilemap.Level = (uint)EditorGUILayout.IntField((int)mTilemap.Level);
            EditorGUILayout.EndHorizontal();

            mEditMode = (EEditMode)EditorGUILayout.EnumPopup("Edit Mode", mEditMode);
            if (mEditMode == EEditMode.Modify || mEditMode == EEditMode.Erase)
            {
                mEditRange = EditorGUILayout.IntSlider("Range", mEditRange, 1, 4);
            }

            mEditLayer = (ETileMapLayer)EditorGUILayout.EnumPopup("Edit Layer", mEditLayer);

            EditorGUILayout.LabelField("Terrain List", EditorStyles.boldLabel);
            if (mTerrainList.Draw())
                Repaint();

            EditorGUILayout.LabelField("Portal List", EditorStyles.boldLabel);
            if (mPortalList.Draw())
                Repaint();

            EditorGUILayout.LabelField("Monster List", EditorStyles.boldLabel);
            if (mMonsterList.Draw())
                Repaint();

            EditorGUILayout.LabelField("Item List", EditorStyles.boldLabel);
            if (mItemList.Draw())
                Repaint();

            EditorGUILayout.LabelField("Door List", EditorStyles.boldLabel);
            if (mDoorList.Draw())
                Repaint();

            if (GUILayout.Button("Save"))
            {
                mTilemap.Save(string.Format("Assets/Resources/level{0}.bytes", mTilemap.Level));
            }
        }
    } 
}