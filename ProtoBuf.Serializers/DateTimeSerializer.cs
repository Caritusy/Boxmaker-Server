using System;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers;

internal sealed class DateTimeSerializer : IProtoSerializer
{
	private static readonly Type expectedType = typeof(DateTime);

	private readonly bool includeKind;

	bool IProtoSerializer.RequiresOldValue => false;

	bool IProtoSerializer.ReturnsValue => true;

	public Type ExpectedType => expectedType;

	public DateTimeSerializer(TypeModel model)
	{
		includeKind = model?.SerializeDateTimeKind() ?? false;
	}

	public object Read(object value, ProtoReader source)
	{
		return BclHelpers.ReadDateTime(source);
	}

	public void Write(object value, ProtoWriter dest)
	{
		if (includeKind)
		{
			BclHelpers.WriteDateTimeWithKind((DateTime)value, dest);
		}
		else
		{
			BclHelpers.WriteDateTime((DateTime)value, dest);
		}
	}
}
