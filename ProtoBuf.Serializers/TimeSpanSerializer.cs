using System;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers;

internal sealed class TimeSpanSerializer : IProtoSerializer
{
	private static readonly Type expectedType = typeof(TimeSpan);

	bool IProtoSerializer.RequiresOldValue => false;

	bool IProtoSerializer.ReturnsValue => true;

	public Type ExpectedType => expectedType;

	public TimeSpanSerializer(TypeModel model)
	{
	}

	public object Read(object value, ProtoReader source)
	{
		return BclHelpers.ReadTimeSpan(source);
	}

	public void Write(object value, ProtoWriter dest)
	{
		BclHelpers.WriteTimeSpan((TimeSpan)value, dest);
	}
}
