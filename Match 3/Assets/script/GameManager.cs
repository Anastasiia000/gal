using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class BoardSetting
{
    public int xSize, ySize;
    public tile tileGo;
    public List<Sprite> tileSprite;
}

public class GameManager : MonoBehaviour
{
    [Header ("Параметры игровой доски")]
    public BoardSetting boardSetting;
    // Start is called before the first frame update
    void Start()
    {
        BoardController.instace.SetValue(board.instance.SetValue(boardSetting.xSize, boardSetting.ySize, boardSetting.tileGo, boardSetting.tileSprite),
            boardSetting.xSize, boardSetting.ySize,
            boardSetting.tileSprite);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
