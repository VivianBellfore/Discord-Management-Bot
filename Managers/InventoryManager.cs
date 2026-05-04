
using System.Threading.Tasks;
using System.Collections.Generic;



namespace LCNET_Management_Bot
{
    internal class InventoryManager
    {
        /// <summary>
        /// Contains all item objects from data base. Is loaded on bot start once.
        /// </summary>
        internal static Dictionary<int, ItemObject> ExistingItems = new Dictionary<int, ItemObject>();

        /// <summary>
        /// Fetching all entrys from `discord_items` data base table and adding them to as item object class into the `ExistingItems` dictionary.
        /// </summary>
        internal static async Task LoadItems()
        {
            ExistingItems.Clear();

            List<dynamic> itemList = await MySqlWrapper.SQLExecuteReader(
                "SELECT * FROM `discord_items`",
                new Dictionary<string, object>() { });

            foreach (dynamic item in itemList)
            {
                ExistingItems.Add(item.id, new ItemObject(item.id, item.name, item.card_url, item.item_type));
            }
        }

        /// <summary>
        /// Fetching items from `ExistingItems` list by item type string.
        /// </summary>
        internal static List<int> GetItemsByType(string itemType)
        {
            List<int> items = new List<int>();

            foreach ( var entry in ExistingItems)
            {
                if ( entry.Value.ItemType == itemType )
                    items.Add(entry.Key);
            }

            return items;
        }

        /// <summary>
        /// Adding an item to the user inventory in data base.
        /// </summary>
        internal static async Task<bool> GiveItemToUser(ulong userId, int itemId, int amount)
        {
            bool isUpdated = await MySqlWrapper.SetIntegerForIdentifier("user_dc_inventory", "amount", new Dictionary<string, object> { { "user_id", userId }, { "item_id", itemId } }, amount, 1, false);

            if ( isUpdated ) return true;

            int insertCount = await MySqlWrapper.SQLExecuteNonQuery(
                "INSERT INTO `user_dc_inventory` (`user_id`, `item_id`, `amount`) VALUES (@user_id, @item_id, @amount)",
                new Dictionary<string, object>() { { "user_id", userId }, { "item_id", itemId }, { "amount", amount } });

            if ( insertCount > 0 ) return true;

            await Utilities.SendDevLogMessage(1, $"Could not insert item for user. User is ||{userId}|| and item is {itemId} with amount {amount}.");

            return false;
        }

        /// <summary>
        /// Removing an item from the user inventory in data base.
        /// </summary>
        internal static async Task<bool> RemoveItemFromUser(ulong userId, int itemId, int amount)
        {
            bool isUpdated = await MySqlWrapper.SetIntegerForIdentifier("user_dc_inventory", "amaount", new Dictionary<string, object> { { "user_id", userId }, { "item_id", itemId } }, amount, 2, false);

            if (isUpdated) return true;

            await Utilities.SendDevLogMessage(1, $"Could not remove item from user. User is ||{userId}|| and item is {itemId} with amount {amount}.");

            return false;
        }
    }



    internal class ItemObject
    {
        internal int Id { get; set; }
        internal string Name { get; set; }
        internal string CardURL { get; set; }
        internal string ItemType { get; set; }

        internal ItemObject(int id, string name, string cardUrl, string itemType)
        {
            Id = id;
            Name = name;
            CardURL = cardUrl;
            ItemType = itemType;
        }
    }
}
