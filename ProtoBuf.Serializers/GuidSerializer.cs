using System;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers;

internal sealed class GuidSerializer : IProtoSerializer
{
	private static readonly Type expectedType = typeof(Guid);

	bool IProtoSerializer.RequiresOldValue => false;

	bool IProtoSerializer.ReturnsValue => true;

	public Type ExpectedType => expectedType;

	public GuidSerializer(TypeModel model)
	{
	}

	public void Write(object value, ProtoWriter dest)
	{
		BclHelpers.WriteGuid((Guid)value, dest);
	}

	public object Read(object value, ProtoReader source)
	{
		return BclHelpers.ReadGuid(source);
	}
}
