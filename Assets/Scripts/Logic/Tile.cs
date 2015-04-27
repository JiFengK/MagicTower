using UnityEngine;
using System.Collections;
using SafeCoroutine;

namespace MagicTower.Logic
{
    /// <summary>
    /// tile����
    /// </summary>
    public class Tile
    {
        /// <summary>
        /// display�ӿ�
        /// </summary>
        protected Display.ITileDisplay mDisplay;
        public Display.ITileDisplay Display
        {
            get { return mDisplay; }
        }

        /// <summary>
        /// tile������
        /// </summary>
        public enum EType
        {
            None = 0,
            Floor = 1,
            Wall = 2,
            Water = 3,
            Sky = 4,
            Monster = 5,
            Npc = 6,
            Item = 7,
            Trigger = 8,
            Door = 9,
            Portal = 10,
            Player = 11,
        }

        protected EType mType;
        public EType Type
        {
            get { return mType; }
        }

        /// <summary>
        /// ���ڵ�tile map
        /// </summary>
        protected TileMap mParent = null;
        public TileMap Parent
        {
            get { return mParent; }
            set { mParent = value; }
        }

        /// <summary>
        /// ���ڵ�tile layer
        /// </summary>
        protected TileLayer mLayer;
        public TileLayer Layer
        {
            get { return mLayer; }
            set { mLayer = value; }
        }

        public Tile(EType type)
        {
            mType = type;
        }

        /// <summary>
        /// ��ǰ���߼�λ��
        /// </summary>
        protected TilePosition mPosition = new TilePosition(0, 0);
        public TilePosition Position
        {
            get { return mPosition; }
            set { mPosition = value; }
        }

        /// <summary>
        /// �Ƿ���赲�ƶ�
        /// </summary>
        protected bool mIsBlock = true;
        public bool IsBlock
        {
            get { return mIsBlock; }
            set { mIsBlock = value; }
        }

        /// <summary>
        /// �ж��Ƿ�����target�ƶ�����
        /// </summary>
        /// <param name="target"></param>
        /// <returns>����ͷ���true</returns>
        public virtual bool ValidateMove(Tile target)
        {
            return !mIsBlock;
        }

        /// <summary>
        /// ��ʼ��target��ײ
        /// </summary>
        /// <param name="target"></param>
        /// <returns>��ײ�ɹ�����true</returns>
        public virtual IEnumerator BeginTrigger(Tile target)
        {
            yield return true;
        }

        /// <summary>
        /// ��ײ����
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public virtual IEnumerator EndTrigger(Tile target)
        {
            yield return null;
        }

        /// <summary>
        /// �ƶ�tile��ָ��λ��
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public virtual IEnumerator MoveTo(uint row, uint col)
        {
            yield return MoveTo(new TilePosition(row, col));
        }

        public virtual IEnumerator MoveTo(TilePosition destination)
        {
            yield return mDisplay.MoveTo(destination);

            mLayer[Position] = null;
            mLayer[destination] = this;

            yield return null;
        }

        /// <summary>
        /// �״ν��볡��
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerator Enter()
        {
            if (mDisplay == null)
            {
                mDisplay = Game.Instance.DisplayFactory.GetTileDisplay(this);
            }

            yield return mDisplay.Enter();
        }

        /// <summary>
        /// �뿪����
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerator Exit()
        {
            mLayer[mPosition] = null;

            yield return mDisplay.Exit();

            mDisplay = null;
        }

        /// <summary>
        /// ��ǰ�Ƿ����ƶ���
        /// </summary>
        private bool mIsMoving = false;
        public bool IsMoving
        {
            get { return mIsMoving; }
            set { mIsMoving = value; }
        }

        public float Speed
        {
            get { return 1.0f; }
        }
    }
}

