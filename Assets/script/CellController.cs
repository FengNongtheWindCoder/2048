/*********************************************************************************
 *Copyright(C) 2015 by #AUTHOR#
 *All rights reserved.
 *FileName:     #SCRIPTFULLNAME#
 *Author:       #AUTHOR#
 *Version:      #VERSION#
 *UnityVersion：#UNITYVERSION#
 *Date:         #DATE#
 *Description:   
 *History:  
**********************************************************************************/
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CellController : MonoBehaviour
{
    int cellnum = 2;
    public Text cellnumtext;
    public bool merged = false;
    public int Cellnum
    {
        set { cellnum = value; cellnumtext.text = cellnum.ToString(); }
        get { return cellnum; }
    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
