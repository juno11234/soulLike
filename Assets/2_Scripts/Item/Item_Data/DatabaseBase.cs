using System.Collections.Generic;
using UnityEngine;


// 모든 데이터베이스의 공통 부모 클래스
public abstract class DatabaseBase : ScriptableObject
{
    [Header("TSV 파일 설정")] [Tooltip("여기에 .tsv 또는 .txt 파일을 드래그하세요")]
    public TextAsset tsvFile;

    // 자식들이 각자 알아서 구현해야 하는 추상 함수
    // "로드하는 행동은 필수지만, 로드하는 내용은 자식마다 다르다"는 뜻입니다.
    public abstract void LoadData();
}

[System.Serializable]
public class ItemData
{
    public int ID;
    public string Name;
    public float AttackPower;
    public string Description;
}

[System.Serializable]
public class TestData
{
    public int ID;
    public string Name;
    public float TestFloat;
    public string Description;
    public bool nice;
}

[CreateAssetMenu(fileName = "TestData", menuName = "Game/TestDatabase")]
public class TestDatabase : DatabaseBase
{
    public List<TestData> testData = new List<TestData>();

    public override void LoadData()
    {
        if (tsvFile == null)
        {
            Debug.LogError("tsvFile is null");
            return;
        }

        testData = TSVImporter.Parse<TestData>(tsvFile.text);
        Debug.Log($"<color=yellow>성공: </color>{testData.Count}</color>");
    }
}

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Game/ItemDatabase")]
public class ItemDatabase : DatabaseBase
{
    public List<ItemData> items = new List<ItemData>();

    public override void LoadData()
    {
        if (tsvFile == null)
        {
            Debug.LogError("TSV 파일이 비어있습니다! 인스펙터에서 파일을 할당해주세요.");
            return;
        }

        // 이전에 만든 TSVImporter를 사용
        items = TSVImporter.Parse<ItemData>(tsvFile.text);
        Debug.Log($"<color=green>성공:</color> {items.Count}개의 아이템 데이터를 불러왔습니다.");
    }
}