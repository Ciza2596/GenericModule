using System;
using UnityEngine;

[Serializable]
public class FakeClass
{
	[SerializeField]
	private int _hp;

	[SerializeField]
	private Vector2 _position;

	[SerializeField]
	private FakeClass _child;

	[SerializeField]
	private FakeClass[] _children;

	public FakeClass() { }

	public FakeClass(int hp, Vector2 position)
	{
		_hp       = hp;
		_position = position;
	}

	public FakeClass(int hp, Vector2 position, FakeClass child) : this(hp, position) =>
		_child = child;

	public FakeClass(int hp, Vector2 position, FakeClass[] children) : this(hp, position) =>
		_children = children;

	public int     Hp       => _hp;
	public Vector2 Position => _position;

	public FakeClass   Child    => _child;
	public FakeClass[] Children => _children;
}
