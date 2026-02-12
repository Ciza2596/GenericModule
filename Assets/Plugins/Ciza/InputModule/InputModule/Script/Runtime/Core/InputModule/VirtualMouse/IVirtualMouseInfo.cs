using System;
using UnityEngine;

namespace CizaInputModule
{
	public interface IVirtualMouseInfo
	{
		Type VirtualMouseType { get; }

		GameObject BodyPrefab { get; }
	}
}