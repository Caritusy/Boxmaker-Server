using System;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers;

internal sealed class DecimalSerializer : IProtoSerializer
{
	private static readonly Type expectedType = typeof(decimal);

	bool IProtoSerializer.RequiresOldValue => false;

	bool IProtoSerializer.ReturnsValue => true;

	public Type ExpectedType => expectedType;

	public DecimalSerializer(TypeModel model)
	{
	}

	public object Read(object value, ProtoReader source)
	{
		return BclHelpers.ReadDecimal(source);
	}

	public void Write(object value, ProtoWriter dest)
	{
		BclHelpers.WriteDecimal((decimal)value, dest);
	}
}
