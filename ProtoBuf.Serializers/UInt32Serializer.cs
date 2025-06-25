using System;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers;

internal sealed class UInt32Serializer : IProtoSerializer
{
	private static readonly Type expectedType = typeof(uint);

	bool IProtoSerializer.RequiresOldValue => false;

	bool IProtoSerializer.ReturnsValue => true;

	public Type ExpectedType => expectedType;

	public UInt32Serializer(TypeModel model)
	{
	}

	public object Read(object value, ProtoReader source)
	{
		return source.ReadUInt32();
	}

	public void Write(object value, ProtoWriter dest)
	{
		ProtoWriter.WriteUInt32((uint)value, dest);
	}
}
