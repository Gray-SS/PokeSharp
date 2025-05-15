using Microsoft.Xna.Framework;
using Pokemon.DesktopGL.Creatures;
using PokeSharp.Core;

namespace Pokemon.DesktopGL.Miscellaneous;

public static class Utils
{
    private static readonly float[,] _effectivenessChart = new float[18, 18]
    {
        //                 NOR FIR WAT ELE GRA ICE FIG POI GRO FLY PSY BUG ROC GHO DRA DAR STE FAI
        /* Normal  */  { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 0.5f, 0f, 1f, 1f, 0.5f, 1f },
        /* Fire    */  { 1f, 0.5f, 0.5f, 1f, 2f, 2f, 1f, 1f, 1f, 1f, 1f, 2f, 0.5f, 1f, 0.5f, 1f, 2f, 1f },
        /* Water   */  { 1f, 2f, 0.5f, 1f, 0.5f, 1f, 1f, 1f, 2f, 1f, 1f, 1f, 2f, 1f, 0.5f, 1f, 1f, 1f },
        /* Electric*/  { 1f, 1f, 2f, 0.5f, 0.5f, 1f, 1f, 1f, 0f, 2f, 1f, 1f, 1f, 1f, 0.5f, 1f, 1f, 1f },
        /* Grass   */  { 1f, 0.5f, 2f, 1f, 0.5f, 1f, 1f, 0.5f, 2f, 0.5f, 1f, 0.5f, 2f, 1f, 0.5f, 1f, 0.5f, 1f },
        /* Ice     */  { 1f, 0.5f, 0.5f, 1f, 2f, 0.5f, 1f, 1f, 2f, 2f, 1f, 1f, 1f, 1f, 2f, 1f, 0.5f, 1f },
        /* Fighting*/  { 2f, 1f, 1f, 1f, 1f, 2f, 1f, 0.5f, 1f, 0.5f, 0.5f, 0.5f, 2f, 0f, 1f, 2f, 2f, 0.5f },
        /* Poison  */  { 1f, 1f, 1f, 1f, 2f, 1f, 1f, 0.5f, 0.5f, 1f, 1f, 1f, 0.5f, 0.5f, 1f, 1f, 0f, 2f },
        /* Ground  */  { 1f, 2f, 1f, 2f, 0.5f, 1f, 1f, 2f, 1f, 0f, 1f, 0.5f, 2f, 1f, 1f, 1f, 2f, 1f },
        /* Flying  */  { 1f, 1f, 1f, 0.5f, 2f, 1f, 2f, 1f, 1f, 1f, 1f, 2f, 0.5f, 1f, 1f, 1f, 0.5f, 1f },
        /* Psychic */  { 1f, 1f, 1f, 1f, 1f, 1f, 2f, 2f, 1f, 1f, 0.5f, 1f, 1f, 1f, 1f, 0f, 0.5f, 1f },
        /* Bug     */  { 1f, 0.5f, 1f, 1f, 2f, 1f, 0.5f, 0.5f, 1f, 0.5f, 2f, 1f, 1f, 0.5f, 1f, 2f, 0.5f, 0.5f },
        /* Rock    */  { 1f, 2f, 1f, 1f, 1f, 2f, 0.5f, 1f, 0.5f, 2f, 1f, 2f, 1f, 1f, 1f, 1f, 0.5f, 1f },
        /* Ghost   */  { 0f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 1f, 1f, 2f, 1f, 0.5f, 1f, 1f },
        /* Dragon  */  { 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 1f, 0.5f, 0f },
        /* Dark    */  { 1f, 1f, 1f, 1f, 1f, 1f, 0.5f, 1f, 1f, 1f, 2f, 1f, 1f, 2f, 1f, 0.5f, 1f, 0.5f },
        /* Steel   */  { 1f, 0.5f, 0.5f, 0.5f, 1f, 2f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 1f, 1f, 1f, 0.5f, 2f },
        /* Fairy   */  { 1f, 0.5f, 1f, 1f, 1f, 1f, 2f, 0.5f, 1f, 1f, 1f, 1f, 1f, 1f, 2f, 2f, 0.5f, 1f },
    };

    public static (int Col, int Row) ConvertWorldPosToMapCoord(Vector2 worldPos)
    {
        int col = (int)(worldPos.X / GameConstants.TileSize);
        int row = (int)(worldPos.Y / GameConstants.TileSize);

        return (col, row);
    }

    public static (int Col, int Row) ConvertMapPosToTileCoord(Vector2 mapPosition)
    {
        var map = PokemonGame.Instance.ActiveWorld.Map.TiledMap;
        int col = (int)(mapPosition.X / map.TileWidth);
        int row = (int)(mapPosition.Y / map.TileHeight);

        return (col, row);
    }

    public static Vector2 ConvertTileCoordToWorldPos((int Col, int Row) coord)
        => new Vector2(coord.Col, coord.Row) * GameConstants.TileSize;

    public static Vector2 ConvertMapPosToWorldPos(Vector2 mapPosition)
        => ConvertTileCoordToWorldPos(ConvertMapPosToTileCoord(mapPosition));

    public static float GetTypeEffectiveness(CreatureType attacker, CreatureType defenderType1, CreatureType? defenderType2 = null)
    {
        float effectiveness1 = _effectivenessChart[(int)attacker, (int)defenderType1];
        float effectiveness2 = 1.0f;
        if (defenderType2.HasValue)
            effectiveness2 = _effectivenessChart[(int)attacker, (int)defenderType2];

        return effectiveness1 * effectiveness2;
    }
}