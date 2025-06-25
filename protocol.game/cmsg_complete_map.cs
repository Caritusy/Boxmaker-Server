using System;
using System.ComponentModel;
using ProtoBuf;

namespace protocol.game;

[Serializable]
[ProtoContract(Name = "cmsg_complete_map")]
public class cmsg_complete_map : IExtensible
{
	private msg_common _common;

	private int _suc;

	private int _x;

	private int _y;

	private int _point;

	private byte[] _video;

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

	[ProtoMember(3, IsRequired = true, Name = "suc", DataFormat = DataFormat.TwosComplement)]
	public int suc
	{
		get
		{
			return _suc;
		}
		set
		{
			_suc = value;
		}
	}

	[ProtoMember(4, IsRequired = false, Name = "x", DataFormat = DataFormat.TwosComplement)]
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

	[ProtoMember(5, IsRequired = false, Name = "y", DataFormat = DataFormat.TwosComplement)]
	[DefaultValue(0)]
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

	[DefaultValue(0)]
	[ProtoMember(6, IsRequired = false, Name = "point", DataFormat = DataFormat.TwosComplement)]
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

	[ProtoMember(7, IsRequired = false, Name = "video", DataFormat = DataFormat.Default)]
	[DefaultValue(null)]
	public byte[] video
	{
		get
		{
			return _video;
		}
		set
		{
			_video = value;
		}
	}

	[DefaultValue(0)]
	[ProtoMember(8, IsRequired = false, Name = "time", DataFormat = DataFormat.TwosComplement)]
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
