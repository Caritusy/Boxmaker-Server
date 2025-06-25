using System;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers;

internal sealed class StringSerializer : IProtoSerializer
{
	private static readonly Type expectedType = typeof(string);

	bool IProtoSerializer.RequiresOldValue => false;

	bool IProtoSerializer.ReturnsValue => true;

	public Type ExpectedType => expectedType;

	public StringSerializer(TypeModel model)
	{
	}

	public void Write(object value, ProtoWriter dest)
	{
		ProtoWriter.WriteString((string)value, dest);
	}

	public object Read(object value, ProtoReader source)
	{
		return source.ReadString();
	}
}
