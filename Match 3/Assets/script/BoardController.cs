using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{//нужно было сделать наследование от board

    public static BoardController instace;

    private int xSize, ySize;
    private List<Sprite> tileSprite = new List<Sprite>();
    private tile[,] tileArray;
    
    private tile oldSelectTile;
    private Vector2[] dirRay = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

    private bool isFindMatch = false;
    private bool isShift = false;
    private bool isSearchEmptyTile = false;


    public void SetValue(tile[,] tileArray,int xSize, int ySize, List<Sprite> tileSprite)
    {
        this.tileArray = tileArray;
        this.xSize = xSize;
        this.ySize = ySize;
        this.tileSprite = tileSprite;
    }

    private void Awake()
    {
        instace = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (isSearchEmptyTile)
        {
            SearchEmptyTile();
        }
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D ray = Physics2D.GetRayIntersection
                (Camera.main.ScreenPointToRay(Input.mousePosition));
            if (ray != false)
            {
                CheckSelectTile(ray.collider.gameObject.
                    GetComponent<tile>());
            }
        }
    }
    #region (Выделить тайл, снять выделение с тайла, уплавление выделением)
    private void SelectTile(tile tile)
    {
        tile.isSelected = true;
        tile.spriteRenderer.color = new Color(0.5f, 0.5f, 0.5f);
        oldSelectTile = tile;
    }

    private void DeselectTile(tile tile)
    {
        tile.isSelected = false;
        tile.spriteRenderer.color = new Color(1, 1, 1);
        oldSelectTile = null;
    }

    private void CheckSelectTile(tile tile)
    {
        if (tile.isEmpty||isShift) { return; }
        if (tile.isSelected) { DeselectTile(tile); }
        else {
            //первое выделение тайла
            if (!tile.isSelected && oldSelectTile == null)
            {
                SelectTile(tile);
            }
            //попытка выбрать другой тайл
            else
            {   //если 2й выбраный тайл сосед предыдущего
                if (AdjacentTiles().Contains(tile))
                {                    
                    SwapTwoTiles(tile);
                    FindAllMatch(tile);
                    DeselectTile(oldSelectTile);
                }
                //если мы нажимаем на какое-то другой тайл, он начинаем выделятьсяб забываем старый тайл
                else
                {
                    DeselectTile(oldSelectTile);
                    SelectTile(tile);
                }
            }
        }
    }
    #endregion


    #region(Поиск совпадения, удаление спрайта, поиск совпадений)
    private List<tile> FindMatch (tile tile, Vector2 dir)
    {
        List<tile> cashFindTiles = new List<tile>();
        RaycastHit2D hit = Physics2D.Raycast(tile.transform.position, dir);
        while(hit.collider != null 
            && hit.collider.gameObject.GetComponent<tile>().spriteRenderer.sprite
            == tile.spriteRenderer.sprite)
        {
            cashFindTiles.Add(hit.collider.gameObject.GetComponent<tile>());
            hit = Physics2D.Raycast(hit.collider.gameObject.transform.position, dir);
        }
        return cashFindTiles;
    }

    private void DeleteSprite(tile tile, Vector2[] dirArrray)
    {
        List<tile> cashFindSprite = new List<tile>();
        for (int i=0; i<dirArrray.Length; i++)
        {
            cashFindSprite.AddRange(FindMatch(tile, dirArrray[i]));
        }
        if (cashFindSprite.Count >= 2)
        {
            for (int i = 0; i < cashFindSprite.Count; i++)
            {
                cashFindSprite[i].spriteRenderer.sprite = null;
            }
            isFindMatch = true;
        }
    }

    private void FindAllMatch(tile tile)
    {
        if (tile.isEmpty)
        {
            return;
        }
        DeleteSprite(tile, new Vector2[2] { Vector2.up, Vector2.down });
        DeleteSprite(tile, new Vector2[2] { Vector2.left, Vector2.right });
        if (isFindMatch)
        {
            isFindMatch = false;
            tile.spriteRenderer.sprite = null;
            isSearchEmptyTile = true;
        }
    }
    #endregion

    #region(Смена 2х тайлов, соседние тайлы)

    private void SwapTwoTiles(tile tile)
    {
        if (oldSelectTile.spriteRenderer.sprite == tile.spriteRenderer.sprite) { return; }
        Sprite cashSprite = oldSelectTile.spriteRenderer.sprite;
        oldSelectTile.spriteRenderer.sprite = tile.spriteRenderer.sprite; //меняем спрайты местами
        tile.spriteRenderer.sprite = cashSprite;

        UI.instance.Moves(1);
    }
    
    private List<tile> AdjacentTiles ( )
    {
        List<tile> cashTiles = new List<tile>();
        for (int i=0; i<dirRay.Length; i++)
        {
            RaycastHit2D hit = Physics2D.Raycast(oldSelectTile.transform.position,
                dirRay[i]);
            if (hit.collider != null)
            {
                cashTiles.Add(hit.collider.gameObject.GetComponent<tile>());
            }            
        }
        return cashTiles;
    }
    #endregion

    #region(Поис пустого тайла, сдвиг тайла внизб установить новое изображение)
    private void SearchEmptyTile()
    {
        for (int x=0; x < xSize; x++)
        {
            for (int y=0; y<ySize; y++)
            {
                if (tileArray[x, y].isEmpty)
                {
                    ShiftTileDown(x, y);
                    break;
                }
                if (x == xSize && y == ySize-1)
                {
                    isSearchEmptyTile = false;
                }                
            }
        }
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                FindAllMatch(tileArray[x, y]);
            }
        }
    }
    private void ShiftTileDown(int xPos, int yPos)
    {
        isShift = true;
        List<SpriteRenderer> cashRender = new List<SpriteRenderer>();
        int count = 0;
        for(int y=yPos;y<ySize; y++)
        {
            tile tile = tileArray[xPos, y];
            if (tile.isEmpty)
            {
                count++;
            }
            cashRender.Add(tile.spriteRenderer);
            
        }
        for (int i = 0; i < count; i++)
        {
            UI.instance.Score(50);
            SetNewSprite(xPos, cashRender);
        }
        isShift = false;


    }
    private void SetNewSprite(int xPos, List<SpriteRenderer> render)
    {
        for(int y = 0; y<render.Count-1; y++)
        {
            render[y].sprite = render[y + 1].sprite;
            render[y + 1].sprite = GetNewSprite(xPos, ySize - 1);
        }
    }
    private Sprite GetNewSprite(int xPos, int yPos)
    {
        List<Sprite> cashSprite = new List<Sprite>();
        cashSprite.AddRange(tileSprite);

        if (xPos > 0)
        {
            cashSprite.Remove(tileArray[xPos - 1, yPos].spriteRenderer.sprite);
        }
        if (xPos < xSize - 1)
        {
            cashSprite.Remove(tileArray[xPos + 1, yPos].spriteRenderer.sprite);
        }
        if (yPos > 0)
        {
            cashSprite.Remove(tileArray[xPos, yPos - 1].spriteRenderer.sprite);
        }
        return cashSprite[Random.Range(0, cashSprite.Count)];
    }
    #endregion
}
