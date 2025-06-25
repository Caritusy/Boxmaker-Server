using System;
using System.ComponentModel;
using ProtoBuf;

namespace protocol.game;

[Serializable]
[ProtoContract(Name = "cmsg_upload_map")]
public class cmsg_upload_map : IExtensible
{
	private msg_common _common;

	private int _id;

	private int _ver;

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

	[ProtoMember(2, IsRequired = true, Name = "id", DataFormat = DataFormat.TwosComplement)]
	public int id
	{
		get
		{
			return _id;
		}
		set
		{
			_id = value;
		}
	}

	[DefaultValue(0)]
	[ProtoMember(3, IsRequired = false, Name = "ver", DataFormat = DataFormat.TwosComplement)]
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

	[ProtoMember(4, IsRequired = false, Name = "video", DataFormat = DataFormat.Default)]
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
	[ProtoMember(5, IsRequired = false, Name = "time", DataFormat = DataFormat.TwosComplement)]
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
