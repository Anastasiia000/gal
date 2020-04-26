using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class board : MonoBehaviour
{
    public static board instance;

    private int xSize, ySize;
    private tile tileGo;
    private List<Sprite> tileSprite = new List<Sprite>();

    private void Awake()
    {
        instance = this;
    }
    public tile[,] SetValue(int xSize, int ySize, tile tileGo, List<Sprite> tileSprite)
    {
        this.xSize = xSize;
        this.ySize = ySize;
        this.tileGo = tileGo;
        this.tileSprite = tileSprite;

        return CreateBoard();
    } 

    private tile[,] CreateBoard()
    {
        tile[,] tileArray = new tile[xSize, ySize];
        float xPos = transform.position.x;
        float yPos = transform.position.y;
        Vector2 tileSize = tileGo.spriteRenderer.bounds.size;

        Sprite cashSprite = null;

        for (int x=0; x<xSize; x++)
        {
            for(int y = 0; y<ySize; y++)
            {
                tile newTile = Instantiate(tileGo, transform.position, Quaternion.identity);
                newTile.transform.position = new Vector3(xPos + (tileSize.x * x), yPos + (tileSize.y * y), 0);
                newTile.transform.parent = transform;

                tileArray[x, y] = newTile;

                List<Sprite> tempSprite = new List<Sprite>();
                tempSprite.AddRange(tileSprite);
                if (x > 0)
                {
                    tempSprite.Remove(tileArray[x-1,y].spriteRenderer.sprite);
                }
                tempSprite.Remove(cashSprite); //удаляем по у (цикл по у)
                newTile.spriteRenderer.sprite = tempSprite[Random.Range(0, tempSprite.Count)];
                cashSprite = newTile.spriteRenderer.sprite;
            }
        }
        return tileArray;
    }
}
