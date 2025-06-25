using System;
using System.ComponentModel;
using ProtoBuf;

namespace protocol.game;

[Serializable]
[ProtoContract(Name = "smsg_login")]
public class smsg_login : IExtensible
{
	private string _openid = string.Empty;

	private string _openkey = string.Empty;

	private string _sig = string.Empty;

	private int _userid;

	private string _name = string.Empty;

	private string _nationality = string.Empty;

	private int _visitor;

	private int _head;

	private int _level;

	private int _exp;

	private int _life;

	private int _jewel;

	private ulong _server_time;

	private ulong _life_time;

	private int _upload;

	private int _testify;

	private ulong _exp_time;

	private int _guide;

	private int _mapid;

	private int _support;

	private int _review;

	private string _notify_uri = string.Empty;

	private int _init_life;

	private int _life_per_time;

	private int _challenge_start;

	private int _download_num;

	private int _download_max;

	private IExtension extensionObject;

	[DefaultValue("")]
	[ProtoMember(1, IsRequired = false, Name = "openid", DataFormat = DataFormat.Default)]
	public string openid
	{
		get
		{
			return _openid;
		}
		set
		{
			_openid = value;
		}
	}

	[DefaultValue("")]
	[ProtoMember(2, IsRequired = false, Name = "openkey", DataFormat = DataFormat.Default)]
	public string openkey
	{
		get
		{
			return _openkey;
		}
		set
		{
			_openkey = value;
		}
	}

	[ProtoMember(3, IsRequired = false, Name = "sig", DataFormat = DataFormat.Default)]
	[DefaultValue("")]
	public string sig
	{
		get
		{
			return _sig;
		}
		set
		{
			_sig = value;
		}
	}

	[DefaultValue(0)]
	[ProtoMember(4, IsRequired = false, Name = "userid", DataFormat = DataFormat.TwosComplement)]
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
	[ProtoMember(5, IsRequired = false, Name = "name", DataFormat = DataFormat.Default)]
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

	[DefaultValue("")]
	[ProtoMember(6, IsRequired = false, Name = "nationality", DataFormat = DataFormat.Default)]
	public string nationality
	{
		get
		{
			return _nationality;
		}
		set
		{
			_nationality = value;
		}
	}

	[ProtoMember(7, IsRequired = false, Name = "visitor", DataFormat = DataFormat.TwosComplement)]
	[DefaultValue(0)]
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

	[ProtoMember(8, IsRequired = false, Name = "head", DataFormat = DataFormat.TwosComplement)]
	[DefaultValue(0)]
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

	[ProtoMember(9, IsRequired = false, Name = "level", DataFormat = DataFormat.TwosComplement)]
	[DefaultValue(0)]
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

	[DefaultValue(0)]
	[ProtoMember(10, IsRequired = false, Name = "exp", DataFormat = DataFormat.TwosComplement)]
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

	[ProtoMember(11, IsRequired = false, Name = "life", DataFormat = DataFormat.TwosComplement)]
	[DefaultValue(0)]
	public int life
	{
		get
		{
			return _life;
		}
		set
		{
			_life = value;
		}
	}

	[ProtoMember(12, IsRequired = false, Name = "jewel", DataFormat = DataFormat.TwosComplement)]
	[DefaultValue(0)]
	public int jewel
	{
		get
		{
			return _jewel;
		}
		set
		{
			_jewel = value;
		}
	}

	[ProtoMember(13, IsRequired = false, Name = "server_time", DataFormat = DataFormat.TwosComplement)]
	[DefaultValue(0f)]
	public ulong server_time
	{
		get
		{
			return _server_time;
		}
		set
		{
			_server_time = value;
		}
	}

	[ProtoMember(14, IsRequired = false, Name = "life_time", DataFormat = DataFormat.TwosComplement)]
	[DefaultValue(0f)]
	public ulong life_time
	{
		get
		{
			return _life_time;
		}
		set
		{
			_life_time = value;
		}
	}

	[ProtoMember(15, IsRequired = false, Name = "upload", DataFormat = DataFormat.TwosComplement)]
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

	[ProtoMember(16, IsRequired = false, Name = "testify", DataFormat = DataFormat.TwosComplement)]
	[DefaultValue(0)]
	public int testify
	{
		get
		{
			return _testify;
		}
		set
		{
			_testify = value;
		}
	}

	[DefaultValue(0f)]
	[ProtoMember(17, IsRequired = false, Name = "exp_time", DataFormat = DataFormat.TwosComplement)]
	public ulong exp_time
	{
		get
		{
			return _exp_time;
		}
		set
		{
			_exp_time = value;
		}
	}

	[DefaultValue(0)]
	[ProtoMember(18, IsRequired = false, Name = "guide", DataFormat = DataFormat.TwosComplement)]
	public int guide
	{
		get
		{
			return _guide;
		}
		set
		{
			_guide = value;
		}
	}

	[ProtoMember(19, IsRequired = false, Name = "mapid", DataFormat = DataFormat.TwosComplement)]
	[DefaultValue(0)]
	public int mapid
	{
		get
		{
			return _mapid;
		}
		set
		{
			_mapid = value;
		}
	}

	[DefaultValue(0)]
	[ProtoMember(20, IsRequired = false, Name = "support", DataFormat = DataFormat.TwosComplement)]
	public int support
	{
		get
		{
			return _support;
		}
		set
		{
			_support = value;
		}
	}

	[DefaultValue(0)]
	[ProtoMember(21, IsRequired = false, Name = "review", DataFormat = DataFormat.TwosComplement)]
	public int review
	{
		get
		{
			return _review;
		}
		set
		{
			_review = value;
		}
	}

	[ProtoMember(22, IsRequired = false, Name = "notify_uri", DataFormat = DataFormat.Default)]
	[DefaultValue("")]
	public string notify_uri
	{
		get
		{
			return _notify_uri;
		}
		set
		{
			_notify_uri = value;
		}
	}

	[ProtoMember(23, IsRequired = false, Name = "init_life", DataFormat = DataFormat.TwosComplement)]
	[DefaultValue(0)]
	public int init_life
	{
		get
		{
			return _init_life;
		}
		set
		{
			_init_life = value;
		}
	}

	[DefaultValue(0)]
	[ProtoMember(24, IsRequired = false, Name = "life_per_time", DataFormat = DataFormat.TwosComplement)]
	public int life_per_time
	{
		get
		{
			return _life_per_time;
		}
		set
		{
			_life_per_time = value;
		}
	}

	[ProtoMember(25, IsRequired = false, Name = "challenge_start", DataFormat = DataFormat.TwosComplement)]
	[DefaultValue(0)]
	public int challenge_start
	{
		get
		{
			return _challenge_start;
		}
		set
		{
			_challenge_start = value;
		}
	}

	[ProtoMember(26, IsRequired = false, Name = "download_num", DataFormat = DataFormat.TwosComplement)]
	[DefaultValue(0)]
	public int download_num
	{
		get
		{
			return _download_num;
		}
		set
		{
			_download_num = value;
		}
	}

	[ProtoMember(27, IsRequired = false, Name = "download_max", DataFormat = DataFormat.TwosComplement)]
	[DefaultValue(0)]
	public int download_max
	{
		get
		{
			return _download_max;
		}
		set
		{
			_download_max = value;
		}
	}

	IExtension IExtensible.GetExtensionObject(bool createIfMissing)
	{
		return Extensible.GetExtensionObject(ref extensionObject, createIfMissing);
	}
}
