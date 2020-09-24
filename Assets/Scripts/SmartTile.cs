using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SmartTile : Tile
{

    private bool up;
    private bool down;
    private bool right;
    private bool left;
    public Sprite[] m_Sprites;
    /* 
     * 0 = standalone_tile  (all free)
     * 1 = standalone_left  (right used)
     * 2 = standalone_right (left used)
     * 3 = dirt_center  (all used)
     * 4 = dirt_bottom_right  (left and top used)
     * 5 = dirt_bottom_left  (right and top used)
     * 6 = dirt_left  (top, right, bottom used)
     * 7 = dirt_right  (top, left, bottom used)
     * 8 = dirt_bottom (top, right, left used)
     * 9 = grass_center (bottom used)
     *10 = grass_left (bottom, right used)
     *11 = grass_right (bottom, left used)
     *12 = grass_solo_center (right, left used)
     */
    public Sprite m_Preview;
    // This refreshes itself and other RoadTiles that are orthogonally and diagonally adjacent
    public override void RefreshTile(Vector3Int location, ITilemap tilemap)
    {
        for (int yd = -1; yd <= 1; yd++)
            for (int xd = -1; xd <= 1; xd++)
            {
                Vector3Int position = new Vector3Int(location.x + xd, location.y + yd, location.z);
                if (HasTile(tilemap, position))
                    tilemap.RefreshTile(position);
            }
    }
    // This determines which sprite is used based on the Tiles that are adjacent to it and rotates it to fit the other tiles.
    // As the rotation is determined by the Tile, the TileFlags.OverrideTransform is set for the tile.
    public override void GetTileData(Vector3Int location, ITilemap tilemap, ref TileData tileData)
    {
        up = false;
        down = false;
        right = false;
        left = false;

        // bool-flags raised if tile got adjacent tiles on eighter side
        if (HasTile(tilemap, location + new Vector3Int(0, 1, 0)))
            up = true;

        if (HasTile(tilemap, location + new Vector3Int(0, -1, 0)))
            down = true;
            
        if (HasTile(tilemap, location + new Vector3Int(1, 0, 0)))
            right = true;
        
        if (HasTile(tilemap, location + new Vector3Int(-1, 0, 0)))
            left = true;
        
        int index = GetIndex(up, down, right, left);

        if (index >= 0 && index < m_Sprites.Length)
        {
            tileData.sprite = m_Sprites[index];
            tileData.color = Color.white;
            var m = tileData.transform;
            m.SetTRS(Vector3.zero, Quaternion.Euler(0f, 0f, 0f), Vector3.one);
            tileData.transform = m;
            tileData.flags = TileFlags.LockTransform;
            tileData.colliderType = ColliderType.Sprite;

        }
        else
        {
            Debug.LogWarning("Not enough sprites in SmartTile instance");
        }
    }
    // This determines if the Tile at the position is the same SmartTile.
    private bool HasTile(ITilemap tilemap, Vector3Int position)
    {
        return tilemap.GetTile(position) == this;
    }
    // The following determines which sprite to use based on the number of adjacent SmartTiles
    private int GetIndex(bool up, bool down, bool right, bool left)
    {
        if (!up && !down && !right && !left)
            return 0;

        else if (!up && !down && right && !left)
            return 1;

        else if (!up && !down && !right && left)
            return 2;

        else if (up && down && right && left)
            return 3;

        else if (up && !down && !right && left)
            return 4;

    	else if (up && !down && right && !left)
            return 5;
        
        else if (up && down && right && !left)
            return 6;
        
        else if (up && down && !right && left)
            return 7;
        
        else if (up && !down && right && left)
            return 8;
        
        else if (!up && down && !right && !left)
            return 9;

        else if (!up && down && right && !left)
            return 10;

        else if (!up && down && !right && left)
            return 11;

        else if (!up && !down && right && left)
            return 12;

        else if (up && !down && !right && !left)
            return 8;

        else if (!up && down && right && left)
            return 9;

        else if (up && down && !right && !left)
            return 3;

        else
            return -1;
    }
    
#if UNITY_EDITOR
    // The following is a helper that adds a menu item to create a RoadTile Asset
    [MenuItem("Assets/Create/SmartTile")]
    public static void CreateTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Smart Tile", "New Smart Tile", "Asset", "Save Smart Tile", "Assets");
        if (path == "")
            return;
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<SmartTile>(), path);
    }
#endif
}
