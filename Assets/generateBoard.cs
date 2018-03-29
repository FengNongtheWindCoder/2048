/*********************************************************************************
 *Copyright(C) 2015 by #AUTHOR#
 *All rights reserved.
 *FileName:     #SCRIPTFULLNAME#
 *Author:       #AUTHOR#
 *Version:      #VERSION#
 *UnityVersion：#UNITYVERSION#
 *Date:         #DATE#
 *Description:   功能脚本，生成一个完整的背景cell，然后保存成prefab，不是游戏运行时候用的
 *History:  
**********************************************************************************/
using UnityEngine;
using System.Collections;

public class NewBehaviourScript : MonoBehaviour {
    public GameObject gridcell;
    public Transform parent;
	// Use this for initialization
	void Start () {
        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                GameObject newcell = Instantiate(gridcell, new Vector3(x, y, 0), Quaternion.identity) as GameObject;
                newcell.name = "grid-" + newcell.transform.position;
                newcell.transform.SetParent(parent);
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
