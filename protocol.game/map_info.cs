using System;
using System.ComponentModel;
using ProtoBuf;

namespace protocol.game;

[Serializable]
[ProtoContract(Name = "map_info")]
public class map_info : IExtensible
{
	private int _id;

	private string _name = string.Empty;

	private byte[] _url;

	private int _owner_id;

	private string _owner_name = string.Empty;

	private string _country = string.Empty;

	private int _head;

	private int _favorite;

	private int _amount;

	private int _pas;

	private string _date = string.Empty;

	private int _collect;

	private int _finish;

	private int _difficulty;

	private int _like;

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

	[ProtoMember(4, IsRequired = false, Name = "owner_id", DataFormat = DataFormat.TwosComplement)]
	[DefaultValue(0)]
	public int owner_id
	{
		get
		{
			return _owner_id;
		}
		set
		{
			_owner_id = value;
		}
	}

	[ProtoMember(5, IsRequired = false, Name = "owner_name", DataFormat = DataFormat.Default)]
	[DefaultValue("")]
	public string owner_name
	{
		get
		{
			return _owner_name;
		}
		set
		{
			_owner_name = value;
		}
	}

	[ProtoMember(6, IsRequired = false, Name = "country", DataFormat = DataFormat.Default)]
	[DefaultValue("")]
	public string country
	{
		get
		{
			return _country;
		}
		set
		{
			_country = value;
		}
	}

	[DefaultValue(0)]
	[ProtoMember(7, IsRequired = false, Name = "head", DataFormat = DataFormat.TwosComplement)]
	public int head
	{
		get
		{
			return _head;
		}
		set
		{
			_head = value;
		}
	}

	[ProtoMember(8, IsRequired = false, Name = "favorite", DataFormat = DataFormat.TwosComplement)]
	[DefaultValue(0)]
	public int favorite
	{
		get
		{
			return _favorite;
		}
		set
		{
			_favorite = value;
		}
	}

	[DefaultValue(0)]
	[ProtoMember(9, IsRequired = false, Name = "amount", DataFormat = DataFormat.TwosComplement)]
	public int amount
	{
		get
		{
			return _amount;
		}
		set
		{
			_amount = value;
		}
	}

	[ProtoMember(10, IsRequired = false, Name = "pas", DataFormat = DataFormat.TwosComplement)]
	[DefaultValue(0)]
	public int pas
	{
		get
		{
			return _pas;
		}
		set
		{
			_pas = value;
		}
	}

	[ProtoMember(11, IsRequired = false, Name = "date", DataFormat = DataFormat.Default)]
	[DefaultValue("")]
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

	[ProtoMember(12, IsRequired = false, Name = "collect", DataFormat = DataFormat.TwosComplement)]
	[DefaultValue(0)]
	public int collect
	{
		get
		{
			return _collect;
		}
		set
		{
			_collect = value;
		}
	}

	[ProtoMember(13, IsRequired = false, Name = "finish", DataFormat = DataFormat.TwosComplement)]
	[DefaultValue(0)]
	public int finish
	{
		get
		{
			return _finish;
		}
		set
		{
			_finish = value;
		}
	}

	[DefaultValue(0)]
	[ProtoMember(14, IsRequired = false, Name = "difficulty", DataFormat = DataFormat.TwosComplement)]
	public int difficulty
	{
		get
		{
			return _difficulty;
		}
		set
		{
			_difficulty = value;
		}
	}

	[DefaultValue(0)]
	[ProtoMember(15, IsRequired = false, Name = "like", DataFormat = DataFormat.TwosComplement)]
	public int like
	{
		get
		{
			return _like;
		}
		set
		{
			_like = value;
		}
	}

	IExtension IExtensible.GetExtensionObject(bool createIfMissing)
	{
		return Extensible.GetExtensionObject(ref extensionObject, createIfMissing);
	}
}
