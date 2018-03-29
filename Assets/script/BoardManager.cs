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
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
public class BoardManager : MonoBehaviour
{
    struct loopfactor
    {
        public int loopstart;
        public int loopdelta;
        public loopfactor(int start, int delta)
        {
            loopstart = start;
            loopdelta = delta;
        }
    }
    public GameObject cell;
    int size = 4;
    public ArrayList emptypos = new ArrayList();
    Dictionary<Vector2, CellController> currentCells = new Dictionary<Vector2, CellController>();
    // Use this for initialization
    void Start()
    {
        setupFirstCell();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Horizontal"))
        {
            int hinput = (int)Input.GetAxisRaw("Horizontal");
            if (hinput == 0)
            {
                return;
            }
            Debug.Log("hinput" + hinput);
            clearMergeFlag();
            Move(new Vector2(hinput, 0));
        }
        else if (Input.GetButtonDown("Vertical"))
        {
            int vinput = (int)Input.GetAxisRaw("Vertical");
            if (vinput == 0)
            {
                return;
            }
            Debug.Log("vinput" + vinput);
            clearMergeFlag();
            Move(new Vector2(0, vinput));
        }
        if (checkGameOver())
        {
            GameManager.instance.gameOver();
            enabled = false;
        }
    }
    void prepareLoop(out loopfactor xfactor, out loopfactor yfactor, Vector2 direction)
    {
        if (direction == Vector2.up)
        {
            xfactor = new loopfactor(0, 1);
            yfactor = new loopfactor(size - 1, -1);
        }
        else if (direction == Vector2.down)
        {
            xfactor = new loopfactor(0, 1);
            yfactor = new loopfactor(0, 1);
        }
        else if (direction == Vector2.right)
        {
            xfactor = new loopfactor(size - 1, -1);
            yfactor = new loopfactor(0, 1);
        }
        else if (direction == Vector2.left)
        {
            xfactor = new loopfactor(0, 1);
            yfactor = new loopfactor(0, 1);
        }
        else
        {
            xfactor = new loopfactor(0, 1);
            yfactor = new loopfactor(0, 1);
        }
    }
    void Move(Vector2 direction)
    {
        bool moved = false;
        loopfactor xfactor, yfactor;
        prepareLoop(out xfactor, out yfactor, direction);
        for (int x = xfactor.loopstart; x < size && x >= 0; x += xfactor.loopdelta)
        {
            for (int y = yfactor.loopstart; y < size && y >= 0; y += yfactor.loopdelta)
            {
                Vector2 pos = new Vector2(x, y);
                if (!currentCells.ContainsKey(pos))
                { continue; }
                CellController controller = currentCells[pos];
                Vector2 movetoPos = checkMoveToPos(pos, direction);
                if (tryMergeNextCell(controller, movetoPos, direction))
                {
                    moved = true;
                    currentCells.Remove(pos);
                    emptypos.Add(pos);
                    Destroy(controller.gameObject);
                }
                else if (movetoPos != pos)
                {
                    moved = true;
                    controller.transform.position = movetoPos;
                    currentCells.Remove(pos);
                    currentCells.Add(movetoPos, controller);
                    emptypos.Add(pos);
                    emptypos.Remove(movetoPos);
                }
            }
        }
        if (moved)
        {
            setupNewCell();
        }
    }
    void clearMergeFlag()
    {
        foreach (var item in currentCells.Values)
        {
            item.merged = false;
        }
    }
    /// <summary>
    /// 在指定位置和方向，检查能否与下一个cell合并
    /// 这个pos 通过moveto获得，确保下个位置只能是出界或者被占用，不会是空的
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="direction"></param>
    /// <returns>合并时返回true，否则false</returns>
    bool tryMergeNextCell(CellController controller, Vector2 pos, Vector2 direction)
    {
        Vector2 nextpos = pos + direction;
        if (nextpos.x >= size || nextpos.x < 0 || nextpos.y >= size || nextpos.y < 0)
        {
            return false;
        }
        CellController nextcontroller = currentCells[nextpos];
        if (nextcontroller.Cellnum == controller.Cellnum && !nextcontroller.merged)
        {
            nextcontroller.Cellnum = nextcontroller.Cellnum * 2;
            nextcontroller.merged = true;
            if (nextcontroller.Cellnum == 2048)
            {
                GameManager.instance.gameSuccess();
            }
            return true;
        }
        return false;
    }
    /// <summary>
    /// 在给定方向上，检查可以移动到的位置,
    /// 如果下一个位置出界或者有东西返回当前位置，否则继续迭代至最后一个可选位置
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    Vector2 checkMoveToPos(Vector2 pos, Vector2 direction)
    {
        Vector2 nextpos = pos + direction;
        if (nextpos.x >= size || nextpos.x < 0 || nextpos.y >= size || nextpos.y < 0)
        {
            return pos;
        }
        else if (currentCells.ContainsKey(nextpos))
        {
            return pos;
        }
        else
        {
            return checkMoveToPos(nextpos, direction);
        }
    }
    /// <summary>
    /// 新生成的cell是2或者4, 2占9成
    /// </summary>
    /// <returns></returns>
    int getNewCellNum()
    {
        return Random.Range(0, 1.0f) > 0.9f ? 4 : 2;
    }
    /// <summary>
    /// 新增一个cell
    /// </summary>
    void setupNewCell()
    {
        int index = Random.Range(0, emptypos.Count);
        Vector2 cellpos = (Vector2)emptypos[index];
        emptypos.RemoveAt(index);
        GameObject newcell = Instantiate(cell, cellpos, Quaternion.identity) as GameObject;
        newcell.transform.SetParent(transform);
        CellController controller = newcell.GetComponent<CellController>();
        currentCells.Add(cellpos, controller);
        int num = getNewCellNum();
        controller.Cellnum = num;
    }
    /// <summary>
    /// 设置开场的两个cell
    /// </summary>
    void setupFirstCell()
    {
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                emptypos.Add(new Vector2(x, y));
            }
        }
        for (int i = 0; i < 2; i++)
        {
            setupNewCell();
        }
    }
    /// <summary>
    /// 在没有空位，且任何一个点的值与所有邻居都不同的时候，gameover
    /// </summary>
    /// <returns></returns>
    bool checkGameOver()
    {
        if (emptypos.Count > 0)
        {
            return false;
        }
        Vector2[] alldirection = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
        foreach (var item in currentCells.Keys)
        {
            CellController controller = currentCells[item];
            foreach (var direction in alldirection)
            {
                Vector2 neighbourPos = item + direction;
                CellController neighbourController;
                if (currentCells.TryGetValue(neighbourPos, out neighbourController))
                {
                    if (neighbourController.Cellnum == controller.Cellnum)
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }
}
