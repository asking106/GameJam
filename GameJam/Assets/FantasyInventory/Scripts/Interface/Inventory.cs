using System.Collections.Generic;
using System.Linq;
using Assets.FantasyInventory.Scripts.Data;
using Assets.FantasyInventory.Scripts.Enums;
using Assets.FantasyInventory.Scripts.GameData;
using Assets.FantasyInventory.Scripts.Interface.Elements;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.FantasyInventory.Scripts.Interface
{
    /// <summary>
    /// High-level inventory inverface.
    /// </summary>
    public class Inventory : ItemWorkspace
    {
        public Equipment Equipment;
        public ScrollInventory Bag;
        public Button EquipButton;
        public Button RemoveButton;
        public AudioSource AudioSource;
        public AudioClip EquipSound;
        public AudioClip RemoveSound;
        public AudioClip drinkSound;
        public Text equipOrDrinkText;
        /// <summary>
        /// Initialize owned items (just for example).
        /// </summary>
        public void Awake()
        {
            var inventory = new List<Item>
            {
                
                new Item(ItemId.生锈铁剑,1),
                new Item(ItemId.木剑,1),
                new Item(ItemId.玄铁剑,1),
                new Item(ItemId.Gold,1),
                new Item (ItemId.HealthPotion,1),
                new Item (ItemId.ManaPotion,1),
                new Item(ItemId.布袍,1),
                new Item(ItemId.兜帽,1),
                 new Item(ItemId.青丝巾,1),
            };

            var equipped = new List<Item>();

            Bag.Initialize(ref inventory);
            Equipment.Initialize(ref equipped);
        }

        protected void Start()
        {
            Reset();
            EquipButton.interactable = RemoveButton.interactable = false;

            // TODO: Assigning static callbacks. Don't forget to set null values when UI will be closed. You can also use events instead.
            InventoryItem.OnItemSelected = SelectItem;
            InventoryItem.OnDragStarted = SelectItem;
            InventoryItem.OnDragCompleted = InventoryItem.OnDoubleClick = item => { if (Bag.Items.Contains(item)) Equip(); else Remove(); };
        }

        public void SelectItem(Item item)
        {
            SelectItem(item.Id);
        }

        public void SelectItem(ItemId itemId)
        {
            SelectedItem = itemId;
            SelectedItemParams = Items.Params[itemId];
            ItemInfo.Initialize(SelectedItem, SelectedItemParams);
            Refresh();
        }

        public void Equip()
        {
            var equipped = Equipment.Items.LastOrDefault(i => i.Params.Type == SelectedItemParams.Type);

            if (equipped != null)
            {
                AutoRemove(SelectedItemParams.Type, Equipment.Slots.Count(i => i.ItemType == SelectedItemParams.Type));
            }

            if (SelectedItemParams.Tags.Contains(ItemTag.双手武器))
            {
                var shield = Equipment.Items.SingleOrDefault(i => i.Params.Type == ItemType.Shield);

                if (shield != null)
                {
                    MoveItem(shield, Equipment, Bag);
                }
                
            }
            else if (SelectedItemParams.Type == ItemType.Shield)
            {
                var weapon2H = Equipment.Items.SingleOrDefault(i => i.Params.Tags.Contains(ItemTag.TwoHanded));

                if (weapon2H != null)
                {
                    MoveItem(weapon2H, Equipment, Bag);
                }
            }

            MoveItem(SelectedItem, Bag, Equipment);
            if (SelectedItemParams.Type != ItemType.Potion)
            { AudioSource.PlayOneShot(EquipSound); }
            else
            {
                AudioSource.PlayOneShot(drinkSound);
            }
        }

        public void Remove()
        {
            MoveItem(SelectedItem, Equipment, Bag);
            SelectItem(Equipment.Items.FirstOrDefault(i => i.Id == SelectedItem) ?? Bag.Items.Single(i => i.Id == SelectedItem));
            AudioSource.PlayOneShot(RemoveSound);
        }

        public override void Refresh()
        {
            if (SelectedItem == ItemId.Undefined)
            {
                ItemInfo.Reset();
                EquipButton.interactable = RemoveButton.interactable = false;
            }
            else
            {
                if (CanEquip())
                {
                    equipOrDrinkText.text = "装备";
                    EquipButton.interactable = Bag.Items.Any(i => i.Id == SelectedItem)
                        && Equipment.Slots.Count(i => i.ItemType == SelectedItemParams.Type) > Equipment.Items.Count(i => i.Id == SelectedItem);
                    RemoveButton.interactable = Equipment.Items.Any(i => i.Id == SelectedItem);
                    ItemInfo.Price.enabled = !SelectedItemParams.Tags.Contains(ItemTag.NotForSale);
                }
                else
                {
                    equipOrDrinkText.text = "装备";
                    EquipButton.interactable = RemoveButton.interactable = false;
                    if (SelectedItem == ItemId.ManaPotion|| SelectedItem == ItemId.HealthPotion)
                    {
                        equipOrDrinkText.text = "喝";
                        EquipButton.interactable = RemoveButton.interactable = true;
                    }
                    if (SelectedItem == ItemId.Gold )
                    {
                        equipOrDrinkText.text = "不可使用";
                        
                    }

                }
            }
        }

        private bool CanEquip()
        {
            return Equipment.Slots.Any(i => i.ItemType == SelectedItemParams.Type && i.ItemTags.All(j => SelectedItemParams.Tags.Contains(j)));
        }

        /// <summary>
        /// Automatically removes items if target slot is busy.
        /// </summary>
        private void AutoRemove(ItemType itemType, int max)
        {
            var items = Equipment.Items.Where(i => i.Params.Type == itemType).ToList();
            long sum = 0;

            foreach (var p in items)
            {
                sum += p.Count;
            }

            if (sum == max)
            {
                MoveItem(items.LastOrDefault(i => i.Id != SelectedItem) ?? items.Last(), Equipment, Bag);
            }
        }
    }
}