using System;
using System.ComponentModel;
using ProtoBuf;

namespace protocol.game;

[Serializable]
[ProtoContract(Name = "cmsg_register")]
public class cmsg_register : IExtensible
{
	private msg_common _common;

	private string _openid = string.Empty;

	private string _openkey = string.Empty;

	private string _nationality = string.Empty;

	private string _nickname = string.Empty;

	private int _head;

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

	[DefaultValue("")]
	[ProtoMember(3, IsRequired = false, Name = "openkey", DataFormat = DataFormat.Default)]
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
	[ProtoMember(5, IsRequired = false, Name = "nickname", DataFormat = DataFormat.Default)]
	public string nickname
	{
		get
		{
			return _nickname;
		}
		set
		{
			_nickname = value;
		}
	}

	[DefaultValue(0)]
	[ProtoMember(6, IsRequired = false, Name = "head", DataFormat = DataFormat.TwosComplement)]
	public int head
	{
		get
		{
			return _head;
		}
		set
		{
			_head = value;
		}
	}

	IExtension IExtensible.GetExtensionObject(bool createIfMissing)
	{
		return Extensible.GetExtensionObject(ref extensionObject, createIfMissing);
	}
}
