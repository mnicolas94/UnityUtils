using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Utils.Editor.Gizmos
{
    public static class GridCoordinatesGizmo
    {
        [DrawGizmo(GizmoType.Selected)]
        public static void DrawGizmo(Grid grid, GizmoType gizmoType)
        {
            Draw(SceneView.currentDrawingSceneView, grid);
        }
        
        [DrawGizmo(GizmoType.Selected)]
        public static void DrawGizmo(Tilemap tilemap, GizmoType gizmoType)
        {
            Draw(SceneView.currentDrawingSceneView, tilemap.layoutGrid);
        }
        
        private static void Draw(SceneView sceneView, Grid grid)
        {
            var camera = sceneView.camera;
            var bottomLeftWorld = camera.ViewportToWorldPoint(Vector3.zero);
            var topRightWorld = camera.ViewportToWorldPoint(Vector3.one);
            
            var labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.UpperLeft
            };
            var tmpTextSize = labelStyle.CalcSize(new GUIContent("0"));
            float textHeightWorld = GetVerticalScreenToWorldSize(camera, tmpTextSize.y);
            float textHalfHeightWorld = textHeightWorld * 0.5f;
            float offset = textHeightWorld * 2;

            // compute horizontal variables
            int minCellX = grid.WorldToCell(bottomLeftWorld).x;
            int maxCellX = grid.WorldToCell(topRightWorld).x;
            float minCellXTextSize = GetTextWidthWorld($"{minCellX}", labelStyle, camera);
            float maxCellXTextSize = GetTextWidthWorld($"{maxCellX}", labelStyle, camera);
            float maxWidthSize = Mathf.Max(minCellXTextSize, maxCellXTextSize);

            IEnumerable<int> xCells;
            if (maxWidthSize >= grid.cellSize.x * 0.8f)
                xCells = MathUtils.SplitNicely(minCellX, maxCellX).Select(x => (int) x);
            else
                xCells = Enumerable.Range(minCellX, maxCellX - minCellX + 1);
            
            // compute vertical variables
            int minCellY = grid.WorldToCell(bottomLeftWorld).y;
            int maxCellY = grid.WorldToCell(topRightWorld).y;
            IEnumerable<int> yCells;
            if (textHeightWorld >= grid.cellSize.y * 0.8f)
                yCells = MathUtils.SplitNicely(minCellY, maxCellY).Select(y => (int) y);
            else
                yCells = Enumerable.Range(minCellY, maxCellY - minCellY + 1);

            var xCellsList = xCells.ToList();
            var yCellsList = yCells.ToList();

            string cellText = "XXX ; XXX";
            if (GetTextWidthWorld(cellText, GUI.skin.label, camera) > grid.cellSize.x * 0.7f)
                DrawInAxis(grid, xCellsList, labelStyle, camera, bottomLeftWorld, offset, yCellsList, textHalfHeightWorld);
            else
                DrawInCells(grid, xCellsList, yCellsList, textHeightWorld);
        }

        private static void DrawInAxis(Grid grid, IEnumerable<int> xCells, GUIStyle labelStyle, Camera camera,
            Vector3 bottomLeftWorld, float offset, IEnumerable<int> yCells, float textHalfHeightWorld)
        {
            foreach (var x in xCells)
            {
                string text = $"{x}";
                float textHalfWidthWorld = GetTextWidthWorld(text, labelStyle, camera) * 0.5f;
                
                float xCenter = grid.GetCellCenterWorld(new Vector3Int(x, 0, 0)).x;
                float xText = xCenter - textHalfWidthWorld;
                float yText = bottomLeftWorld.y + offset;
                var handlePos = new Vector3(xText, yText, 0);
                Handles.Label(handlePos, text, labelStyle);
            }

            foreach (var y in yCells)
            {
                string text = $"{y}";
                float yCenter = grid.GetCellCenterWorld(new Vector3Int(0, y, 0)).y;
                float xText = bottomLeftWorld.x + offset;
                float yText = yCenter + textHalfHeightWorld;
                var handlePos = new Vector3(xText, yText, 0);
                Handles.Label(handlePos, text, labelStyle);
            }
        }
        
        private static void DrawInCells(Grid grid, IEnumerable<int> xCells, IEnumerable<int> yCells, float textHeight)
        {
            var xCellsList = xCells.ToList();
            var yCellsList = yCells.ToList();
            foreach (var x in xCellsList)
            {
                foreach (var y in yCellsList)
                {
                    string text = $"{x} ; {y}";
                    var handlePos = grid.CellToWorld(new Vector3Int(x, y, 0));
                    handlePos.y = handlePos.y + textHeight;
                    Handles.Label(handlePos, text, GUI.skin.label);
                }
            }
        }

        private static float GetTextWidthWorld(string text, GUIStyle style, Camera camera)
        {
            var textSize = style.CalcSize(new GUIContent(text));
            float textWidthWorld = GetHorizontalScreenToWorldSize(camera, textSize.x);
            return textWidthWorld;
        }
        
        private static float GetHorizontalScreenToWorldSize(Camera camera, float size)
        {
            float xZeroWorld = camera.ScreenToWorldPoint(Vector3.zero).x;
            float xRightWorld = camera.ScreenToWorldPoint(new Vector3(size, 0, 0)).x;
            return xRightWorld - xZeroWorld;
        }
        
        private static float GetVerticalScreenToWorldSize(Camera camera, float size)
        {
            float yZeroWorld = camera.ScreenToWorldPoint(Vector3.zero).y;
            float yRightWorld = camera.ScreenToWorldPoint(new Vector3(0, size, 0)).y;
            return yRightWorld - yZeroWorld;
        }
    }
}