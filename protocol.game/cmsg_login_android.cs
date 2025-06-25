using System;
using System.ComponentModel;
using ProtoBuf;

namespace protocol.game;

[Serializable]
[ProtoContract(Name = "cmsg_login_android")]
public class cmsg_login_android : IExtensible
{
	private string _userid = string.Empty;

	private string _token = string.Empty;

	private string _channel = string.Empty;

	private string _nationality = string.Empty;

	private string _ver = string.Empty;

	private IExtension extensionObject;

	[ProtoMember(1, IsRequired = false, Name = "userid", DataFormat = DataFormat.Default)]
	[DefaultValue("")]
	public string userid
	{
		get
		{
			return _userid;
		}
		set
		{
			_userid = value;
		}
	}

	[DefaultValue("")]
	[ProtoMember(2, IsRequired = false, Name = "token", DataFormat = DataFormat.Default)]
	public string token
	{
		get
		{
			return _token;
		}
		set
		{
			_token = value;
		}
	}

	[ProtoMember(3, IsRequired = false, Name = "channel", DataFormat = DataFormat.Default)]
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

	[DefaultValue("")]
	[ProtoMember(4, IsRequired = false, Name = "nationality", DataFormat = DataFormat.Default)]
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
	[ProtoMember(5, IsRequired = false, Name = "ver", DataFormat = DataFormat.Default)]
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

	IExtension IExtensible.GetExtensionObject(bool createIfMissing)
	{
		return Extensible.GetExtensionObject(ref extensionObject, createIfMissing);
	}
}
