using System.Collections.Generic;
using DOL.GS.Realm;
using DOL.GS.ServerProperties;

namespace DOL.GS
{
	public static class CharacterClassRaceRules
	{
		public static bool IsRaceEligible(ICharacterClass characterClass, eRace race, eRealm realm)
		{
			if (Properties.ALLOW_ALL_REALM_RACES_FOR_CLASSES)
				return PlayerRace.TryGetRace(race, out PlayerRace playerRace) && playerRace.Realm == realm;

			return characterClass.EligibleRaces.Exists(r => r.ID == race);
		}

		public static List<PlayerRace> GetEligibleRaces(ICharacterClass characterClass, eRealm realm)
		{
			if (Properties.ALLOW_ALL_REALM_RACES_FOR_CLASSES)
				return PlayerRace.GetRacesForRealm(realm);

			return characterClass.EligibleRaces;
		}
	}
}
