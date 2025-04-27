using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class PatternEditorWindow : EditorWindow
{
    private const int gridSize = 3;
    private const int cellSize = 50;
    private const int cellPadding = 5;
    private HashSet<(int x, int y)> selectedCells = new HashSet<(int x, int y)> { (0, 0) };

    [MenuItem("Tools/Pattern Editor")]
    public static void OpenWindow()
    {
        GetWindow<PatternEditorWindow>("Pattern Editor");
    }

    private void OnGUI()
    {
        GUILayout.Label("Pattern Editor", EditorStyles.boldLabel);

        DrawGrid();

        if (GUILayout.Button("Save"))
        {
            SavePattern();
        }
    }

    private void DrawGrid()
    {
        GUILayout.BeginVertical();

        for (int y = 2; y >= 0; y--) // Commencer en haut (y=2) et descendre vers 0
        {
            EditorGUILayout.BeginHorizontal();
            for (int x = 0; x <= 2; x++) // Gauche (x=0) à droite (x=2)
            {
                DrawCell((x, y));
                if (x < 2) GUILayout.Space(cellPadding);
            }
            EditorGUILayout.EndHorizontal();
            if (y > 0) GUILayout.Space(cellPadding);
        }

        GUILayout.EndVertical();
    }

    private void DrawCell((int x, int y) position)
    {
        bool isSelected = selectedCells.Contains(position);
        Rect rect = GUILayoutUtility.GetRect(cellSize, cellSize, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));
        
        Color cellColor = isSelected ? Color.green : Color.gray;
        if (position == (0, 0)) cellColor = Color.red; // (0,0) est toujours sélectionné et rouge
        
        EditorGUI.DrawRect(rect, cellColor);
        HandleInput(rect, position);
    }

    private void HandleInput(Rect rect, (int x, int y) position)
    {
        Event e = Event.current;
        if (rect.Contains(e.mousePosition) && e.type == EventType.MouseDown)
        {
            if (e.button == 0)
            {
                selectedCells.Add(position);
            }
            else if (e.button == 1 && position != (0, 0)) // Empêcher la suppression de (0,0)
            {
                selectedCells.Remove(position);
            }
            e.Use();
            Repaint();
        }
    }

    private void SavePattern()
    {
        PatternData asset = CreateInstance<PatternData>();
        asset.Init(selectedCells);

        string path = EditorUtility.SaveFilePanelInProject
        (
            "Save Pattern Data",
            "NewPatternData",
            "asset",
            "Enter Pattern Name...",
            "Assets/Datas/GameDatas/Pattern"
        );
        if (string.IsNullOrEmpty(path)) return;

        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
}