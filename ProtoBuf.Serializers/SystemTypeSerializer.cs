using System;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers;

internal sealed class SystemTypeSerializer : IProtoSerializer
{
	private static readonly Type expectedType = typeof(Type);

	bool IProtoSerializer.RequiresOldValue => false;

	bool IProtoSerializer.ReturnsValue => true;

	public Type ExpectedType => expectedType;

	public SystemTypeSerializer(TypeModel model)
	{
	}

	void IProtoSerializer.Write(object value, ProtoWriter dest)
	{
		ProtoWriter.WriteType((Type)value, dest);
	}

	object IProtoSerializer.Read(object value, ProtoReader source)
	{
		return source.ReadType();
	}
}
