using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

public class CollectionTypeTest : BaseTypeTest
{
    [Test]
    public void _01_Save_Array_Type()
    {
        //arrange
        var saveValue = new int[] { 1, 2, 3 };
        var expectedValue = saveValue;


        //act
        Save_Data(saveValue);


        //assert
        var loadData = GetAndCheck_LoadData<int[]>();
        Check_Value_Match(expectedValue, loadData);
    }

    [Test]
    public void _02_Save_Array2D_Type()
    {
        //arrange
        var saveValue = new int[][] { new int[] { 1, 2, 3 }, new int[] { 4, 5, 6 } };
        var expectedValue = saveValue;


        //act
        Save_Data(saveValue);


        //assert
        var loadData = GetAndCheck_LoadData<int[][]>();

        for (int i = 0; i < loadData.Length; i++)
        {
            var items = loadData[i];
            var expectedItems = expectedValue[i];
            Check_Value_Match(expectedItems, items);
        }
    }

    [Test]
    public void _03_Save_Array3D_Type()
    {
        //arrange
        var saveValue = new int[][][]
        {
            new int[][] { new int[] { 1, 2 }, new int[] { 3, 4 } },
            new int[][] { new int[] { 5, 6 }, new int[] { 7, 8 } }
        };
        var expectedValue = saveValue;


        //act
        Save_Data(saveValue);

        //assert
        var loadData = GetAndCheck_LoadData<int[][][]>();

        for (int i = 0; i < loadData.Length; i++)
        {
            var items1 = loadData[i];
            for (int j = 0; j < items1.Length; j++)
            {
                var items2 = items1[j];
                var expectedItems = expectedValue[i][j];
                Check_Value_Match(expectedItems, items2);
            }
        }
    }

    [Test]
    public void _04_Save_Dictionary_Type()
    {
        //arrange
        var saveValue = new Dictionary<string, int>();
        saveValue.Add("Key_1", 1);
        saveValue.Add("Key_2", 2);
        saveValue.Add("Key_3", 3);

        var expectedValue = saveValue;


        //act
        Save_Data(saveValue);


        //assert
        var loadData = GetAndCheck_LoadData<Dictionary<string, int>>();

        var loadDataKeys = loadData.Keys.ToArray();
        var expectedValueKeys = expectedValue.Keys.ToArray();
        Check_Value_Match(expectedValueKeys, loadDataKeys);

        var loadDataValues = loadData.Values.ToArray();
        var expectedValueValues = expectedValue.Values.ToArray();
        Check_Value_Match(expectedValueValues, loadDataValues);
    }

    [Test]
    public void _05_Save_HashSet_Type()
    {
        //arrange
        var saveValue = new HashSet<int>() { 1, 2, 3 };
        var expectedValue = saveValue;


        //act
        Save_Data(saveValue);


        //assert
        var loadData = GetAndCheck_LoadData<HashSet<int>>();
        Check_Value_Match(expectedValue.ToArray(), loadData.ToArray());
    }

    [Test]
    public void _06_Save_List_Type()
    {
        //arrange
        var saveValue = new List<int>() { 1, 2, 3 };
        var expectedValue = saveValue;


        //act
        Save_Data(saveValue);


        //assert
        var loadData = GetAndCheck_LoadData<List<int>>();
        Check_Value_Match(expectedValue.ToArray(), loadData.ToArray());
    }

    [Test]
    public void _07_Save_Queue_Type()
    {
        //arrange
        var saveValue = new Queue<int>();
        saveValue.Enqueue(1);
        saveValue.Enqueue(2);
        saveValue.Enqueue(3);
        
        var expectedValue = saveValue;


        //act
        Save_Data(saveValue);


        //assert
        var loadData = GetAndCheck_LoadData<Queue<int>>();
        Check_Value_Match(expectedValue.ToArray(), loadData.ToArray());
        
    }

    [Test]
    public void _08_Save_Stack_Type()
    {
        //arrange
        var saveValue = new Stack<int>();
        saveValue.Push(1);
        saveValue.Push(2);
        saveValue.Push(3);
        
        var expectedValue = saveValue;


        //act
        Save_Data(saveValue);


        //assert
        var loadData = GetAndCheck_LoadData<Stack<int>>();
        Check_Value_Match(expectedValue.ToArray(), loadData.ToArray());

    }

    private void Check_Value_Match<T>(T[] exceptedValues, T[] values)
    {
        for (int i = 0; i < values.Length; i++)
        {
            var value = values[i];
            var exceptedValue = exceptedValues[i];
            Assert.AreEqual(exceptedValue, value, "SaveData and loadData ia not match.");
        }
    }

    private void Save_Data<T>(T saveValue) =>
        _saveLoadModule.Save(_saveKey, saveValue, _filePath);

    private T GetAndCheck_LoadData<T>()
    {
        var hasData = _saveLoadModule.TryLoad<T>(_saveKey, out var value, _filePath);
        Assert.IsTrue(hasData, "Not find data on file path.");
        return value;
    }
}