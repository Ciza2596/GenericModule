using NUnit.Framework;
using UnityEngine;

public class ClassTypeTest : BaseTypeTest
{
	public const           int     _hp_1       = 10;
	public static readonly Vector2 _position_1 = Vector2.one;

	public const           int     _hp_2       = 20;
	public static readonly Vector2 _position_2 = Vector2.up;

	[Test]
	public void _01_Save_FakeClass_Type()
	{
		// arrange
		var saveValue = new FakeClass(_hp_1, _position_1);


		// act
		_saveLoadModule.Save(_saveKey, saveValue, _filePath);


		// assert
		_saveLoadModule.TryLoad<FakeClass>(_saveKey, out var loadSave, _filePath);
		Assert.AreEqual(_hp_1, loadSave.Hp, $"LoadSave's hp should be {_hp_1}.");
		Assert.AreEqual(_position_1, saveValue.Position, $"LoadSave's position should be {_position_1}.");
	}

	[Test]
	public void _02_Save_FakeClass_With_Child_Type()
	{
		// arrange
		var children  = new[] { new FakeClass(_hp_2, _position_2), new FakeClass(_hp_2, _position_2) };
		var saveValue = new FakeClass(_hp_1, _position_1, children);


		// act
		_saveLoadModule.Save(_saveKey, saveValue, _filePath);


		// assert
		_saveLoadModule.TryLoad<FakeClass>(_saveKey, out var loadSave, _filePath);
		Assert.AreEqual(_hp_1, loadSave.Hp, $"LoadSave's hp should be {_hp_1}.");
		Assert.AreEqual(_position_1, saveValue.Position, $"LoadSave's position should be {_position_1}.");

		foreach (var child in loadSave.Children)
		{
			Assert.AreEqual(_hp_2, child.Hp, $"LoadSaveChild's hp should be {_hp_2}.");
			Assert.AreEqual(_position_2, child.Position, $"LoadSaveChild's position should be {_position_2}.");
		}
	}

	[Test]
	public void _03_TryLoad_FakeClass_Without_Save()
	{
		// act
		var isLoaded = _saveLoadModule.TryLoad<FakeClass>("Save/123", out var data);

		Assert.IsFalse(isLoaded, "FakeClass should be false.");
	}
}
