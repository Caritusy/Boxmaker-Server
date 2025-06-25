using System;
using System.ComponentModel;
using ProtoBuf;

namespace protocol.game;

[Serializable]
[ProtoContract(Name = "map_upload")]
public class map_upload : IExtensible
{
	private int _id;

	private string _name = string.Empty;

	private byte[] _url;

	private string _time = string.Empty;

	private IExtension extensionObject;

	[ProtoMember(1, IsRequired = false, Name = "id", DataFormat = DataFormat.TwosComplement)]
	[DefaultValue(0)]
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

	[ProtoMember(2, IsRequired = false, Name = "name", DataFormat = DataFormat.Default)]
	[DefaultValue("")]
	public string name
	{
		get
		{
			return _name;
		}
		set
		{
			_name = value;
		}
	}

	[ProtoMember(3, IsRequired = false, Name = "url", DataFormat = DataFormat.Default)]
	[DefaultValue(null)]
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

	[ProtoMember(4, IsRequired = false, Name = "time", DataFormat = DataFormat.Default)]
	[DefaultValue("")]
	public string time
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
