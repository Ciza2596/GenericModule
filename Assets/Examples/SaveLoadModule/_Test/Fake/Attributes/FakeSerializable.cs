using System;


[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Property)]
public class FakeSerializable : Attribute
{
}