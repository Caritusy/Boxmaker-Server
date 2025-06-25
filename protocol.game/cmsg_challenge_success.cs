using System;
using System.ComponentModel;
using ProtoBuf;

namespace protocol.game;

[Serializable]
[ProtoContract(Name = "cmsg_challenge_success")]
public class cmsg_challenge_success : IExtensible
{
	private msg_common _common;

	private int _point;

	private int _time;

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

	[ProtoMember(2, IsRequired = false, Name = "point", DataFormat = DataFormat.TwosComplement)]
	[DefaultValue(0)]
	public int point
	{
		get
		{
			return _point;
		}
		set
		{
			_point = value;
		}
	}

	[ProtoMember(3, IsRequired = false, Name = "time", DataFormat = DataFormat.TwosComplement)]
	[DefaultValue(0)]
	public int time
	{
		get
		{
			return _time;
		}
		set
		{
			_time = value;
		}
	}

	IExtension IExtensible.GetExtensionObject(bool createIfMissing)
	{
		return Extensible.GetExtensionObject(ref extensionObject, createIfMissing);
	}
}
