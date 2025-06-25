using System;
using System.ComponentModel;
using ProtoBuf;

namespace protocol.game;

[Serializable]
[ProtoContract(Name = "map_show")]
public class map_show : IExtensible
{
	private int _id;

	private string _name = string.Empty;

	private byte[] _url;

	private int _amount;

	private int _pas;

	private int _collect;

	private int _finish;

	private int _difficulty;

	private int _like;

	private IExtension extensionObject;

	[DefaultValue(0)]
	[ProtoMember(1, IsRequired = false, Name = "id", DataFormat = DataFormat.TwosComplement)]
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

	[DefaultValue(null)]
	[ProtoMember(3, IsRequired = false, Name = "url", DataFormat = DataFormat.Default)]
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

	[ProtoMember(4, IsRequired = false, Name = "amount", DataFormat = DataFormat.TwosComplement)]
	[DefaultValue(0)]
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

	[ProtoMember(5, IsRequired = false, Name = "pas", DataFormat = DataFormat.TwosComplement)]
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

	[DefaultValue(0)]
	[ProtoMember(6, IsRequired = false, Name = "collect", DataFormat = DataFormat.TwosComplement)]
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

	[DefaultValue(0)]
	[ProtoMember(7, IsRequired = false, Name = "finish", DataFormat = DataFormat.TwosComplement)]
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
	[ProtoMember(8, IsRequired = false, Name = "difficulty", DataFormat = DataFormat.TwosComplement)]
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
	[ProtoMember(9, IsRequired = false, Name = "like", DataFormat = DataFormat.TwosComplement)]
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
