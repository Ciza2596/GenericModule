using System;
using NUnit.Framework;

public class PrimitiveTypeTest : BaseTypeTest
{
    [TestCase(true, true)]
    [TestCase(false, false)]
    public void _01_Save_Bool_Type(bool saveValue, bool expectedValue)
    {
        //act
        Save_Data(saveValue);

        //assert
        Check_LoadData_Result(expectedValue);
    }

    [TestCase('s', 's')]
    [TestCase('@', '@')]
    public void _02_Save_Char_Type(char saveValue, char expectedValue)
    {
        //act
        Save_Data(saveValue);

        //assert
        Check_LoadData_Result(expectedValue);
    }

    [Test]
    public void _03_Save_DateTime_Type()
    {
        //arrange
        var saveValue = DateTime.MaxValue;
        var expectedValue = saveValue;

        //act
        Save_Data(saveValue);

        //assert
        Check_LoadData_Result(expectedValue);
    }

    [TestCase(0.5, 0.5)]
    [TestCase(10, 10)]
    public void _04_Save_Double_Type(double saveValue, double expectedValue)
    {
        //act
        Save_Data(saveValue);

        //assert
        Check_LoadData_Result(expectedValue);
    }

    [TestCase(FakeEnum.Fake_1, FakeEnum.Fake_1)]
    [TestCase(FakeEnum.Fake_2, FakeEnum.Fake_2)]
    public void _05_Save_Enum_Type(FakeEnum saveValue, FakeEnum expectedValue)
    {
        //act
        Save_Data(saveValue);

        //assert
        Check_LoadData_Result(expectedValue);
    }

    [TestCase(0.5f, 0.5f)]
    [TestCase(10f, 10f)]
    public void _06_Save_Float_Type(float saveValue, float expectedValue)
    {
        //act
        Save_Data(saveValue);

        //assert
        Check_LoadData_Result(expectedValue);
    }

    [TestCase(0, 0)]
    [TestCase(10, 10)]
    public void _07_Save_Int_Type(int saveValue, int expectedValue)
    {
        //act
        Save_Data(saveValue);

        //assert
        Check_LoadData_Result(expectedValue);
    }

    [TestCase(100,100)]
    public void _08_Save_Long_Type(long saveValue, long expectedValue)
    {
        //act
        Save_Data(saveValue);

        //assert
        Check_LoadData_Result(expectedValue);
    }

    [TestCase(50,50)]
    public void _09_Save_Short_Type(short saveValue, short expectedValue)
    {
        //act
        Save_Data(saveValue);

        //assert
        Check_LoadData_Result(expectedValue);
    }

    [TestCase("Hello", "Hello")]
    [TestCase("Ciza", "Ciza")]
    public void _10_Save_String_Type(string saveValue, string expectedValue)
    {
        //act
        Save_Data(saveValue);

        //assert
        Check_LoadData_Result(expectedValue);
    }

    //private method
    private void Save_Data<T>(T saveValue) =>
        _saveLoadModule.Save(SAVE_KEY, saveValue, FILE_PATH);

    private void Check_LoadData_Result<T>(T expectedValue)
    {
        var hasData = _saveLoadModule.TryLoad<T>(SAVE_KEY, out var value, FILE_PATH);
        Assert.IsTrue(hasData, "Not find data on file path.");
        Assert.AreEqual(expectedValue, value, "ExpectedValue and loadData ia not match.");
    }
}