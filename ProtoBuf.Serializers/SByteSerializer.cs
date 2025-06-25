using System;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers;

internal sealed class SByteSerializer : IProtoSerializer
{
	private static readonly Type expectedType = typeof(sbyte);

	bool IProtoSerializer.RequiresOldValue => false;

	bool IProtoSerializer.ReturnsValue => true;

	public Type ExpectedType => expectedType;

	public SByteSerializer(TypeModel model)
	{
	}

	public object Read(object value, ProtoReader source)
	{
		return source.ReadSByte();
	}

	public void Write(object value, ProtoWriter dest)
	{
		ProtoWriter.WriteSByte((sbyte)value, dest);
	}
}
