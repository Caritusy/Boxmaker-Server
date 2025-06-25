using System;
using System.ComponentModel;
using ProtoBuf;

namespace protocol.game;

[Serializable]
[ProtoContract(Name = "challenge_rank")]
public class challenge_rank : IExtensible
{
	private int _user_head;

	private string _user_name = string.Empty;

	private string _user_country = string.Empty;

	private int _user_id;

	private int _user_visitor;

	private int _user_level;

	private int _user_index;

	private int _user_life;

	private IExtension extensionObject;

	[DefaultValue(0)]
	[ProtoMember(1, IsRequired = false, Name = "user_head", DataFormat = DataFormat.TwosComplement)]
	public int user_head
	{
		get
		{
			return _user_head;
		}
		set
		{
			_user_head = value;
		}
	}

	[DefaultValue("")]
	[ProtoMember(2, IsRequired = false, Name = "user_name", DataFormat = DataFormat.Default)]
	public string user_name
	{
		get
		{
			return _user_name;
		}
		set
		{
			_user_name = value;
		}
	}

	[DefaultValue("")]
	[ProtoMember(3, IsRequired = false, Name = "user_country", DataFormat = DataFormat.Default)]
	public string user_country
	{
		get
		{
			return _user_country;
		}
		set
		{
			_user_country = value;
		}
	}

	[DefaultValue(0)]
	[ProtoMember(4, IsRequired = false, Name = "user_id", DataFormat = DataFormat.TwosComplement)]
	public int user_id
	{
		get
		{
			return _user_id;
		}
		set
		{
			_user_id = value;
		}
	}

	[ProtoMember(5, IsRequired = false, Name = "user_visitor", DataFormat = DataFormat.TwosComplement)]
	[DefaultValue(0)]
	public int user_visitor
	{
		get
		{
			return _user_visitor;
		}
		set
		{
			_user_visitor = value;
		}
	}

	[DefaultValue(0)]
	[ProtoMember(6, IsRequired = false, Name = "user_level", DataFormat = DataFormat.TwosComplement)]
	public int user_level
	{
		get
		{
			return _user_level;
		}
		set
		{
			_user_level = value;
		}
	}

	[DefaultValue(0)]
	[ProtoMember(7, IsRequired = false, Name = "user_index", DataFormat = DataFormat.TwosComplement)]
	public int user_index
	{
		get
		{
			return _user_index;
		}
		set
		{
			_user_index = value;
		}
	}

	[DefaultValue(0)]
	[ProtoMember(8, IsRequired = false, Name = "user_life", DataFormat = DataFormat.TwosComplement)]
	public int user_life
	{
		get
		{
			return _user_life;
		}
		set
		{
			_user_life = value;
		}
	}

	IExtension IExtensible.GetExtensionObject(bool createIfMissing)
	{
		return Extensible.GetExtensionObject(ref extensionObject, createIfMissing);
	}
}
