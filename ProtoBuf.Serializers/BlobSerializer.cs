using System;
using ProtoBuf.Meta;

namespace ProtoBuf.Serializers;

internal sealed class BlobSerializer : IProtoSerializer
{
	private static readonly Type expectedType = typeof(byte[]);

	private readonly bool overwriteList;

	bool IProtoSerializer.RequiresOldValue => !overwriteList;

	bool IProtoSerializer.ReturnsValue => true;

	public Type ExpectedType => expectedType;

	public BlobSerializer(TypeModel model, bool overwriteList)
	{
		this.overwriteList = overwriteList;
	}

	public object Read(object value, ProtoReader source)
	{
		return ProtoReader.AppendBytes((!overwriteList) ? ((byte[])value) : null, source);
	}

	public void Write(object value, ProtoWriter dest)
	{
		ProtoWriter.WriteBytes((byte[])value, dest);
	}
}
