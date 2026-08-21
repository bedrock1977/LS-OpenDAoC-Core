using System;
using DOL.Database.Attributes;

namespace DOL.Database
{
	[DataTable(TableName = "CharacterAchievement")]
	public class DbCharacterAchievement : DataObject
	{
		private int m_id;
		private string m_characterId = string.Empty;
		private string m_achievementKey = string.Empty;
		private DateTime m_unlockedAt = DateTime.UtcNow;

		[PrimaryKey(AutoIncrement = true)]
		public int ID
		{
			get => m_id;
			set
			{
				Dirty = true;
				m_id = value;
			}
		}

		[DataElement(AllowDbNull = false, Varchar = 100, Index = true, UniqueColumns = "AchievementKey")]
		public string Character_ID
		{
			get => m_characterId;
			set
			{
				Dirty = true;
				m_characterId = value;
			}
		}

		[DataElement(AllowDbNull = false, Varchar = 64, Index = true)]
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
