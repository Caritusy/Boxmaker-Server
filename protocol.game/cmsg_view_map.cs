using System;
using System.ComponentModel;
using ProtoBuf;

namespace protocol.game;

[Serializable]
[ProtoContract(Name = "cmsg_view_map")]
public class cmsg_view_map : IExtensible
{
	private msg_common _common;

	private int _type;

	private int _index;

	private int _ver;

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

	[ProtoMember(2, IsRequired = false, Name = "type", DataFormat = DataFormat.TwosComplement)]
	[DefaultValue(0)]
	public int type
	{
		get
		{
			return _type;
		}
		set
		{
			_type = value;
		}
	}

	[ProtoMember(3, IsRequired = false, Name = "index", DataFormat = DataFormat.TwosComplement)]
	[DefaultValue(0)]
	public int index
	{
		get
		{
			return _index;
		}
		set
		{
			_index = value;
		}
	}

	[ProtoMember(4, IsRequired = false, Name = "ver", DataFormat = DataFormat.TwosComplement)]
	[DefaultValue(0)]
	public int ver
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
