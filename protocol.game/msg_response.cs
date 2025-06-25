using System;
using System.ComponentModel;
using ProtoBuf;

namespace protocol.game;

[Serializable]
[ProtoContract(Name = "msg_response")]
public class msg_response : IExtensible
{
	private int _res;

	private byte[] _error;

	private byte[] _msg;

	private IExtension extensionObject;

	[DefaultValue(0)]
	[ProtoMember(1, IsRequired = false, Name = "res", DataFormat = DataFormat.TwosComplement)]
	public int res
	{
		get
		{
			return _res;
		}
		set
		{
			_res = value;
		}
	}

	[DefaultValue(null)]
	[ProtoMember(2, IsRequired = false, Name = "error", DataFormat = DataFormat.Default)]
	public byte[] error
	{
		get
		{
			return _error;
		}
		set
		{
			_error = value;
		}
	}

	[DefaultValue(null)]
	[ProtoMember(3, IsRequired = false, Name = "msg", DataFormat = DataFormat.Default)]
	public byte[] msg
	{
		get
		{
			return _msg;
		}
		set
		{
			_msg = value;
		}
	}

	IExtension IExtensible.GetExtensionObject(bool createIfMissing)
	{
		return Extensible.GetExtensionObject(ref extensionObject, createIfMissing);
	}
}
