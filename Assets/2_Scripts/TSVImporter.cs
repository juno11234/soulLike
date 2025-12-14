using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class TSVImporter : MonoBehaviour
{
 // 제네릭 메서드: <T>는 어떤 클래스 타입이든 될 수 있음을 의미합니다.
     public static List<T> Parse<T>(string textData) where T : new()
     {
         List<T> resultList = new List<T>();
 
         // 1. 줄바꿈을 기준으로 데이터를 나눕니다.
         string[] lines = textData.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
 
         if (lines.Length == 0) return resultList;
 
         // 2. 첫 번째 줄(헤더)을 파싱하여 변수 이름 리스트를 만듭니다.
         string[] headers = lines[0].Split('\t');
 
         // 3. 두 번째 줄부터 실제 데이터이므로 반복합니다.
         for (int i = 1; i < lines.Length; i++)
         {
             string[] values = lines[i].Split('\t');
             
             // 빈 줄이거나 데이터 개수가 안 맞으면 건너뜁니다. (안전 장치)
             if (values.Length != headers.Length) continue;
 
             // 새로운 객체 생성 (T 타입)
             T entry = new T();
 
             // 4. 리플렉션을 사용하여 헤더 이름과 일치하는 필드에 값을 넣습니다.
             for (int j = 0; j < headers.Length; j++)
             {
                 string header = headers[j].Trim();
                 string value = values[j].Trim();
 
                 // T 클래스 안에 header와 이름이 같은 필드(변수) 정보를 가져옵니다.
                 FieldInfo field = typeof(T).GetField(header, BindingFlags.Public | BindingFlags.Instance);
 
                 if (field != null)
                 {
                     try
                     {
                         // 문자열인 TSV 데이터를 필드의 실제 타입(int, float, enum 등)으로 변환합니다.
                         object convertedValue = ConvertType(value, field.FieldType);
                         field.SetValue(entry, convertedValue);
                     }
                     catch (Exception e)
                     {
                         Debug.LogError($"파싱 에러! 필드: {header}, 값: {value}. 에러: {e.Message}");
                     }
                 }
             }
 
             resultList.Add(entry);
         }
 
         return resultList;
     }
 
     // 타입 변환 도우미 메서드
     private static object ConvertType(string value, Type type)
     {
         if (type == typeof(int)) return int.Parse(value);
         if (type == typeof(float)) return float.Parse(value);
         if (type == typeof(bool)) return bool.Parse(value);
         if (type.IsEnum) return Enum.Parse(type, value);
         
         return value; // 기본적으로 string
     }
}
