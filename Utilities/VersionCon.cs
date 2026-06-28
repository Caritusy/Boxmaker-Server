namespace BoxMaker_Server
{
	public class VersionCon : IComparable<VersionCon>
	{
		public int Major { get; }
		public int Minor { get; }

		public VersionCon(string versionString)
		{
			string[] parts = versionString.Split('.');
			if (parts.Length != 2)
			{
				throw new ArgumentException("Invalid version format. Version string must be in the format 'major.minor'");
			}

			if (!int.TryParse(parts[0], out int major) || !int.TryParse(parts[1], out int minor))
			{
				throw new ArgumentException("Invalid version format. Major and minor version must be integers.");
			}

			Major = major;
			Minor = minor;
		}

		public int CompareTo(VersionCon other)
		{
			if (ReferenceEquals(this, other)) return 0;
			if (ReferenceEquals(null, other)) return 1;

			int majorComparison = Major.CompareTo(other.Major);
			if (majorComparison != 0) return majorComparison;

			return Minor.CompareTo(other.Minor);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((VersionCon)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (Major * 397) ^ Minor;
			}
		}

		protected bool Equals(VersionCon other)
		{
			return Major == other.Major && Minor == other.Minor;
		}

		public static bool operator <(VersionCon v1, VersionCon v2)
		{
			return v1.CompareTo(v2) < 0;
		}

		public static bool operator >(VersionCon v1, VersionCon v2)
		{
			return v1.CompareTo(v2) > 0;
		}

		public static bool operator ==(VersionCon v1, VersionCon v2)
		{
			return v1.CompareTo(v2) == 0;
		}

		public static bool operator !=(VersionCon v1, VersionCon v2)
		{
			return v1.CompareTo(v2) != 0;
		}
	}
}
