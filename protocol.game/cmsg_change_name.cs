using System;
using System.ComponentModel;
using ProtoBuf;

namespace protocol.game;

[Serializable]
[ProtoContract(Name = "cmsg_change_name")]
public class cmsg_change_name : IExtensible
{
	private msg_common _common;

	private int _head;

	private string _nickname = string.Empty;

	private string _nationality = string.Empty;

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

	[ProtoMember(2, IsRequired = false, Name = "head", DataFormat = DataFormat.TwosComplement)]
	[DefaultValue(0)]
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

	[ProtoMember(3, IsRequired = false, Name = "nickname", DataFormat = DataFormat.Default)]
	[DefaultValue("")]
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

	IExtension IExtensible.GetExtensionObject(bool createIfMissing)
	{
		return Extensible.GetExtensionObject(ref extensionObject, createIfMissing);
	}
}
