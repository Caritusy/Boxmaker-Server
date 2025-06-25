using System;
using System.ComponentModel;
using ProtoBuf;

namespace protocol.game;

[Serializable]
[ProtoContract(Name = "edit_data")]
public class edit_data : IExtensible
{
	private int _id;

	private string _name = string.Empty;

	private byte[] _url;

	private string _date = string.Empty;

	private int _upload;

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

	[DefaultValue("")]
	[ProtoMember(2, IsRequired = false, Name = "name", DataFormat = DataFormat.Default)]
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

	[DefaultValue("")]
	[ProtoMember(4, IsRequired = false, Name = "date", DataFormat = DataFormat.Default)]
	public string date
	{
		get
		{
			return _date;
		}
		set
		{
			_date = value;
		}
	}

	[ProtoMember(5, IsRequired = false, Name = "upload", DataFormat = DataFormat.TwosComplement)]
	[DefaultValue(0)]
	public int upload
	{
		get
		{
			return _upload;
		}
		set
		{
			_upload = value;
		}
	}

	IExtension IExtensible.GetExtensionObject(bool createIfMissing)
	{
		return Extensible.GetExtensionObject(ref extensionObject, createIfMissing);
	}
}
