using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Utils
{
    public static class TilemapExtension
    {
        public static int GetTilesCount(this Tilemap tm)
        {
            int count = 0;
            var bounds = tm.cellBounds;
            var tilePosition = new Vector3Int();
            for (int i = bounds.xMin; i < bounds.xMax; ++i)
            {
                for (int j = bounds.yMin; j < bounds.yMax; j++)
                {
                    for (int k = bounds.zMin; k < bounds.zMax; k++)
                    {
                        tilePosition.x = i;
                        tilePosition.y = j;
                        tilePosition.z = k;
                        if (tm.HasTile(tilePosition))
                        {
                            count++;
                        }
                    }
                }
            }

            return count;
        }
        
        public static IEnumerable<Vector3Int> GetTilePositions(this Tilemap tm)
        {
            var bounds = tm.cellBounds;
            for (int i = bounds.xMin; i < bounds.xMax; i++)
            {
                for (int j = bounds.yMin; j < bounds.yMax; j++)
                {
                    for (int k = bounds.zMin; k < bounds.zMax; k++)
                    {
                        Vector3Int tilepos = new Vector3Int(i, j, k);
                        if (tm.HasTile(tilepos))
                        {
                            yield return tilepos;
                        }
                    }
                }    
            }
        }
    
        public static IEnumerable<Vector3> GetTileWorldPositions(this Tilemap tm)
        {
            var tilespos = tm.GetTilePositions();
            foreach (var tp in tilespos)
            {
                yield return tm.GetCellCenterWorld(tp);
            }
        }
    
        public static IEnumerable<TileBase> GetTiles(this Tilemap tm)
        {
            var tilespos = tm.GetTilePositions();
            foreach (var tp in tilespos)
            {
                yield return tm.GetTile(tp);
            }
        }

        /// <summary>
        /// Obtener las posiciones del borde del tilemap. Funciona correctamente solo con tilemaps rectangulares.
        /// </summary>
        /// <returns></returns>
        public static List<Vector3Int> GetEdgePositions(this Tilemap tm)
        {
            List<Vector3Int> edgePos = new List<Vector3Int>();
            var bounds = tm.cellBounds;
            for (int i = bounds.xMin; i < bounds.xMax; i++)  // iterar las columnas
            {
                for (int k = bounds.zMin; k < bounds.zMax; k++)
                {
                    int minRow = bounds.yMax;
                    int maxRow = bounds.yMin;
                    bool existsTile = false;
                    Vector3Int vtemp = Vector3Int.zero;
                    for (int j = bounds.yMin; j < bounds.yMax; j++)
                    {
                        vtemp.x = i; vtemp.y = j; vtemp.z = k;
                        if (tm.HasTile(vtemp))
                        {
                            maxRow = j;
                            if (j < minRow)
                                minRow = j;
                            existsTile = true;
                        }
                    }
                    if (existsTile)
                    {
                        edgePos.Add(new Vector3Int(i, minRow - 1, k));
                        edgePos.Add(new Vector3Int(i, maxRow + 1, k));
                    }
                }
            }
        
            for (int j = bounds.yMin; j < bounds.yMax; j++)  // iterar las filas
            {
                for (int k = bounds.zMin; k < bounds.zMax; k++)
                {
                    int minCol = bounds.xMax;
                    int maxCol = bounds.xMin;
                    bool existsTile = false;
                    Vector3Int vtemp = Vector3Int.zero;
                    for (int i = bounds.xMin; i < bounds.xMax; i++)
                    {
                        vtemp.x = i; vtemp.y = j; vtemp.z = k;
                        if (tm.HasTile(vtemp))
                        {
                            maxCol = i;
                            if (i < minCol)
                                minCol = i;
                            existsTile = true;
                        }
                    }
                    if (existsTile)
                    {
                        edgePos.Add(new Vector3Int(minCol - 1, j, k));
                        edgePos.Add(new Vector3Int(maxCol + 1, j, k));
                    }
                }
            }

            return edgePos;
        }

        /// <summary>
        /// Devuelve los verdaderos límites de un Tilemap. El parámetro Tilemap.bounds tiene en cuenta posiciones de casillas
        /// eliminadas por lo tanto en ocasiones los límites no son precisos.
        /// Este método se puede optimizar para no tener que iterar por todos los elementos.
        /// </summary>
        /// <param name="tm"></param>
        /// <returns></returns>
        public static BoundsInt RealCellBounds(this Tilemap tm)
        {
            var bounds = tm.cellBounds;
            int minx = bounds.xMax;
            int miny = bounds.yMax;
            int minz = bounds.zMax;
            int maxx = bounds.xMin;
            int maxy = bounds.yMin;
            int maxz = bounds.zMin;
            for (int i = bounds.xMin; i < bounds.xMax; i++)
            {
                for (int j = bounds.yMin; j < bounds.yMax; j++)
                {
                    for (int k = bounds.zMin; k < bounds.zMax; k++)
                    {
                        Vector3Int tilepos = new Vector3Int(i, j, k);
                        if (tm.HasTile(tilepos))
                        {
                            if (i < minx) minx = i;
                            if (i > maxx) maxx = i;
                            if (j < miny) miny = j;
                            if (j > maxy) maxy = j;
                            if (k < minz) minz = k;
                            if (k > maxz) maxz = k;
                        }
                    }
                }    
            }
            bounds.xMin = minx;
            bounds.xMax = maxx + 1;
            bounds.yMin = miny;
            bounds.yMax = maxy + 1;
            bounds.zMin = minz;
            bounds.zMax = maxz + 1;
        
            return bounds;
        }

        public static Bounds WorldBounds(this Tilemap tm)
        {
            var cellBounds = tm.RealCellBounds();
            var tmPos = tm.transform.position;
            Vector3 min = Vector3.Scale(cellBounds.min, tm.cellSize) + tmPos;
            Vector3 max = Vector3.Scale(cellBounds.max, tm.cellSize) + tmPos;
            var bounds = new Bounds();
            bounds.SetMinMax(min, max);
            return bounds;
        }

        public static void MoveByOffset(this Tilemap tm, Vector3Int offset)
        {
            var tilesPositions = tm.GetTilePositions();
            var tiles = new Dictionary<Vector3Int, TileBase>();
            
            foreach (var tilePosition in tilesPositions)
            {
                tiles.Add(tilePosition, tm.GetTile(tilePosition));
            }
            tm.ClearAllTiles();
            
            foreach (var tilePosition in tiles.Keys)
            {
                var tile = tiles[tilePosition];
                var newTilePosition = tilePosition + offset;
                tm.SetTile(newTilePosition, tile);
            }
        }
        
        public static Vector3Int Center(this Tilemap tm)
        {
            var cellBounds = tm.RealCellBounds();
            int horizontalCenter = (cellBounds.xMin + cellBounds.xMax) / 2;
            int verticalCenter = (cellBounds.yMin + cellBounds.yMax) / 2;
            var offset = new Vector3Int(-horizontalCenter, -verticalCenter, 0);
            tm.MoveByOffset(offset);

            return offset;
        }
        
        public static void MirrorHorizontally(this Tilemap tm)
        {
            var tilesPositions = tm.GetTilePositions();
            var tiles = new Dictionary<Vector3Int, TileBase>();

            foreach (var tilePosition in tilesPositions)
            {
                tiles.Add(tilePosition, tm.GetTile(tilePosition));
            }
            tm.ClearAllTiles();

            foreach (var tilePosition in tiles.Keys)
            {
                var tile = tiles[tilePosition];
                var newTilePosition = new Vector3Int(-tilePosition.x, tilePosition.y, tilePosition.z);
                
                tm.SetTile(newTilePosition, tile);
            }
        }
        
        public static void MirrorVertically(this Tilemap tm)
        {
            var tilesPositions = tm.GetTilePositions();
            var tiles = new Dictionary<Vector3Int, TileBase>();

            foreach (var tilePosition in tilesPositions)
            {
                tiles.Add(tilePosition, tm.GetTile(tilePosition));
            }
            tm.ClearAllTiles();

            foreach (var tilePosition in tiles.Keys)
            {
                var tile = tiles[tilePosition];
                var newTilePosition = new Vector3Int(tilePosition.x, -tilePosition.y, tilePosition.z);
                
                tm.SetTile(newTilePosition, tile);
            }
        }

        public static void Rotate90(this Tilemap tm)
        {
            var tilesPositions = tm.GetTilePositions();
            var tiles = new Dictionary<Vector3Int, TileBase>();
            
            foreach (var tilePosition in tilesPositions)
            {
                tiles.Add(tilePosition, tm.GetTile(tilePosition));
            }
            tm.ClearAllTiles();
            
            foreach (var tilePosition in tiles.Keys)
            {
                var tile = tiles[tilePosition];
                var newTilePosition = new Vector3Int(tilePosition.y, -tilePosition.x, tilePosition.z);
                
                tm.SetTile(newTilePosition, tile);
            }
        }
        
        public static void Rotate180(this Tilemap tm)
        {
            var tilesPositions = tm.GetTilePositions();
            var tiles = new Dictionary<Vector3Int, TileBase>();
            
            foreach (var tilePosition in tilesPositions)
            {
                tiles.Add(tilePosition, tm.GetTile(tilePosition));
            }
            tm.ClearAllTiles();
            
            foreach (var tilePosition in tiles.Keys)
            {
                var tile = tiles[tilePosition];
                var newTilePosition = new Vector3Int(-tilePosition.x, -tilePosition.y, tilePosition.z);
                
                tm.SetTile(newTilePosition, tile);
            }
        }
        
        public static void Rotate270(this Tilemap tm)
        {
            var tilesPositions = tm.GetTilePositions();
            var tiles = new Dictionary<Vector3Int, TileBase>();
            
            foreach (var tilePosition in tilesPositions)
            {
                tiles.Add(tilePosition, tm.GetTile(tilePosition));
            }
            tm.ClearAllTiles();
            
            foreach (var tilePosition in tiles.Keys)
            {
                var tile = tiles[tilePosition];
                var newTilePosition = new Vector3Int(-tilePosition.y, tilePosition.x, tilePosition.z);
                
                tm.SetTile(newTilePosition, tile);
            }
        }

        public static Vector3Int GetRandomTilePosition(this Tilemap tm)
        {
            var positions = new List<Vector3Int>(tm.GetTilePositions());
            int randomIndex = Random.Range(0, positions.Count);
            var tilePosition = positions[randomIndex];
            return tilePosition;
        }
        
        public static Vector3Int GetNearestAdjacentTile(this Tilemap tilemap, Vector3 position, Vector3Int tilePosition)
        {
            Vector3Int pos = Vector3Int.zero;

            var nearestTile = tilePosition;
            float minDistance = Mathf.Infinity;
            
            for (int i = -1; i < 1; i++)
            {
                for (int j = -1; j < 1; j++)
                {
                    if (i != 0 || j != 0)
                    {
                        pos.x = tilePosition.x + i;
                        pos.y = tilePosition.y + j;

                        if (tilemap.HasTile(pos))
                        {
                            var worldTilePosition = tilemap.CellToWorld(pos);
                            float dist = Vector3.Distance(position, worldTilePosition);
                            if (dist < minDistance)
                            {
                                minDistance = dist;
                                nearestTile = pos;
                            }
                        }
                    }
                }
            }

            return nearestTile;
        }
    }
}
