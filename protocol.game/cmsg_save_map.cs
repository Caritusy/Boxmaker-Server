using System;
using ProtoBuf;

namespace protocol.game;

[Serializable]
[ProtoContract(Name = "cmsg_save_map")]
public class cmsg_save_map : IExtensible
{
	private msg_common _common;

	private int _id;

	private byte[] _url;

	private byte[] _mapdata;

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

	[ProtoMember(3, IsRequired = true, Name = "url", DataFormat = DataFormat.Default)]
	public byte[] url
	{
		get
		{
			return _url;
		}
		set
		{
			_url = value;
		}
	}

	[ProtoMember(4, IsRequired = true, Name = "mapdata", DataFormat = DataFormat.Default)]
	public byte[] mapdata
	{
		get
		{
			return _mapdata;
		}
		set
		{
			_mapdata = value;
		}
	}

	IExtension IExtensible.GetExtensionObject(bool createIfMissing)
	{
		return Extensible.GetExtensionObject(ref extensionObject, createIfMissing);
	}
}
