using UnityEngine;
using UnityEditor;

public class CustomLevelEditorWindow : EditorWindow
{
    private int currentLevelHeight = 5;
    private int currentLevelWidth = 5;

    private int levelHeight = 5;
    private int levelWidth = 5;

    private const int cellSize = 50;
    private const int cellPadding = 5;

    private AreaData[,] level;
    
    private Vector2 currentScrollPosition;


    [MenuItem("Tools/Custom Level Editor")]
    public static void OpenWindow()
    {
        GetWindow<CustomLevelEditorWindow>("Custom Level Editor");
    }

    private void OnEnable()
    {
        level = new AreaData[currentLevelHeight, currentLevelWidth];
    }

    private void OnGUI()
    {
        GUILayout.Label("Size Parameters", EditorStyles.boldLabel);
        
        levelHeight = EditorGUILayout.IntField("Height", levelHeight);
        levelWidth = EditorGUILayout.IntField("Width", levelWidth);
        
        if (GUILayout.Button("Resize"))
        {
            currentLevelHeight = levelHeight;
            currentLevelWidth = levelWidth;

            level = new AreaData[currentLevelHeight, currentLevelWidth];
        }
        
        GUILayout.Label("Drag and Drop Areas", EditorStyles.boldLabel);

        currentScrollPosition = EditorGUILayout.BeginScrollView(currentScrollPosition);

        DrawLevelGrid();

        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("Save"))
        {
            SaveCustomLevel();
        }

        if (GUI.changed)
        {
            Repaint();
        }
    }

    private void DrawLevelGrid()
    {
        GUILayout.BeginVertical("box");

        for (int i = 0; i < currentLevelHeight; i++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int j = 0; j < currentLevelWidth; j++)
            {
                DrawCell((i, j));
                if (j < currentLevelWidth - 1)
                {
                    GUILayout.Space(cellPadding);
                }
            }
            EditorGUILayout.EndHorizontal();
            if (i < currentLevelHeight - 1)
            {
                GUILayout.Space(cellPadding);
            }
        }

        GUILayout.EndVertical();
    }

    private void DrawCell((int x, int y) cellPosition)
    {
        AreaData cellData = level[cellPosition.x, cellPosition.y];

        Rect rect = GUILayoutUtility.GetRect(cellSize, cellSize, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));

        EditorGUI.DrawRect(rect, Color.white);

        if (cellData != null)
        {
            GUIStyle textStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true,
                normal = { textColor = Color.black }
            };
            GUI.Label(rect, cellData.areaName, textStyle);
        }

        DragAndDropZone(rect, cellPosition);
    }

    private void DragAndDropZone(Rect dropArea, (int x, int y) cellPosition)
    {
        Event currentEvent = Event.current;

        if (dropArea.Contains(currentEvent.mousePosition))
        {
            if (currentEvent.type == EventType.DragUpdated)
            {
                if (DragAndDrop.objectReferences.Length > 0 && DragAndDrop.objectReferences[0] is AreaData)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                }
                else
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                }
                currentEvent.Use();
            }
            else if (currentEvent.type == EventType.DragPerform)
            {
                DragAndDrop.AcceptDrag();
                if (DragAndDrop.objectReferences.Length > 0)
                {
                    AreaData droppedData = DragAndDrop.objectReferences[0] as AreaData;
                    if (droppedData != null)
                    {
                        level[cellPosition.x, cellPosition.y] = droppedData;
                        GUI.changed = true;
                    }
                }
                currentEvent.Use();
            }
            else if (currentEvent.type == EventType.MouseDown && currentEvent.button == 1)
            {
                level[cellPosition.x, cellPosition.y] = null;
                GUI.changed = true;
                currentEvent.Use();
            }
        }
    }

    private void SaveCustomLevel()
    {
        CustomLevelData asset = CreateInstance<CustomLevelData>();
        asset.Init(level);

        string path = EditorUtility.SaveFilePanelInProject
        (
            "Save Custom Level Data",
            "NewCustomLevelData",
            "asset",
            "Enter Custom Level Name...",
            "Assets/Datas/GameDatas/CustomLevel"
        );
        if (string.IsNullOrEmpty(path)) return;

        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
}
