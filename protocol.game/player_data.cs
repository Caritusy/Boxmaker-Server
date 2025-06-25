using System;
using System.ComponentModel;
using ProtoBuf;

namespace protocol.game;

[Serializable]
[ProtoContract(Name = "player_data")]
public class player_data : IExtensible
{
	private int _userid;

	private string _name = string.Empty;

	private string _country = string.Empty;

	private int _head;

	private int _level;

	private int _exp;

	private string _register = string.Empty;

	private int _amount;

	private int _pas;

	private int _point;

	private int _comment;

	private int _video;

	private int _visitor;

	private int _watched;

	private int _mlevel;

	private int _mexp;

	private IExtension extensionObject;

	[ProtoMember(1, IsRequired = false, Name = "userid", DataFormat = DataFormat.TwosComplement)]
	[DefaultValue(0)]
	public int userid
	{
		get
		{
			return _userid;
		}
		set
		{
			_userid = value;
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

	[ProtoMember(3, IsRequired = false, Name = "country", DataFormat = DataFormat.Default)]
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
	[ProtoMember(4, IsRequired = false, Name = "head", DataFormat = DataFormat.TwosComplement)]
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

	[DefaultValue(0)]
	[ProtoMember(5, IsRequired = false, Name = "level", DataFormat = DataFormat.TwosComplement)]
	public int level
	{
		get
		{
			return _level;
		}
		set
		{
			_level = value;
		}
	}

	[ProtoMember(6, IsRequired = false, Name = "exp", DataFormat = DataFormat.TwosComplement)]
	[DefaultValue(0)]
	public int exp
	{
		get
		{
			return _exp;
		}
		set
		{
			_exp = value;
		}
	}

	[ProtoMember(7, IsRequired = false, Name = "register", DataFormat = DataFormat.Default)]
	[DefaultValue("")]
	public string register
	{
		get
		{
			return _register;
		}
		set
		{
			_register = value;
		}
	}

	[ProtoMember(8, IsRequired = false, Name = "amount", DataFormat = DataFormat.TwosComplement)]
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

	[DefaultValue(0)]
	[ProtoMember(9, IsRequired = false, Name = "pas", DataFormat = DataFormat.TwosComplement)]
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

	[ProtoMember(10, IsRequired = false, Name = "point", DataFormat = DataFormat.TwosComplement)]
	[DefaultValue(0)]
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

	[DefaultValue(0)]
	[ProtoMember(11, IsRequired = false, Name = "comment", DataFormat = DataFormat.TwosComplement)]
	public int comment
	{
		get
		{
			return _comment;
		}
		set
		{
			_comment = value;
		}
	}

	[DefaultValue(0)]
	[ProtoMember(12, IsRequired = false, Name = "video", DataFormat = DataFormat.TwosComplement)]
	public int video
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
	[ProtoMember(13, IsRequired = false, Name = "visitor", DataFormat = DataFormat.TwosComplement)]
	public int visitor
	{
		get
		{
			return _visitor;
		}
		set
		{
			_visitor = value;
		}
	}

	[ProtoMember(14, IsRequired = false, Name = "watched", DataFormat = DataFormat.TwosComplement)]
	[DefaultValue(0)]
	public int watched
	{
		get
		{
			return _watched;
		}
		set
		{
			_watched = value;
		}
	}

	[ProtoMember(15, IsRequired = false, Name = "mlevel", DataFormat = DataFormat.TwosComplement)]
	[DefaultValue(0)]
	public int mlevel
	{
		get
		{
			return _mlevel;
		}
		set
		{
			_mlevel = value;
		}
	}

	[DefaultValue(0)]
	[ProtoMember(16, IsRequired = false, Name = "mexp", DataFormat = DataFormat.TwosComplement)]
	public int mexp
	{
		get
		{
			return _mexp;
		}
		set
		{
			_mexp = value;
		}
	}

	IExtension IExtensible.GetExtensionObject(bool createIfMissing)
	{
		return Extensible.GetExtensionObject(ref extensionObject, createIfMissing);
	}
}
