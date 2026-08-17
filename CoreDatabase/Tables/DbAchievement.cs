using DOL.Database.Attributes;

namespace DOL.Database
{
	[DataTable(TableName = "Achievement")]
	public class DbAchievement : DataObject
	{
		private string m_achievementKey = string.Empty;
		private string m_name = string.Empty;
		private string m_description = string.Empty;
		private string m_category = "General";
		private int m_points;
		private bool m_hidden;
		private int m_minLevel;

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
		public string Name
		{
			get => m_name;
			set
			{
				Dirty = true;
				m_name = value;
			}
		}

		[DataElement(AllowDbNull = true)]
		public string Description
		{
			get => m_description;
			set
			{
				Dirty = true;
				m_description = value;
			}
		}

		[DataElement(AllowDbNull = false)]
		public string Category
		{
			get => m_category;
			set
			{
				Dirty = true;
				m_category = value;
			}
		}

		[DataElement(AllowDbNull = false)]
		public int Points
		{
			get => m_points;
			set
			{
				Dirty = true;
				m_points = value;
			}
		}

		[DataElement(AllowDbNull = false)]
		public bool Hidden
		{
			get => m_hidden;
			set
			{
				Dirty = true;
				m_hidden = value;
			}
		}

		[DataElement(AllowDbNull = false)]
		public int MinLevel
		{
			get => m_minLevel;
			set
			{
				Dirty = true;
				m_minLevel = value;
			}
		}
	}
}
