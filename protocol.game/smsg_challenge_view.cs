using System;
using System.ComponentModel;
using ProtoBuf;

namespace protocol.game;

[Serializable]
[ProtoContract(Name = "smsg_challenge_view")]
public class smsg_challenge_view : IExtensible
{
	private int _index;

	private int _top;

	private int _life;

	private int _start;

	private int _exp;

	private string _date = string.Empty;

	private string _subject = string.Empty;

	private int _life_num;

	private int _jewel;

	private IExtension extensionObject;

	[DefaultValue(0)]
	[ProtoMember(1, IsRequired = false, Name = "index", DataFormat = DataFormat.TwosComplement)]
	public int index
	{
		get
		{
			return _index;
		}
		set
		{
			_index = value;
		}
	}

	[ProtoMember(2, IsRequired = false, Name = "top", DataFormat = DataFormat.TwosComplement)]
	[DefaultValue(0)]
	public int top
	{
		get
		{
			return _top;
		}
		set
		{
			_top = value;
		}
	}

	[DefaultValue(0)]
	[ProtoMember(3, IsRequired = false, Name = "life", DataFormat = DataFormat.TwosComplement)]
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

	[ProtoMember(4, IsRequired = false, Name = "start", DataFormat = DataFormat.TwosComplement)]
	[DefaultValue(0)]
	public int start
	{
		get
		{
			return _start;
		}
		set
		{
			_start = value;
		}
	}

	[DefaultValue(0)]
	[ProtoMember(5, IsRequired = false, Name = "exp", DataFormat = DataFormat.TwosComplement)]
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

	[DefaultValue("")]
	[ProtoMember(6, IsRequired = false, Name = "date", DataFormat = DataFormat.Default)]
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

	[DefaultValue("")]
	[ProtoMember(7, IsRequired = false, Name = "subject", DataFormat = DataFormat.Default)]
	public string subject
	{
		get
		{
			return _subject;
		}
		set
		{
			_subject = value;
		}
	}

	[ProtoMember(8, IsRequired = false, Name = "life_num", DataFormat = DataFormat.TwosComplement)]
	[DefaultValue(0)]
	public int life_num
	{
		get
		{
			return _life_num;
		}
		set
		{
			_life_num = value;
		}
	}

	[DefaultValue(0)]
	[ProtoMember(9, IsRequired = false, Name = "jewel", DataFormat = DataFormat.TwosComplement)]
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

	IExtension IExtensible.GetExtensionObject(bool createIfMissing)
	{
		return Extensible.GetExtensionObject(ref extensionObject, createIfMissing);
	}
}
