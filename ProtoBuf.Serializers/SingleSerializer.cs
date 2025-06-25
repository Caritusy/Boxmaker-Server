using System;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers;

internal sealed class SingleSerializer : IProtoSerializer
{
	private static readonly Type expectedType = typeof(float);

	bool IProtoSerializer.RequiresOldValue => false;

	bool IProtoSerializer.ReturnsValue => true;

	public Type ExpectedType => expectedType;

	public SingleSerializer(TypeModel model)
	{
	}

	public object Read(object value, ProtoReader source)
	{
		return source.ReadSingle();
	}

	public void Write(object value, ProtoWriter dest)
	{
		ProtoWriter.WriteSingle((float)value, dest);
	}
}
