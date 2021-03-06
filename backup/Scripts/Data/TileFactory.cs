//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1026
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using UnityEngine;

namespace mt
{
    public class TileFactory : Singleton<TileFactory>
    {
        public TileFactory()
        {
            
        }

        public Tile CreatePlayer()
        {
            return new Tile_Player();
        }

        public Tile CreateMonster(uint id)
        {
            return new Tile_Monster(id);
        }

        public Tile CreateTile(char tile_type)
        {
            if (tile_type == '1')
            {
                return new Tile(Tile.EType.Floor);
            }
            else if (tile_type == '2')
            {
                return new Tile(Tile.EType.Wall);
            }
            else if (tile_type == '3')
            {
                return new Tile(Tile.EType.Water);
            }
            else if (tile_type == '4')
            {
                return new Tile(Tile.EType.Sky);
            }
            else
            {
                return null;
            }
        }
    }
}

