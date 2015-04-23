﻿using UnityEngine;
using MagicTower;
using Utils;
using System.Collections;

public class Main : MonoBehaviour 
{
	public GameObject TileMap;
	public GameObject Player;

	// Use this for initialization
	void Start () 
	{
        SpriteSheetManager.Instance.Load("3");
        SpriteSheetManager.Instance.Load("monster");
        SpriteSheetManager.Instance.Load("wall");

		Logger.LogInfo(CSVManager.Instance["monster"][2]["name"]);

        EventQueue.Instance.AddEvent(EEventType.ENTER_GAME);
    }

    //private void AddMonster(int r, int c, uint id)
    //{
    //    var monster_prefab = Resources.Load ("Prefabs/Monster_" + id) as GameObject;
    //    var monster = (GameObject)GameObject.Instantiate (monster_prefab);
    //    monster.transform.parent = TileMap.transform;
    //    monster.transform.localPosition = new Vector3 (c, r, 0);
    //}
	
	// Update is called once per frame
	void Update () 
    {
        if (Input.GetKeyDown(KeyCode.W))
        {

        }

        EventQueue.Instance.Update(Time.deltaTime);
        SafeCoroutine.CoroutineManager.Update(Time.deltaTime);
	}
}
