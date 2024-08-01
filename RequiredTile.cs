using UnityEngine;

public class RequiredTile : Tile
{
    [SerializeField]
    private GameObject notSteppedOnGraphic;

    [SerializeField]
    private GameObject steppedOnGraphic;

    private bool steppedOn = false;

    public bool SteppedOn
    {
        get
        {
            return steppedOn;
        }
        private set
        {
            steppedOn = value;

            steppedOnGraphic.SetActive(value);
            notSteppedOnGraphic.SetActive(!value);

            CheckTiles();
        }
    }

    protected override void GameStart()
    {
        base.GameStart();

        SteppedOn = false;
    }

    protected override void OnPlayerSteppedOn()
    {
        if (!SteppedOn)
            SteppedOn = true;
    }

    private void CheckTiles()
    {
        var tiles = levelContainer.GetComponentsInChildren<RequiredTile>();

        int tilesSteppedOn = 0;
        foreach (var tile in tiles)
        {
            if (tile.SteppedOn)
                tilesSteppedOn++;
        }

        if (tilesSteppedOn == tiles.Length)
            OnComplete();
    }

    private void OnComplete()
    {
        Debug.Log("Stepped on all tiles");
    }
}