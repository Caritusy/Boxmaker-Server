using System;
using ProtoBuf;

namespace protocol.game;

[Serializable]
[ProtoContract(Name = "cmsg_complete_guide")]
public class cmsg_complete_guide : IExtensible
{
	private msg_common _common;

	private byte[] _url;

	private byte[] _data;

	private IExtension extensionObject;

	[ProtoMember(1, IsRequired = true, Name = "common", DataFormat = DataFormat.Default)]
	public msg_common common
	{
		get
		{
			return _common;
		}
		set
		{
			_common = value;
		}
	}

	[ProtoMember(2, IsRequired = true, Name = "url", DataFormat = DataFormat.Default)]
	public byte[] url
	{
		get
		{
			return _url;
		}
		set
		{
			_url = value;
		}
	}

	[ProtoMember(3, IsRequired = true, Name = "data", DataFormat = DataFormat.Default)]
	public byte[] data
	{
		get
		{
			return _data;
		}
		set
		{
			_data = value;
		}
	}

	IExtension IExtensible.GetExtensionObject(bool createIfMissing)
	{
		return Extensible.GetExtensionObject(ref extensionObject, createIfMissing);
	}
}
