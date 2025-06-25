using System;
using System.ComponentModel;

namespace ProtoBuf;

[ImmutableObject(true)]
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class ProtoAfterSerializationAttribute : Attribute
{
}
