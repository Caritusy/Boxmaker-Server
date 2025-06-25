using System;
using System.ComponentModel;
using ProtoBuf;

namespace protocol.game;

[Serializable]
[ProtoContract(Name = "cmsg_change_account")]
public class cmsg_change_account : IExtensible
{
	private msg_common _common;

	private string _openid = string.Empty;

	private string _openkey = string.Empty;

	private string _channel = string.Empty;

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

	[DefaultValue("")]
	[ProtoMember(2, IsRequired = false, Name = "openid", DataFormat = DataFormat.Default)]
	public string openid
	{
		get
		{
			return _openid;
		}
		set
		{
			_openid = value;
		}
	}

	[ProtoMember(3, IsRequired = false, Name = "openkey", DataFormat = DataFormat.Default)]
	[DefaultValue("")]
	public string openkey
	{
		get
		{
			return _openkey;
		}
		set
		{
			_openkey = value;
		}
	}

	[ProtoMember(4, IsRequired = false, Name = "channel", DataFormat = DataFormat.Default)]
	[DefaultValue("")]
	public string channel
	{
		get
		{
			return _channel;
		}
		set
		{
			_channel = value;
		}
	}

	IExtension IExtensible.GetExtensionObject(bool createIfMissing)
	{
		return Extensible.GetExtensionObject(ref extensionObject, createIfMissing);
	}
}
