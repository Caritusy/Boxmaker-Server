using System;
using System.ComponentModel;
using ProtoBuf;

namespace protocol.game;

[Serializable]
[ProtoContract(Name = "cmsg_challenge_fail")]
public class cmsg_challenge_fail : IExtensible
{
	private msg_common _common;

	private int _x;

	private int _y;

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

	[ProtoMember(2, IsRequired = false, Name = "x", DataFormat = DataFormat.TwosComplement)]
	[DefaultValue(0)]
	public int x
	{
		get
		{
			return _x;
		}
		set
		{
			_x = value;
		}
	}

	[DefaultValue(0)]
	[ProtoMember(3, IsRequired = false, Name = "y", DataFormat = DataFormat.TwosComplement)]
	public int y
	{
		get
		{
			return _y;
		}
		set
		{
			_y = value;
		}
	}

	IExtension IExtensible.GetExtensionObject(bool createIfMissing)
	{
		return Extensible.GetExtensionObject(ref extensionObject, createIfMissing);
	}
}
