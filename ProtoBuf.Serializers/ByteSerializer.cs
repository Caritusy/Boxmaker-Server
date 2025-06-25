using System;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers;

internal sealed class ByteSerializer : IProtoSerializer
{
	private static readonly Type expectedType = typeof(byte);

	bool IProtoSerializer.RequiresOldValue => false;

	bool IProtoSerializer.ReturnsValue => true;

	public Type ExpectedType => expectedType;

	public ByteSerializer(TypeModel model)
	{
	}

	public void Write(object value, ProtoWriter dest)
	{
		ProtoWriter.WriteByte((byte)value, dest);
	}

	public object Read(object value, ProtoReader source)
	{
		return source.ReadByte();
	}
}
