using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DatabaseBase), true)]
public class DatabaseEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // 1. 기본 인스펙터 화면을 그립니다 (변수들 보여주기)
        base.OnInspectorGUI();

        // 줄 긋기 (디자인)
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("데이터 관리 도구", EditorStyles.boldLabel);

        // 현재 선택된 객체를 가져옵니다.
        DatabaseBase database = (DatabaseBase)target;

        // 2. 버튼을 만들고 클릭되었는지 확인합니다.
        if (GUILayout.Button("TSV 데이터 로드하기", GUILayout.Height(40)))
        {
            // 데이터 로드 실행
            database.LoadData();

            // 유니티에게 "이 데이터가 변경되었으니 저장해야 해"라고 알립니다. (매우 중요)
            EditorUtility.SetDirty(database);
            AssetDatabase.SaveAssets();
        }
    }
}
