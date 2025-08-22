using System.Collections.Generic;
using Assets.FantasyInventory.Scripts.Data;
using Assets.FantasyInventory.Scripts.Enums;

namespace Assets.FantasyInventory.Scripts.GameData
{
  
    public class Items
    {
        public static readonly Dictionary<ItemId, ItemParams> Params = new Dictionary<ItemId, ItemParams>
        {




            {
                ItemId.仙女剑,
                new ItemParams
                {
                    Type = ItemType.Weapon,
                    Tags = new List<ItemTag> { ItemTag.双手武器 },
                    Properties = new List<Property> { new Property(PropertyId.PhysicDamage, 10) },

                    Price = 140
                     
                }
            },

             {
                ItemId.生锈铁剑,
                new ItemParams
                {
                    Type = ItemType.Weapon,
                     Tags = new List<ItemTag> { ItemTag.单手武器 },
                    Properties = new List<Property> { new Property(PropertyId.PhysicDamage, 15) },
                    Price = 200
                }
            },
               {
                ItemId.木剑,
                new ItemParams
                {
                    Type = ItemType.Weapon,
                     Tags = new List<ItemTag> { ItemTag.单手武器 },
                    Properties = new List<Property> { new Property(PropertyId.PhysicDamage, 20) },
                    Price = 230
                }
            },
                 {
                ItemId.玄铁剑,
                new ItemParams
                {
                    Type = ItemType.Weapon,
                     Tags = new List<ItemTag> { ItemTag.单手武器,ItemTag.不可出售},
                    Properties = new List<Property> { new Property(PropertyId.PhysicDamage, 18) },
                    Price = 210
                }
            },








                 {
                ItemId.兜帽,
                new ItemParams
                {
                    Type = ItemType.Helmet,
                    
                    Properties = new List<Property> { new Property(PropertyId.PhysicDefense, 10) },

                    Price = 70

                }
            },


                 {
                ItemId.布袍,
                new ItemParams
                {
                    Type = ItemType.Armor,

                    Properties = new List<Property> { new Property(PropertyId.PhysicDefense, 13) },

                    Price = 90

                } 
            },
                     {
                ItemId.青丝巾,
                new ItemParams
                {
                    Type = ItemType.Necklace,

                    Properties = new List<Property> { new Property(PropertyId.PhysicDefense, 2) },

                    Price = 40

                } 
            },













            {
                ItemId.Scroll,
                new ItemParams
                {
                    Type = ItemType.Scroll,
                    Properties = new List<Property> { new Property(PropertyId.MagicDamage, 100) },
                    Price = 1000
                }
            },
            {
                ItemId.Flute,
                new ItemParams
                {
                    Type = ItemType.Loot,
                    Price = 10
                }
            },
            {
                ItemId.Gold,
                new ItemParams
                {
                    Type = ItemType.Currency,
                    Tags = new List<ItemTag> { ItemTag.NotForSale }
                }
            },
            {
                ItemId.HealthPotion,
                new ItemParams
                {
                    Type = ItemType.Potion,
                    Properties = new List<Property> { new Property(PropertyId.RestoreHealth, 50) },
                    Price = 200
                }
            },
            {
                ItemId.Sword,
                new ItemParams
                {
                    Type = ItemType.Weapon,
                    Tags = new List<ItemTag> { ItemTag.Sword },
                    Properties = new List<Property> { new Property(PropertyId.PhysicDamage, 10) },
                    Price = 1000
                }
            },
            {
                ItemId.Bow,
                new ItemParams
                {
                    Type = ItemType.Weapon,
                    Tags = new List<ItemTag> { ItemTag.Bow, ItemTag.TwoHanded },
                    Properties = new List<Property> { new Property(PropertyId.PhysicDamage, 15) },
                    Price = 2000
                }
            },
            {
                ItemId.Armor,
                new ItemParams
                {
                    Type = ItemType.Armor,
                    Properties = new List<Property> { new Property(PropertyId.PhysicDefense, 10) },
                    Price = 1000
                }
            },
            {
                ItemId.Helmet,
                new ItemParams
                {
                    Type = ItemType.Helmet,
                    Properties = new List<Property> { new Property(PropertyId.PhysicDamage, 5) },
                    Price = 500
                }
            },
            {
                ItemId.ManaPotion,
                new ItemParams
                {
                    Type = ItemType.Potion,
                    Properties = new List<Property> { new Property(PropertyId.RestoreMana, 50) },
                    Price = 200
                }
            },
            {
                ItemId.Shield,
                new ItemParams
                {
                    Type = ItemType.Shield,
                    Properties = new List<Property> { new Property(PropertyId.PhysicDamage, 10) },
                    Price = 1000
                }
            },
            {
                ItemId.SilverRing,
                new ItemParams
                {
                    Type = ItemType.Ring,
                    Properties = new List<Property> { new Property(PropertyId.MagicDefense, 5) },
                    Price = 500
                }
            },
            {
                ItemId.Spear,
                new ItemParams
                {
                    Type = ItemType.Weapon,
                    Tags = new List<ItemTag> { ItemTag.Spear, ItemTag.TwoHanded },
                    Properties = new List<Property> { new Property(PropertyId.PhysicDamage, 15) },
                    Price = 2500
                }
            },
            {
                ItemId.StoneAmulet,
                new ItemParams
                {
                    Type = ItemType.Necklace,
                    Properties = new List<Property> { new Property(PropertyId.MagicDefense, 10) },
                    Price = 1000
                }
            },
            {
                ItemId.TwoHandedSword,
                new ItemParams
                {
                    Type = ItemType.Weapon,
                    Tags = new List<ItemTag> { ItemTag.Sword, ItemTag.TwoHanded },
                    Properties = new List<Property> { new Property(PropertyId.PhysicDamage, 20) },
                    Price = 5000
                }
            },
            {
                ItemId.Axe,
                new ItemParams
                {
                    Type = ItemType.Weapon,
                    Tags = new List<ItemTag> { ItemTag.Axe },
                    Properties = new List<Property> { new Property(PropertyId.PhysicDamage, 5) },
                    Price = 100
                }
            }
        };
    }
}