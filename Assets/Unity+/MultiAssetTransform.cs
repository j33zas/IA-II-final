using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;


[CustomEditor(typeof(Transform)), CanEditMultipleObjects]
public class MultiAssetTransform : Editor
{
    AnimBool _show;

    int _objSelectedAmount;
    bool _IndividualDeegrees;
    bool _IndividualScale;

    bool _xRotate;
    bool _yRotate;
    bool _zRotate;

    bool _xScale;
    bool _ySclae;
    bool _zScale;

    float _deegreesRotationA;
    float _deegreesRotationB;

    float _UnitsScaleA;
    float _UnitsScaleB;

    bool _RotateOnWorldAxis;
    string CurrentStateR;
    Space RotationSpace = Space.Self;

    GUIStyle SeparatorStyle;
    private void OnEnable()
    {
        SeparatorStyle = new GUIStyle();
        SeparatorStyle.fontSize = 15;
        SeparatorStyle.fontStyle = FontStyle.Bold;
        SeparatorStyle.alignment = TextAnchor.MiddleCenter;
        SeparatorStyle.normal.textColor = Color.grey;

        _show = new AnimBool(false);
        _show.valueChanged.AddListener(Repaint);
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        _show.target = EditorGUILayout.Toggle("Randomize Transform", _show.target);
        if (EditorGUILayout.BeginFadeGroup(_show.faded))
        {
                EditorGUI.DrawRect(GUILayoutUtility.GetRect(300, 1), Color.grey);
            EditorGUILayout.LabelField("Rotation", SeparatorStyle);

            EditorGUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = 15;
            _xRotate = EditorGUILayout.Toggle("X", _xRotate, GUILayout.ExpandWidth(false));
            _yRotate = EditorGUILayout.Toggle("Y", _yRotate, GUILayout.ExpandWidth(false));
            _zRotate = EditorGUILayout.Toggle("Z", _zRotate, GUILayout.ExpandWidth(false));
            EditorGUIUtility.labelWidth = 0;
            EditorGUILayout.EndHorizontal();

            _deegreesRotationB = EditorGUILayout.FloatField("min Deegrees", _deegreesRotationB, GUILayout.ExpandWidth(false));
            _deegreesRotationA = EditorGUILayout.FloatField("max Deegrees", _deegreesRotationA, GUILayout.ExpandWidth(false));

            _RotateOnWorldAxis = EditorGUILayout.Toggle("Rotating on " + CurrentStateR + " axis", _RotateOnWorldAxis);
            if (_RotateOnWorldAxis)
            {
                CurrentStateR = "WORLD";
                RotationSpace = Space.World;
            }
            else
            {
                CurrentStateR = "LOCAL";
                RotationSpace = Space.Self;
            }
            //----------------------------------------------------------------------------------------------------------------------
            EditorGUI.DrawRect(GUILayoutUtility.GetRect(300, 1), Color.grey);
            EditorGUILayout.LabelField("Scale", SeparatorStyle);
            //----------------------------------------------------------------------------------------------------------------------
            EditorGUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = 15;
            _xScale = EditorGUILayout.Toggle("X", _xScale, GUILayout.ExpandWidth(false));
            _ySclae = EditorGUILayout.Toggle("Y", _ySclae, GUILayout.ExpandWidth(false));
            _zScale = EditorGUILayout.Toggle("Z", _zScale, GUILayout.ExpandWidth(false));
            EditorGUIUtility.labelWidth = 0;
            EditorGUILayout.EndHorizontal();

            _UnitsScaleB = EditorGUILayout.FloatField("min Scale", _UnitsScaleB, GUILayout.ExpandWidth(false));
            _UnitsScaleA = EditorGUILayout.FloatField("max Scale", _UnitsScaleA, GUILayout.ExpandWidth(false));

            if(GUILayout.Button("Randomize"))
            {
                if(_xRotate)
                    foreach (var item in Selection.gameObjects)
                        item.transform.Rotate(new Vector3(Random.Range(_deegreesRotationA, _deegreesRotationB), 0, 0), RotationSpace);
                if(_yRotate)
                    foreach (var item in Selection.gameObjects)
                        item.transform.Rotate(new Vector3(0,Random.Range(_deegreesRotationA, _deegreesRotationB), 0), RotationSpace);
                if(_zRotate)
                    foreach (var item in Selection.gameObjects)
                        item.transform.Rotate(new Vector3(0,0,Random.Range(_deegreesRotationA, _deegreesRotationB)), RotationSpace);
                if (_xScale)
                    foreach (var item in Selection.gameObjects)
                        item.transform.localScale += new Vector3(Random.Range(_UnitsScaleA, _UnitsScaleB),0,0);
                if (_ySclae)
                    foreach (var item in Selection.gameObjects)
                        item.transform.localScale += new Vector3(0,Random.Range(_UnitsScaleA, _UnitsScaleB),0);
                if(_zScale)
                    foreach (var item in Selection.gameObjects)
                        item.transform.localScale += new Vector3(0, 0,Random.Range(_UnitsScaleA, _UnitsScaleB));
            }
        }
        EditorGUILayout.EndFadeGroup();
    }
}