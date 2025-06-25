using System;
using System.ComponentModel;
using ProtoBuf;

namespace protocol.game;

[Serializable]
[ProtoContract(Name = "msg_life_error")]
public class msg_life_error : IExtensible
{
	private ulong _server_time;

	private ulong _life_time;

	private IExtension extensionObject;

	[ProtoMember(1, IsRequired = false, Name = "server_time", DataFormat = DataFormat.TwosComplement)]
	[DefaultValue(0f)]
	public ulong server_time
	{
		get
		{
			return _server_time;
		}
		set
		{
			_server_time = value;
		}
	}

	[ProtoMember(2, IsRequired = false, Name = "life_time", DataFormat = DataFormat.TwosComplement)]
	[DefaultValue(0f)]
	public ulong life_time
	{
		get
		{
			return _life_time;
		}
		set
		{
			_life_time = value;
		}
	}

	IExtension IExtensible.GetExtensionObject(bool createIfMissing)
	{
		return Extensible.GetExtensionObject(ref extensionObject, createIfMissing);
	}
}
