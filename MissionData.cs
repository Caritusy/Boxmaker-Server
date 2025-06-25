using protocol.game;

namespace BoxMaker_Server
{
	public class MissionData
	{
		public smsg_mission_view missionData = new smsg_mission_view();

		public List<ServerMap> randomMaps = new List<ServerMap>();

		public int unlocked = 1;

		public List<author_list> GetAuthorList()
		{
			var list = new List<author_list>();
			for (int i = 0; i < randomMaps.Count; i++)
			{
				list.Add(new author_list
				{
					map_name = randomMaps[i].map.name,
					user_country = randomMaps[i].map.country,
					user_head = randomMaps[i].map.head,
					user_name = randomMaps[i].map.owner_name,
				});
			}
			return list;
		}
	}
}
