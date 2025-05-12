// CustomEnemyGridEditorWindow.cs
using UnityEngine;
using UnityEditor;

public class EnemyGridEditorWindow : EditorWindow {
    private const int gridSize = 3;
    private const int cellSize = 80;
    private const int cellPadding = 5;

    private EnemyData[,] grid;
    private Vector2 scrollPos;

    [MenuItem("Tools/Custom Enemy Grid Editor")]
    public static void OpenWindow() {
        GetWindow<EnemyGridEditorWindow>("Enemy Grid Editor");
    }

    private void OnEnable() {
        grid = new EnemyData[gridSize, gridSize];
    }

    private void OnGUI() {
        GUILayout.Label("Enemy Grid (3x3)", EditorStyles.boldLabel);

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        DrawGrid();
        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("Save Grid")) {
            SaveGrid();
        }

        if (GUI.changed) {
            Repaint();
        }
    }

    private void DrawGrid() {
        GUILayout.BeginVertical();
        for (int i = 0; i < gridSize; i++) {
            EditorGUILayout.BeginHorizontal();
            for (int j = 0; j < gridSize; j++) {
                DrawCell(i, j);
                if (j < gridSize - 1) GUILayout.Space(cellPadding);
            }
            EditorGUILayout.EndHorizontal();
            if (i < gridSize - 1) GUILayout.Space(cellPadding);
        }
        GUILayout.EndVertical();
    }

    private void DrawCell(int x, int y) {
        Rect rect = GUILayoutUtility.GetRect(cellSize, cellSize, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));
        EditorGUI.DrawRect(rect, Color.gray);

        if (grid[x, y] != null) {
            GUIStyle style = new GUIStyle(GUI.skin.label) {
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true,
                normal = { textColor = Color.white }
            };
            GUI.Label(rect, grid[x, y].name, style);
        }

        HandleDragAndDrop(rect, x, y);
    }

    private void HandleDragAndDrop(Rect rect, int x, int y) {
        Event evt = Event.current;
        if (rect.Contains(evt.mousePosition)) {
            if (evt.type == EventType.DragUpdated) {
                if (DragAndDrop.objectReferences.Length > 0 && DragAndDrop.objectReferences[0] is EnemyData) {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                } else {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                }
                evt.Use();
            }
            else if (evt.type == EventType.DragPerform) {
                DragAndDrop.AcceptDrag();
                if (DragAndDrop.objectReferences.Length > 0) {
                    var data = DragAndDrop.objectReferences[0] as EnemyData;
                    if (data != null) {
                        grid[x, y] = data;
                        GUI.changed = true;
                    }
                }
                evt.Use();
            }
            else if (evt.type == EventType.MouseDown && evt.button == 1) {
                grid[x, y] = null;
                GUI.changed = true;
                evt.Use();
            }
        }
    }

    private void SaveGrid() {
        EnemyGridData asset = CreateInstance<EnemyGridData>();
        asset.Init(grid);

        string path = EditorUtility.SaveFilePanelInProject(
            "Save Enemy Grid",
            "NewEnemyGrid",
            "asset",
            "Enter a file name...",
            "Assets/Datas/GameDatas"
        );
        if (string.IsNullOrEmpty(path)) return;

        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
}



