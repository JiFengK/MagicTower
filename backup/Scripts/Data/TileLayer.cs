﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mt
{
    public class TileLayer
    {
        private Tile[,] mTiles;
        private TileMap mParent;

        /// <summary>
        /// Gets the width.
        /// </summary>
        /// <value>The width.</value>
        public uint Width
        {
            get { return (uint)mTiles.GetLength(1); }
        }

        /// <summary>
        /// Gets the height.
        /// </summary>
        /// <value>The height.</value>
        public uint Height
        {
            get { return (uint)mTiles.GetLength(0); }
        }

        /// <summary>
        /// Init the specified parent, width and height.
        /// </summary>
        /// <param name="parent">Parent.</param>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        public void Init(TileMap parent, uint width, uint height)
        {
            mParent = parent;
            mTiles = new Tile[height, width];
        }

        /// <summary>
        /// 获取或设置对应位置的tile
        /// </summary>
        /// <param name="r">行坐标</param>
        /// <param name="c">列坐标</param>
        public Tile this[uint r, uint c]
        {
            get { return mTiles[r, c]; }
            set
            {
                mTiles[r, c] = value;

                value.Position = new TilePosition(r, c);
            }
        }
    }
}
