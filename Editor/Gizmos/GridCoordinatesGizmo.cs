using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Utils.Editor.Gizmos
{
    public static class GridCoordinatesGizmo
    {
        [DrawGizmo(GizmoType.InSelectionHierarchy)]
        public static void DrawGizmo(Grid grid, GizmoType gizmoType)
        {
            Draw(SceneView.currentDrawingSceneView, grid);
        }
        
        [DrawGizmo(GizmoType.InSelectionHierarchy)]
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

            int minCellX = grid.WorldToCell(bottomLeftWorld).x;
            int maxCellX = grid.WorldToCell(topRightWorld).x;
            for (int x = minCellX; x <= maxCellX; x++)
            {
                string text = $"{x}";
                
                var textSize = labelStyle.CalcSize(new GUIContent(text));
                float textHalfWidthWorld = GetHorizontalScreenToWorldSize(camera, textSize.x * 0.5f);

                float xCenter = grid.GetCellCenterWorld(new Vector3Int(x, 0, 0)).x;
                float xText = xCenter - textHalfWidthWorld;
                float yText = bottomLeftWorld.y + offset;
                var handlePos = new Vector3(xText, yText, 0);
                Handles.Label(handlePos, text, labelStyle);
            }
            
            int minCellY = grid.WorldToCell(bottomLeftWorld).y;
            int maxCellY = grid.WorldToCell(topRightWorld).y;
            for (int y = minCellY; y <= maxCellY; y++)
            {
                string text = $"{y}";
                float yCenter = grid.GetCellCenterWorld(new Vector3Int(0, y, 0)).y;
                float xText = bottomLeftWorld.x + offset;
                float yText = yCenter + textHalfHeightWorld;
                var handlePos = new Vector3(xText, yText, 0);
                Handles.Label(handlePos, text, labelStyle);
            }
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