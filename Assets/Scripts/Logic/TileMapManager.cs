using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Utils;

namespace MagicTower.Logic
{
	public class TileMapManager : Singleton<TileMapManager>
	{
		private Dictionary<uint, TileMap> mMaps;
	
        //public TileMap GetMap(uint lv)
        //{
        //    TileMap map = null;
        //    if (mMaps.TryGetValue(lv, out map))
        //        return map;
        //    else
        //        return LoadMap(lv);
        //}
	
		public bool HasLoaded(uint lv)
		{
			TileMap map = null;
			if (mMaps.TryGetValue (lv, out map))
				return true;
			
			return false;
		}
	
		public TileMapManager ()
		{
			mMaps = new Dictionary<uint, TileMap> ();
		}
	
		private int parseMapSize(string[] content, int cur, out uint width, out uint height)
		{
			++cur;
	
			var tmp = content[cur++].Split (new char[] {','});
			width = Convert.ToUInt32 (tmp [0]);
			height = Convert.ToUInt32 (tmp [1]);
	
			return cur;
		}
	
		private int parseLayer(string[] content, int cur, TileLayer layer)
		{
			++cur;
	
			var width = layer.Width;
			var height = layer.Height;
	
			for (int r = 0; r < height; ++r, ++cur) {
				for (int c = 0; c < width; ++c) {
					var tile_char = content[cur][c];
					var tile = TileFactory.Instance.CreateTile(tile_char);
					if (tile != null) {
						layer[(uint)(height - r - 1), (uint)c] = tile;
					}
				}
			}
	
			return cur;
		}
	
		private int parseMonter(string[] content, int cur, TileLayer layer)
		{
			++cur;
	
			int count = Convert.ToInt32 (content [cur++]);
			for (int i = 0; i < count; ++i, ++cur) {
                var monster_info = content[cur].Split(new char[] { ',' });
                var row = Convert.ToUInt32(monster_info[0]);
                var col = Convert.ToUInt32(monster_info[1]);
                var monster_id = Convert.ToUInt32(monster_info[2]);

                var monster_tile = TileFactory.Instance.CreateMonster(monster_id);
                layer[row, col] = monster_tile;
			}
	
			return cur;
		}
	
		private int parseNpc(string[] content, int cur, TileLayer layer)
		{
			++cur;
			
			int count = Convert.ToInt32 (content [cur++]);
			for (int i = 0; i < count; ++i, ++cur) {
                var npc_info = content[cur].Split(new char[] { ',' });
                var row = Convert.ToUInt32(npc_info[0]);
                var col = Convert.ToUInt32(npc_info[1]);
                var npc_id = Convert.ToUInt32(npc_info[2]);

                var npc_tile = TileFactory.Instance.CreateMonster(npc_id);
                layer[row, col] = npc_tile;
			}
			
			return cur;
		}
	
		private int parseItem(string[] content, int cur, TileLayer layer)
		{
			++cur;
			
			int count = Convert.ToInt32 (content [cur++]);
			for (int i = 0; i < count; ++i, ++cur) {
                var item_info = content[cur].Split(new char[] { ',' });
                var row = Convert.ToUInt32(item_info[0]);
                var col = Convert.ToUInt32(item_info[1]);
                var item_id = Convert.ToUInt32(item_info[2]);

                var item_tile = TileFactory.Instance.CreateMonster(item_id);
                layer[row, col] = item_tile;
			}
			
			return cur;
		}
	
		private int parsePortal(string[] content, int cur, TileLayer layer)
		{
			++cur;
			
			int count = Convert.ToInt32 (content [cur++]);
			for (int i = 0; i < count; ++i, ++cur) {
                var portal_info = content[cur].Split(new char[] { ',' });
                var row = Convert.ToUInt32(portal_info[0]);
                var col = Convert.ToUInt32(portal_info[1]);
                var direction = portal_info[2].Equals("up") ? EProtalDirection.Up : EProtalDirection.Down;
                var destination_level = Convert.ToUInt32(portal_info[3]);
                var destination_row = Convert.ToUInt32(portal_info[4]);
                var destination_col = Convert.ToUInt32(portal_info[5]);

                var portal_tile = TileFactory.Instance.CreatePortal();
                portal_tile.Direction = direction;
                portal_tile.DestinationLevel = destination_level;
                portal_tile.DestinationPosition = new TilePosition(destination_row, destination_col);

                layer[row, col] = portal_tile;
			}
			
			return cur;
		}

        public IEnumerator LoadMap(uint lv)
        {
            var async = Resources.LoadAsync("level" + lv);
            yield return async;

            var asset = async.asset as TextAsset;

            Data.TileMapData tile_map_data = null;
            using (var stream = new MemoryStream(asset.bytes))
            {
                var formatter = new BinaryFormatter();
                tile_map_data = formatter.Deserialize(stream) as Data.TileMapData;
            }

            var map = new TileMap();
            map.Init(tile_map_data);

            var floor_layer_data = tile_map_data.FloorLayer;
            for (int row = 0; row < floor_layer_data.GetLength(0); ++row)
            {
                for (int col = 0; col < floor_layer_data.GetLength(1); ++col)
                {
                    var tile_type = (Tile.EType)floor_layer_data[row, col];
                    var tile = TileFactory.Instance.CreateTile(tile_type);
                    if (tile != null)
                    {
                        map.LayerFloor[(uint)(map.Height - row - 1), (uint)col] = tile;
                    }
                }
            }

            var portals = tile_map_data.PortalDatas;
            foreach (var portal in portals)
            {
                var portal_tile = TileFactory.Instance.CreatePortal(portal);
                map.LayerCollide[portal.Row, portal.Col] = portal_tile;
            }

            var monsters = tile_map_data.MonsterDatas;
            foreach (var monster in monsters)
            {
                var row = monster.Row;
                var col = monster.Col;
                var monster_id = monster.Id;

                var monster_tile = TileFactory.Instance.CreateMonster(monster_id);
                map.LayerCollide[row, col] = monster_tile;
            }
                
            yield return map;
        }
	
        //public IEnumerator LoadMap(uint lv)
        //{
        //    var asset = Resources.Load ("level" + lv) as TextAsset;
        //    var content = asset.text.Split (new char[] {'\n'}, StringSplitOptions.RemoveEmptyEntries);
	
        //    int cur_idx = 0;
	
        //    // map size
        //    uint width, height;
        //    cur_idx = parseMapSize (content, cur_idx, out width, out height);

        //    var map = new TileMap();
        //    map.Init (width, height);
	
        //    // layer 0
        //    cur_idx = parseLayer (content, cur_idx, map.LayerFloor);
	
        //    // layer 1
        //    cur_idx = parseLayer (content, cur_idx, map.LayerCollide);
	
        //    // monster
        //    cur_idx = parseMonter (content, cur_idx, map.LayerCollide);
	
        //    // npc
        //    cur_idx = parseNpc (content, cur_idx, map.LayerCollide);
	
        //    // item
        //    cur_idx = parseItem (content, cur_idx, map.LayerCollide);
	
        //    // portal
        //    cur_idx = parsePortal (content, cur_idx, map.LayerFloor);
	
        //    yield return map;
        //}
	}
}


