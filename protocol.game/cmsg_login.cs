using System;
using System.ComponentModel;
using ProtoBuf;

namespace protocol.game;

[Serializable]
[ProtoContract(Name = "cmsg_login")]
public class cmsg_login : IExtensible
{
	private string _openid = string.Empty;

	private string _openkey = string.Empty;

	private string _nationality = string.Empty;

	private string _ver = string.Empty;

	private string _channel = string.Empty;

	private IExtension extensionObject;

	[DefaultValue("")]
	[ProtoMember(1, IsRequired = false, Name = "openid", DataFormat = DataFormat.Default)]
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

	[ProtoMember(2, IsRequired = false, Name = "openkey", DataFormat = DataFormat.Default)]
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

	[DefaultValue("")]
	[ProtoMember(3, IsRequired = false, Name = "nationality", DataFormat = DataFormat.Default)]
	public string nationality
	{
		get
		{
			return _nationality;
		}
		set
		{
			_nationality = value;
		}
	}

	[DefaultValue("")]
	[ProtoMember(4, IsRequired = false, Name = "ver", DataFormat = DataFormat.Default)]
	public string ver
	{
		get
		{
			return _ver;
		}
		set
		{
			_ver = value;
		}
	}

	[DefaultValue("")]
	[ProtoMember(5, IsRequired = false, Name = "channel", DataFormat = DataFormat.Default)]
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
