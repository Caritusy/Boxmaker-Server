using System;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers;

internal sealed class BooleanSerializer : IProtoSerializer
{
	private static readonly Type expectedType = typeof(bool);

	bool IProtoSerializer.RequiresOldValue => false;

	bool IProtoSerializer.ReturnsValue => true;

	public Type ExpectedType => expectedType;

	public BooleanSerializer(TypeModel model)
	{
	}

	public void Write(object value, ProtoWriter dest)
	{
		ProtoWriter.WriteBoolean((bool)value, dest);
	}

	public object Read(object value, ProtoReader source)
	{
		return source.ReadBoolean();
	}
}
