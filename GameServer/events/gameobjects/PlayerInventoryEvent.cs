namespace DOL.Events
{
	/// <summary>
	/// This class holds all possible player inventory events.
	/// Only constants defined here!
	/// </summary>
	public class PlayerInventoryEvent : DOLEvent
	{
		/// <summary>
		/// Constructs a new PlayerInventory event
		/// </summary>
		public PlayerInventoryEvent(string name) : base(name) { }

		/// <summary>
		/// The item was just dropped
		/// </summary>
		public static readonly PlayerInventoryEvent ItemDropped = new PlayerInventoryEvent("PlayerInventory.ItemDropped");
        /// <summary>
        /// A bonus on an item changed.
        /// </summary>
        public static readonly PlayerInventoryEvent ItemBonusChanged = new PlayerInventoryEvent("PlayerInventory.ItemBonusChanged");
    }
}
