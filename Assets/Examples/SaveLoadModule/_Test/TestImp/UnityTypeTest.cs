using NUnit.Framework;
using UnityEngine;

public class UnityTypeTest: BaseTypeTest
{
    [Test]
    public void _01_Save_Vector2_Type()
    {
        //arrange
        var saveValue = new Vector2(10.05f, 50.05f);
        var expectedValue = saveValue;
        
        //act
        Save_Data(saveValue);

        //assert
        Check_LoadData_Result(expectedValue);
    }
    
    [Test]
    public void _02_Save_Vector2Int_Type()
    {
        //arrange
        var saveValue = new Vector2Int(10, 50);
        var expectedValue = saveValue;
        
        //act
        Save_Data(saveValue);

        //assert
        Check_LoadData_Result(expectedValue);
    }
    
    [Test]
    public void _03_Save_Vector3_Type()
    {
        //arrange
        var saveValue = new Vector3(10.6f, 50.6f);
        var expectedValue = saveValue;
        
        //act
        Save_Data(saveValue);

        //assert
        Check_LoadData_Result(expectedValue);
    }
    
    [Test]
    public void _04_Save_Vector3Int_Type()
    {
        //arrange
        var saveValue = new Vector3Int(10, 50);
        var expectedValue = saveValue;
        
        //act
        Save_Data(saveValue);

        //assert
        Check_LoadData_Result(expectedValue);
    }
    
    //private method
    private void Save_Data<T>(T saveValue) =>
        _saveLoadModule.Save(_saveKey, saveValue, _filePath);

    private void Check_LoadData_Result<T>(T expectedValue)
    {
        var hasData = _saveLoadModule.TryLoad<T>(_saveKey, out var value, _filePath);
        Assert.IsTrue(hasData, "Not find data on file path.");
        Assert.AreEqual(expectedValue, value, "ExpectedValue and loadData ia not match.");
    }
}
