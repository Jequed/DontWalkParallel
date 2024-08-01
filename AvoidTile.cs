using System;

public class AvoidTile : Tile
{
    protected override void OnPlayerSteppedOn()
    {
        GlobalData.player.AddPhobia(new PhobiaBadTile(GlobalData.player, null));
    }
}