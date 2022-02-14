using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class MaterialEditorWindow : EditorWindow
{
    #region Searchbar
    string _currentsearch;

    string _searchQuery = "";

    List<Material> _searchResults = new List<Material>();
    #endregion


    
    PreviewRenderUtility renderer;

    Material _OGMaterial;
    Material _copyMaterial;

    Editor _EditorA = null;
    Editor _EditorB = null;


    Mesh FBX;

    GUIStyle Centre = new GUIStyle();

    [MenuItem("Unity+/Material editor &m")]
    public static void OpenWindow()
    {
        var me = GetWindow<MaterialEditorWindow>();
    }

    private void OnEnable()
    {
        renderer = new PreviewRenderUtility();
        Centre.alignment = TextAnchor.MiddleCenter;
    }

    private void OnGUI()
    {
        if(_OGMaterial == null)
        {
            _OGMaterial = (Material)EditorGUILayout.ObjectField("Material", _OGMaterial, typeof(Material), true);
        }

        if (_OGMaterial != null)
            _copyMaterial = _OGMaterial;

        GUIStyle _style = new GUIStyle();
        _style.normal.background = Texture2D.blackTexture;
        FBX = (Mesh)EditorGUILayout.ObjectField("Mesh",FBX, typeof(Mesh),false);

        EditorGUILayout.BeginHorizontal();
        if (_OGMaterial != null)
        {
            DrawVerticalMaterialPreview(_EditorA, _OGMaterial, _style);

            GUILayout.Space(1);
            EditorGUI.DrawRect(GUILayoutUtility.GetRect(1, position.height), Color.black);//linea vertical
            EditorGUI.DrawRect(GUILayoutUtility.GetRect(3, position.height), Color.grey);//linea vertical
            EditorGUI.DrawRect(GUILayoutUtility.GetRect(1, position.height), Color.black);//linea vertical
            GUILayout.Space(1);

            DrawVerticalMaterialPreview(_EditorB, _copyMaterial, _style);
        }
        EditorGUILayout.EndHorizontal();
    }
    void DrawVerticalMaterialPreview(Editor EditorToPreview, Material Mat, GUIStyle style)
    {
        Shader MatShader = Mat.shader;
        EditorGUILayout.BeginVertical();
        if (EditorToPreview == null)
            EditorToPreview = Editor.CreateEditor(Mat);
        EditorToPreview.OnPreviewGUI(GUILayoutUtility.GetRect(position.width / 2, 300), style);

        Mat.shader = (Shader)EditorGUILayout.ObjectField("Shader", Mat.shader, typeof(Shader), false);
        GUILayout.Label("Shader Properties", Centre);

        
        EditorGUILayout.EndVertical();
    }
    void SearchBar()
    {
        _searchResults.Clear();
        string[] paths = AssetDatabase.FindAssets(_searchQuery);
        if (paths.Length > 0)
        {
            for (int i = 0; i < paths.Length - 1; i++)
            {

                paths[i] = AssetDatabase.GUIDToAssetPath(paths[i]);

                Object _current = AssetDatabase.LoadAssetAtPath(paths[i], typeof(Object));
            }
        }

        if (_searchResults.Count > 0)
        {
            EditorGUI.DrawRect(new Rect(), Color.black);
            for (int i = 0; i < _searchResults.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.LabelField(_searchResults[i].name);

                GUI.DrawTexture(GUILayoutUtility.GetRect(30, 30), AssetPreview.GetMiniTypeThumbnail(_searchResults[i].GetType()), ScaleMode.ScaleToFit);

                EditorGUILayout.EndHorizontal();
            }
        }
    }
}
