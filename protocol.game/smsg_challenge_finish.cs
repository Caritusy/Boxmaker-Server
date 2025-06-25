using System;
using System.Collections.Generic;
using System.ComponentModel;
using ProtoBuf;

namespace protocol.game;

[Serializable]
[ProtoContract(Name = "smsg_challenge_finish")]
public class smsg_challenge_finish : IExtensible
{
	private int _suc;

	private int _exp;

	private int _rank;

	private List<author_list> _authors = new List<author_list>();

	private int _jewel;

	private IExtension extensionObject;

	[ProtoMember(1, IsRequired = false, Name = "suc", DataFormat = DataFormat.TwosComplement)]
	[DefaultValue(0)]
	public int suc
	{
		get
		{
			return _suc;
		}
		set
		{
			_suc = value;
		}
	}

	[DefaultValue(0)]
	[ProtoMember(2, IsRequired = false, Name = "exp", DataFormat = DataFormat.TwosComplement)]
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

	[DefaultValue(0)]
	[ProtoMember(3, IsRequired = false, Name = "rank", DataFormat = DataFormat.TwosComplement)]
	public int rank
	{
		get
		{
			return _rank;
		}
		set
		{
			_rank = value;
		}
	}

	[ProtoMember(4, Name = "authors", DataFormat = DataFormat.Default)]
	public List<author_list> authors
	{
		get
		{
			return _authors;
		}
		set
		{
			_authors = value;
		}
	}

	[DefaultValue(0)]
	[ProtoMember(5, IsRequired = false, Name = "jewel", DataFormat = DataFormat.TwosComplement)]
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
