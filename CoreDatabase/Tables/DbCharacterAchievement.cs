using System;
using DOL.Database.Attributes;

namespace DOL.Database
{
	[DataTable(TableName = "CharacterAchievement")]
	public class DbCharacterAchievement : DataObject
	{
		private string m_characterId = string.Empty;
		private string m_achievementKey = string.Empty;
		private DateTime m_unlockedAt = DateTime.UtcNow;

		[PrimaryKey]
		public string Character_ID
		{
			get => m_characterId;
			set
			{
				Dirty = true;
				m_characterId = value;
			}
		}

		[PrimaryKey]
		public string AchievementKey
		{
			get => m_achievementKey;
			set
			{
				Dirty = true;
				m_achievementKey = value;
			}
		}

		[DataElement(AllowDbNull = false)]
		public DateTime UnlockedAt
		{
			get => m_unlockedAt;
			set
			{
				Dirty = true;
				m_unlockedAt = value;
			}
		}
	}
}
